 using Application.DTOs.Account;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Inventory.Commands
{
    public partial class CreateInventoryCommand : IRequest<Response<int>>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public int TotalQty { get; set; }
        public int MinQty { get; set; }
        public decimal UnitPrice { get; set; }
        public int? SupplierId { get; set; }
    }

    public class CreateInventoryCommandHandler : IRequestHandler<CreateInventoryCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IInboundRepositoryAsync _inboundRepository;
        private readonly IMapper _mapper;

        public CreateInventoryCommandHandler(IAuthenticatedUserService authenticateUser, IInventoryRepositoryAsync inventoryRepository, IInboundRepositoryAsync inboundRepository, IMapper mapper)
        {
            _authenticatedUser = authenticateUser;
            _inventoryRepository = inventoryRepository;
            _inboundRepository = inboundRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateInventoryCommand command, CancellationToken cancellationToken)
        {
            var existing = await _inventoryRepository.GetByCodeAsync(command.Code);

            if (existing != null)
            {
                if (!existing.IsDeleted)
                {
                    throw new ValidationException(new List<ValidationFailure>
                    {
                        new ValidationFailure("", "Inventory with this code has already exixts. Please try another.")
                    });
                }
                else if (existing.IsDeleted)
                {
                    return Response<int>.WithException(existing.Id, "This inventory was previously deleted. Reactivate?", "ReactivateException");
                }
            }
           //new inventory
            var inventory = _mapper.Map<Domain.Entities.Inventory>(command);
            inventory.ReservedQty = 0;
            inventory.AvailableQty = command.TotalQty - inventory.ReservedQty;
            inventory.LastUnitPrice = command.UnitPrice;
            inventory.AverageUnitPrice = command.UnitPrice;
            if (command.SupplierId != null) { inventory.SupplierId = command.SupplierId; }
            inventory.LastInDate = DateTime.UtcNow;
            inventory.Created = DateTime.UtcNow;
            inventory.CreatedBy = _authenticatedUser.UserId;

            var newInventory = await _inventoryRepository.AddAsync(inventory);

            var newInbound = new Domain.Entities.Inbound
            {
                InventoryId = newInventory.Id,
                Quantity = newInventory.TotalQty,
                RemainingQuantity = newInventory.TotalQty,
                UnitPrice = command.UnitPrice,
                TotalPrice = command.UnitPrice * newInventory.TotalQty,
                Remark = "Initial stock",
                AcquisitionId = null, //no acquisition for initial stock
                Created = DateTime.UtcNow,
                CreatedBy = _authenticatedUser.UserId
            };

            await _inboundRepository.AddAsync(newInbound);

            return new Response<int>(newInventory.Id, "Inventory created successfully");
        }
    }
}