using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Inventory.Commands
{
    public class DeleteInventoryByIdCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class DeleteInventoryByIdCommandHandler : IRequestHandler<DeleteInventoryByIdCommand, Response<int>>
    {
        private readonly IInventoryRepositoryAsync _inventoryRepository;

        public DeleteInventoryByIdCommandHandler(IInventoryRepositoryAsync inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<Response<int>> Handle(DeleteInventoryByIdCommand command, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(command.Id);

            if (inventory == null)
                throw new ApiException($"Inventory not found.");
            
            if (inventory.TotalQty > 0)
                throw new ApiException($"Cannot delete inventory with stock");

            inventory.IsDeleted = true;
            await _inventoryRepository.UpdateAsync(inventory);
            return new Response<int>(inventory.Id, "Inventory deleted successfully");
        }
    }

}