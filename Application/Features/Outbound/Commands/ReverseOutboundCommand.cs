using Application.DTOs.Account;
using Application.DTOs.Email;
using Application.DTOs.Outbound;
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

namespace Application.Features.Outbound.Commands 
{
    public partial class ReverseOutboundCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public string Reason { get; set; }
    }

    public class ReverseOutboundCommandHandler : IRequestHandler<ReverseOutboundCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IInboundRepositoryAsync _inboundRepository;
        private readonly IOutboundRepositoryAsync _outboundRepository;
        private readonly IMapper _mapper;

        public ReverseOutboundCommandHandler(IAuthenticatedUserService authenticateUser, IInventoryRepositoryAsync inventoryRepository,
                                         IInboundRepositoryAsync inboundRepository, IOutboundRepositoryAsync outboundRepository,
                                         IMapper mapper)
        {
            _authenticatedUser = authenticateUser;
            _inventoryRepository = inventoryRepository;
            _inboundRepository = inboundRepository;
            _outboundRepository = outboundRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(ReverseOutboundCommand command, CancellationToken cancellationToken)
        {
            var outbound = await _outboundRepository.GetByIdAsync(command.Id);

            if (outbound == null)
                throw new ApiException("Outbound not found.");

            if (outbound.OrderId != null)
                throw new ApiException($@"Cannot reverse: Linked to OPO{outbound.OrderId:D6}");

            var inbound = outbound.Inbound;
            var inventory = outbound.Inventory;

            // Replenish
            inbound.RemainingQuantity += outbound.Quantity;

            inventory.TotalQty += outbound.Quantity;
            inventory.AvailableQty = inventory.TotalQty - inventory.ReservedQty;
            inventory.LastInDate = DateTime.UtcNow;

            await _inboundRepository.UpdateAsync(inbound);
            await _inventoryRepository.UpdateAsync(inventory);

            outbound.Remark = command.Reason;
            outbound.Quantity = 0;
            outbound.TotalPrice = 0;
            outbound.LastModified = DateTime.UtcNow;
            outbound.LastModifiedBy = _authenticatedUser.UserId;
            await _outboundRepository.UpdateAsync(outbound);

            return new Response<int>(outbound.Id, "Outbound reversed successfully.");
        }
    }
}