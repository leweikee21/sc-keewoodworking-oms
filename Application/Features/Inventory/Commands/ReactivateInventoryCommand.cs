using Application.DTOs.Inventory;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Features.Inventory.Commands
{
    public class ReactivateInventoryCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public ReactivateInventoryRequest Request { get; set; }
    }

    public class ReactivateInventoryCommandHandler : IRequestHandler<ReactivateInventoryCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IInboundRepositoryAsync _inboundRepository;
        private readonly IMapper _mapper;

        public ReactivateInventoryCommandHandler(IAuthenticatedUserService authenticateUser, IInventoryRepositoryAsync inventoryRepository, IInboundRepositoryAsync inboundRepository, IMapper mapper)
        {
            _authenticatedUser = authenticateUser;
            _inventoryRepository = inventoryRepository;
            _inboundRepository = inboundRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(ReactivateInventoryCommand command, CancellationToken cancellationToken)
        {
            var existing = await _inventoryRepository.GetByIdAsync(command.Id);

            if (existing == null || !existing.IsDeleted)
                throw new ApiException("Inventory not found or not deleted.");

            existing.IsDeleted = false;
            existing.Name = command.Request.Name;
            existing.Category = command.Request.Category;
            existing.Unit = command.Request.Unit;
            existing.TotalQty = command.Request.TotalQty;
            existing.ReservedQty = 0;
            existing.LastUnitPrice = command.Request.UnitPrice;
            existing.AverageUnitPrice = command.Request.UnitPrice;
            existing.LastInDate = DateTime.UtcNow;
            if (command.Request.SupplierId != null) 
                { existing.SupplierId = command.Request.SupplierId; }
            else 
                { existing.SupplierId = null; }
            existing.MinQty = command.Request.MinQty;
            existing.AvailableQty = command.Request.TotalQty - existing.ReservedQty;
            existing.LastModified = DateTime.UtcNow;
            existing.LastModifiedBy = _authenticatedUser.UserId;

            await _inventoryRepository.UpdateAsync(existing);

            var inbound = new Domain.Entities.Inbound
            {
                InventoryId = existing.Id,
                Quantity = existing.TotalQty,
                RemainingQuantity = existing.TotalQty,
                UnitPrice = command.Request.UnitPrice,
                TotalPrice = command.Request.UnitPrice * existing.TotalQty,
                Remark = "Reactivated with new stock",
                AcquisitionId = null,
                Created = DateTime.UtcNow,
                CreatedBy = _authenticatedUser.UserId
            };

            await _inboundRepository.AddAsync(inbound);

            return new Response<int>(existing.Id, "Inventory reactivated successfully.");
        }
    }
}