using Application.DTOs.Account;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Inbound.Commands
{
    public partial class CreateInboundBatchCommand : IRequest<Response<int>>
    {
        public int? AcquisitionId { get; set; }
        public List<InboundCreateDto> Items { get; set; }
    }

    public class InboundCreateDto 
    { 
        public int InventoryId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CreateInboundBatchCommandHandler : IRequestHandler<CreateInboundBatchCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IInboundRepositoryAsync _inboundRepository;
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IMapper _mapper;

        public CreateInboundBatchCommandHandler(IAuthenticatedUserService authenticateUser, IInboundRepositoryAsync inboundRepository, IInventoryRepositoryAsync inventoryRepository, IMapper mapper)
        {
            _authenticatedUser = authenticateUser;
            _inboundRepository = inboundRepository;
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateInboundBatchCommand command, CancellationToken cancellationToken)
        {
            foreach (var item in command.Items)
            {
                var inventory = await _inventoryRepository.GetByIdAsync(item.InventoryId);

                if (inventory == null)
                    throw new ApiException("Inventory not found.");

                var inbound = new Domain.Entities.Inbound
                {
                    InventoryId = item.InventoryId,
                    Quantity = item.Quantity, 
                    RemainingQuantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.UnitPrice * item.Quantity,
                    Remark = command.AcquisitionId != null ? $@"AC{command.AcquisitionId:D4}" : null,
                    AcquisitionId = command.AcquisitionId,
                    Created = DateTime.UtcNow,
                    CreatedBy = _authenticatedUser.UserId
                };
                await _inboundRepository.AddAsync(inbound);

                int oldQty = inventory.TotalQty;
                decimal oldAvgPrice = inventory.AverageUnitPrice;

                int newQty = item.Quantity;
                decimal newPrice = item.UnitPrice;

                inventory.TotalQty += newQty;
                inventory.AvailableQty = inventory.TotalQty - inventory.ReservedQty;
                inventory.LastUnitPrice = newPrice;
                inventory.LastInDate = DateTime.UtcNow;

                inventory.AverageUnitPrice = oldQty + newQty == 0
                    ? newPrice
                    : ((oldQty * oldAvgPrice) + (newQty * newPrice)) / (oldQty + newQty);

                await _inventoryRepository.UpdateAsync(inventory);
            }

            return new Response<int>(command.Items.Count, "Inbound records created");
        }
    }
}