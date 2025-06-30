using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Order.Commands
{
    public class CompleteOrderCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUserService;
        private readonly IOrderRepositoryAsync _orderRepository;
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IMaterialUsedRepositoryAsync _materialUsedRepository;
        private readonly IInboundRepositoryAsync _inboundRepository;
        private readonly IOutboundRepositoryAsync _outboundRepository;
        private readonly IPdfService _pdfService;
        private readonly IMapper _mapper;

        public CompleteOrderCommandHandler(IAuthenticatedUserService authenticatedUserService, IOrderRepositoryAsync orderRepository,
                                           IInventoryRepositoryAsync inventoryRepository, IMaterialUsedRepositoryAsync materialUsedRepository, IInboundRepositoryAsync inboundRepository,
                                           IOutboundRepositoryAsync outboundRepository,
                                           IPdfService pdfService, IMapper mapper)
        {
            _authenticatedUserService = authenticatedUserService;
            _orderRepository = orderRepository;
            _inventoryRepository = inventoryRepository;
            _materialUsedRepository = materialUsedRepository;
            _inboundRepository = inboundRepository;
            _outboundRepository = outboundRepository;
            _pdfService = pdfService;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CompleteOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(command.Id);

            if (order == null)
                throw new ApiException($"Order not found.");

            if (order.Status != "In Progress")
                throw new ApiException($"Only orders in 'In Progress' status can be completed.");

            var materialUsed = order.MaterialsUsed.ToList();
            foreach (var item in materialUsed)
            {
                var inventory = item.Inventory;
                var inbounds = await _inboundRepository.GetByInventoryIdAsync(item.InventoryId);

                int remainingQty = item.Quantity;
                decimal totalCost = 0;

                foreach (var inbound in inbounds)
                {
                    if (remainingQty == 0) break;

                    int usedQty = Math.Min(remainingQty, inbound.RemainingQuantity);

                    //Skip if nothing is used from this batch
                    if (usedQty == 0) continue;

                    inbound.RemainingQuantity -= usedQty;
                    await _inboundRepository.UpdateAsync(inbound);

                    totalCost += usedQty * inbound.UnitPrice;
                    remainingQty -= usedQty;

                    var outbound = new Domain.Entities.Outbound
                    {
                        InventoryId = inventory.Id,
                        OrderId = order.Id,
                        InboundId = inbound.Id,
                        Quantity = usedQty,
                        TotalPrice = usedQty * inbound.UnitPrice,
                        Remark = $"OPO{order.Id:D6}",
                        Created = DateTime.UtcNow
                    };
                    await _outboundRepository.AddAsync(outbound);
                }

                item.UnitPrice = totalCost / item.Quantity;
                item.TotalPrice = totalCost;

                inventory.ReservedQty -= item.Quantity;
                inventory.TotalQty -= item.Quantity;
                inventory.AvailableQty = inventory.TotalQty - inventory.ReservedQty;
                inventory.LastOutDate = DateTime.UtcNow;

                await _inventoryRepository.UpdateAsync(inventory);
                await _materialUsedRepository.UpdateAsync(item);
            }

            order.Status = "Completed";
            order.MaterialCost = materialUsed.Sum(x => x.TotalPrice);
            order.TotalRevenue = order.TotalPrice - order.MaterialCost;
            order.LastModified = DateTime.UtcNow;
            order.LastModifiedBy = _authenticatedUserService.UserId;

            await _orderRepository.UpdateAsync(order);
            return new Response<int>(order.Id, "Order completed and outbounds(s) recorded.");
        }
    }

}