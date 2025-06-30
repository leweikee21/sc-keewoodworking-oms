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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Inbound.Commands
{
    public partial class CreateInboundCommand : IRequest<Response<int>>
    {
        public int InventoryId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Remark { get; set; }
        public int? AcquisitionId { get; set; }
    }

    public class CreateInboundCommandHandler : IRequestHandler<CreateInboundCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IInboundRepositoryAsync _inboundRepository;
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IMapper _mapper;

        public CreateInboundCommandHandler(IAuthenticatedUserService authenticateUser, IInboundRepositoryAsync inboundRepository, IInventoryRepositoryAsync inventoryRepository, IMapper mapper)
        {
            _authenticatedUser = authenticateUser;
            _inboundRepository = inboundRepository;
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateInboundCommand command, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(command.InventoryId);

            if (inventory == null)
                throw new ApiException("Inventory not found.");

            var inbound = new Domain.Entities.Inbound
            {
                InventoryId = command.InventoryId,
                Quantity = command.Quantity,
                RemainingQuantity = command.Quantity,
                UnitPrice = command.UnitPrice,
                TotalPrice = command.UnitPrice * command.Quantity,
                Remark = command.Remark,
                AcquisitionId = command.AcquisitionId,
                Created = DateTime.UtcNow,
                CreatedBy = _authenticatedUser.UserId
            };

            await _inboundRepository.AddAsync(inbound);

            int oldQty = inventory.TotalQty;
            decimal oldAvgPrice = inventory.AverageUnitPrice;

            int newQty = command.Quantity;
            decimal newPrice = command.UnitPrice;

            inventory.TotalQty += newQty;
            inventory.AvailableQty = inventory.TotalQty - inventory.ReservedQty;
            inventory.LastUnitPrice = newPrice;
            inventory.LastInDate = DateTime.UtcNow;

            inventory.AverageUnitPrice = oldQty + newQty == 0
                ? newPrice
                : ((oldQty * oldAvgPrice) + (newQty * newPrice)) / (oldQty + newQty);

            await _inventoryRepository.UpdateAsync(inventory);

            return new Response<int>(inbound.Id, "Inbound record created");
        }
    }
}