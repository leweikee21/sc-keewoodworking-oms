using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Order.Commands
{
    public class DeleteOrderByIdCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class DeleteOrderByIdCommandHandler : IRequestHandler<DeleteOrderByIdCommand, Response<int>>
    {
        private readonly IOrderRepositoryAsync _orderRepository;
        private readonly IMaterialUsedRepositoryAsync _materialUsedRepository;
        private readonly IInventoryRepositoryAsync _inventoryRepository;

        public DeleteOrderByIdCommandHandler(IOrderRepositoryAsync orderRepository, IMaterialUsedRepositoryAsync materialUsedRepository, IInventoryRepositoryAsync inventoryRepository)
        {
            _orderRepository = orderRepository;
            _materialUsedRepository = materialUsedRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<Response<int>> Handle(DeleteOrderByIdCommand command, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(command.Id);

            if (order == null)
                throw new ApiException($"Order not found.");

            if (order.Status != "Draft")
                throw new ApiException($"Only draft orders can be deleted");

            var materialUsed = order.MaterialsUsed.ToList();
            foreach (var item in materialUsed)
            {
                await _materialUsedRepository.DeleteAsync(item);
            }

            await _orderRepository.DeleteAsync(order);
            return new Response<int>(order.Id, "Order deleted successfully");
        }
    }

}