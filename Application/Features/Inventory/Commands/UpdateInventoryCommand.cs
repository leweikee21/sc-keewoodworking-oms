using Application.DTOs.User;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Inventory.Commands
{
    public class UpdateInventoryCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public int MinQty { get; set; }
        public int? SupplierId { get; set; }
    }

    public class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IMapper _mapper;

        public UpdateInventoryCommandHandler(IAuthenticatedUserService authenticateUser, IInventoryRepositoryAsync inventoryRepository, IMapper mapper)
        {
            _authenticatedUser = authenticateUser;
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateInventoryCommand command, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(command.Id);

            if (inventory == null)
            {
                throw new ApiException($"Inventory not found.");
            }
            else
            {
                inventory.Name = command.Name;
                inventory.Category = command.Category;
                inventory.Unit = command.Unit;
                inventory.MinQty = command.MinQty;
                if (command.SupplierId != null) 
                { inventory.SupplierId = command.SupplierId; }
                else
                { inventory.SupplierId = null; }
                inventory.LastModified = DateTime.UtcNow;
                inventory.LastModifiedBy = _authenticatedUser.UserId;

                await _inventoryRepository.UpdateAsync(inventory);
                return new Response<int>(inventory.Id, "Inventory updated successfully.");
            }
        }
    }
}