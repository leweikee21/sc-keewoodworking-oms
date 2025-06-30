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
    public partial class CreateOutboundBatchCommand : IRequest<Response<int>>
    {
        public List<OutboundCreateDto> Items { get; set; }
    }

    public class OutboundCreateDto
    {
        public int InventoryId { get; set; }
        public int Quantity { get; set; }
    }
    public class CreateOutboundBatchCommandHandler : IRequestHandler<CreateOutboundBatchCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IInboundRepositoryAsync _inboundRepository;
        private readonly IOutboundRepositoryAsync _outboundRepository;
        private readonly IMapper _mapper;

        public CreateOutboundBatchCommandHandler(IAuthenticatedUserService authenticateUser, IInventoryRepositoryAsync inventoryRepository,
                                         IInboundRepositoryAsync inboundRepository, IOutboundRepositoryAsync outboundRepository,
                                         IMapper mapper)
        {
            _authenticatedUser = authenticateUser;
            _inventoryRepository = inventoryRepository;
            _inboundRepository = inboundRepository;
            _outboundRepository = outboundRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateOutboundBatchCommand command, CancellationToken cancellationToken)
        {
            foreach (var item in command.Items)
            {
                var inventory = await _inventoryRepository.GetByIdAsync(item.InventoryId);

                if (inventory == null)
                    throw new ApiException("Inventory not found.");

                if (inventory.AvailableQty < item.Quantity)
                    throw new ApiException("Insufficient stock available.");

                int remainingQty = item.Quantity;
                var inbounds = await _inboundRepository.GetByInventoryIdAsync(item.InventoryId);

                foreach (var inbound in inbounds)
                {
                    if (remainingQty == 0) break;
                    if (inbound.RemainingQuantity == 0) continue;

                    int usedQty = Math.Min(remainingQty, inbound.RemainingQuantity);

                    inbound.RemainingQuantity -= usedQty;
                    await _inboundRepository.UpdateAsync(inbound);

                    var outbound = new Domain.Entities.Outbound
                    {
                        InventoryId = item.InventoryId,
                        Quantity = usedQty,
                        TotalPrice = usedQty * inbound.UnitPrice,
                        InboundId = inbound.Id,
                        Created = DateTime.UtcNow,
                        CreatedBy = _authenticatedUser.UserId,
                    };

                    await _outboundRepository.AddAsync(outbound);

                    remainingQty -= usedQty;
                }

                inventory.TotalQty -= item.Quantity;
                inventory.AvailableQty = inventory.TotalQty - inventory.ReservedQty;
                inventory.LastOutDate = DateTime.UtcNow;
                await _inventoryRepository.UpdateAsync(inventory);
            }

            return new Response<int>(command.Items.Count, "Outbound record created.");
        }
    }
}