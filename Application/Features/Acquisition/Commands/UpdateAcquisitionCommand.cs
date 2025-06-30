using Application.DTOs.Acquisition;
using Application.DTOs.User;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using System.Linq;

namespace Application.Features.Acquisition.Commands
{
    public class UpdateAcquisitionCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public bool IsDraft { get; set; }
        public int SupplierId { get; set; }
        public List<AcquisitionItemDto> Items { get; set; }
    }

    public class UpdateAcquisitionCommandHandler : IRequestHandler<UpdateAcquisitionCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IAcquisitionRepositoryAsync _acquisitionRepository;
        private readonly IAcquisitionItemRepositoryAsync _acquisitionItemRepository;
        private readonly IMapper _mapper;

        public UpdateAcquisitionCommandHandler(IAuthenticatedUserService authenticatedUser, IAcquisitionRepositoryAsync acquisitionRepository, IAcquisitionItemRepositoryAsync acquisitionItemRepository, IMapper mapper)
        {
            _authenticatedUser = authenticatedUser;
            _acquisitionRepository = acquisitionRepository;
            _acquisitionItemRepository = acquisitionItemRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateAcquisitionCommand command, CancellationToken cancellationToken)
        {
            var acquisition = await _acquisitionRepository.GetByIdAsync(command.Id);

            if (acquisition.Status != "Draft")
            {
                throw new ApiException($"Only draft acquisition can be updated.");
            }

            bool supplierChanged = acquisition.SupplierId != command.SupplierId;

            if (supplierChanged)
            {
                foreach (var item in acquisition.Items.ToList())
                {
                    await _acquisitionItemRepository.DeleteAsync(item);
                }

                acquisition.Items.Clear();

                foreach (var item in command.Items)
                {
                    var newAcquisitionItem = new AcquisitionItem
                    {
                        AcquisitionId = acquisition.Id,
                        InventoryId = item.Inventory.Id,
                        Quantity = item.Quantity
                    };                
                    
                    await _acquisitionItemRepository.AddAsync(newAcquisitionItem);
                    acquisition.Items.Add(newAcquisitionItem);
                }
            }
            else
            {
                var exisitingItems = acquisition.Items.ToList();

                foreach (var newItem in command.Items)
                {
                    var existingItem = exisitingItems.FirstOrDefault(i => i.InventoryId == newItem.InventoryId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity = newItem.Quantity;
                        await _acquisitionItemRepository.UpdateAsync(existingItem);
                    }
                    else
                    {
                        var newAcquisitionItem = new AcquisitionItem
                        {
                            AcquisitionId = acquisition.Id,
                            InventoryId = newItem.Inventory.Id,
                            Quantity = newItem.Quantity
                        };
                        await _acquisitionItemRepository.AddAsync(newAcquisitionItem);
                        acquisition.Items.Add(newAcquisitionItem);
                    }
                }
                
                var newInventoryIds = command.Items.Select(i => i.InventoryId).ToHashSet();
                foreach (var oldItem in exisitingItems)
                {
                    if (!newInventoryIds.Contains(oldItem.InventoryId))
                    {
                        await _acquisitionItemRepository.DeleteAsync(oldItem);
                        acquisition.Items.Remove(oldItem);
                    }
                }
            }

            acquisition.Status = command.IsDraft ? "Draft" : "Created";
            acquisition.SupplierId = command.SupplierId;
            acquisition.CreatedBy = acquisition.CreatedBy == null ? _authenticatedUser.UserId : acquisition.CreatedBy;
            acquisition.LastModified = DateTime.UtcNow;
            acquisition.LastModifiedBy = _authenticatedUser.UserId;
            acquisition.TotalItems = command.Items.Count;

            await _acquisitionRepository.UpdateAsync(acquisition);

            return new Response<int>(acquisition.Id, "Acquisition updated successfully");
        }
    }
}