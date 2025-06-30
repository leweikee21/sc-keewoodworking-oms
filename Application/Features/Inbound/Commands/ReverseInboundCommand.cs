using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Inbound.Commands 
{
    public partial class ReverseInboundCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public string Reason { get; set; }
    }

    public class ReverseInboundCommandHandler : IRequestHandler<ReverseInboundCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IInboundRepositoryAsync _inboundRepository;

        public ReverseInboundCommandHandler(IAuthenticatedUserService authenticateUser, IInventoryRepositoryAsync inventoryRepository,
                                         IInboundRepositoryAsync inboundRepository)
        {
            _authenticatedUser = authenticateUser;
            _inventoryRepository = inventoryRepository;
            _inboundRepository = inboundRepository;
        }

        public async Task<Response<int>> Handle(ReverseInboundCommand command, CancellationToken cancellationToken)
        {
            var inbound = await _inboundRepository.GetByIdAsync(command.Id);

            if (inbound == null)
                throw new ApiException("Inbound not found.");

            if (inbound.AcquisitionId != null)
                throw new ApiException($@"Cannot reverse: Linked to AC{inbound.AcquisitionId:D4}");

            if (inbound.RemainingQuantity < inbound.Quantity)
                throw new ApiException("Cannot reverse: This inbound has already been partially used.");

            if (inbound.Remark == "Initial stock" || inbound.Remark == "Reactivated with new stock")
                throw new ApiException("Cannot reverse: Initial stock");

            var inventory = inbound.Inventory;
            if (inventory == null)
                throw new ApiException("Related inventory not found.");

            // Replenish
            inventory.TotalQty -= inbound.Quantity;
            inventory.AvailableQty = inventory.TotalQty - inventory.ReservedQty;
            inventory.LastOutDate = DateTime.UtcNow;

            var inbounds = await _inboundRepository.GetByInventoryIdAsync(inventory.Id);
            inbounds = inbounds.Where(i => i.Id != inbound.Id).ToList();

            if (inbounds.Count > 0)
            {
                int totalQty = inbounds.Sum(i => i.RemainingQuantity);
                decimal totalCost = inbounds.Sum(i => i.RemainingQuantity * i.UnitPrice);
                inventory.AverageUnitPrice = totalQty > 0 ? totalCost / totalQty : 0;
            }
            else
            {
                inventory.AverageUnitPrice = 0;
            }

            await _inventoryRepository.UpdateAsync(inventory);

            inbound.Quantity = 0;
            inbound.RemainingQuantity = 0;
            inbound.TotalPrice = 0;
            inbound.Remark = command.Reason;
            inbound.LastModified = DateTime.UtcNow;
            inbound.LastModifiedBy = _authenticatedUser.UserId;
            await _inboundRepository.UpdateAsync(inbound);

            return new Response<int>(inbound.Id, "Inbound reversed successfully.");
        }
    }
}