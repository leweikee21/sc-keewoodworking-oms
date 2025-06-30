using Application.DTOs.Email;
using Application.DTOs.Orders;
using Application.DTOs.User;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Order.Commands
{
    public class UpdateOrderCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string ModelCode { get; set; }
        public string ModelCategory { get; set; }
        public string Status { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime RequiredDelDate { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal UnitWages { get; set; }
        public decimal OtherCost { get; set; }
        public decimal HardwareCost { get; set; }
        public bool IsDraft { get; set; }
        public List<MaterialUsedDto> MaterialUsed { get; set; }
    }

    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Response<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser; 
        private readonly IOrderRepositoryAsync _orderRepository;
        private readonly IInventoryRepositoryAsync _inventoryRepository;
        private readonly IMaterialUsedRepositoryAsync _materialUsedRepository;
        private readonly IAcquisitionRepositoryAsync _acquisitionRepository;
        private readonly IAcquisitionItemRepositoryAsync _acquisitionItemRepository;
        private readonly ISupplierRepositoryAsync _supplierRepository;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UpdateOrderCommandHandler(IAuthenticatedUserService authenticateUser, IOrderRepositoryAsync orderRepository,
                                         IInventoryRepositoryAsync inventoryRepository, IMaterialUsedRepositoryAsync materialUsedRepository,
                                         IAcquisitionRepositoryAsync acquisitionRepository, IAcquisitionItemRepositoryAsync acquisitionItemRepositoryAsync,
                                         ISupplierRepositoryAsync supplierRepository,
                                         IEmailService emailService, IUserService userService, IMapper mapper)
        {
            _authenticatedUser = authenticateUser;
            _orderRepository = orderRepository;
            _inventoryRepository = inventoryRepository;
            _materialUsedRepository = materialUsedRepository;
            _acquisitionRepository = acquisitionRepository;
            _acquisitionItemRepository = acquisitionItemRepositoryAsync;
            _supplierRepository = supplierRepository;
            _emailService = emailService;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(command.Id);
            if (order == null)
                throw new ApiException("Order not found.");

            if (order.Status != "Draft")
                throw new ApiException("Only draft orders can be updated.");

            // Update base fields
            order.Model = command.Model;
            order.ModelCode = command.ModelCode;
            order.ModelCategory = command.ModelCategory;
            order.Quantity = command.Quantity;
            order.UnitPrice = command.UnitPrice;
            order.UnitWages = command.UnitWages;
            order.OtherCost = command.OtherCost;
            order.HardwareCost = command.HardwareCost;
            order.TotalPrice = command.TotalPrice;
            order.RequiredDelDate = DateTime.SpecifyKind(command.RequiredDelDate, DateTimeKind.Utc);
            order.ReceivedDate = DateTime.SpecifyKind(command.ReceivedDate, DateTimeKind.Utc);
            order.LastModified = DateTime.UtcNow;
            order.LastModifiedBy = _authenticatedUser.UserId;

            var existingItems = order.MaterialsUsed.ToList();
            var newItemIds = command.MaterialUsed.Select(x => x.InventoryId).ToHashSet();

            bool insufficient = false;
            var acquisitionGroup = new Dictionary<int, List<AcquisitionItem>>();

            // STEP 1: Handle Inventory Reservation / Shortage Detection for Non-Drafts
            if (!command.IsDraft)
            {
                foreach (var newItem in command.MaterialUsed)
                {
                    var inventory = await _inventoryRepository.GetByIdAsync(newItem.InventoryId);

                    if (inventory.AvailableQty < newItem.Quantity)
                    {
                        insufficient = true;

                        if (!acquisitionGroup.ContainsKey((int)inventory.SupplierId))
                            acquisitionGroup[(int)inventory.SupplierId] = new List<AcquisitionItem>();

                        acquisitionGroup[(int)inventory.SupplierId].Add(new AcquisitionItem
                        {
                            InventoryId = inventory.Id,
                            Quantity = newItem.Quantity
                        });
                    }
                    else
                    {
                        inventory.ReservedQty += newItem.Quantity;
                        inventory.AvailableQty = inventory.TotalQty - inventory.ReservedQty;
                        await _inventoryRepository.UpdateAsync(inventory);
                    }
                }
            }

            // STEP 3: Update MaterialUsed table (DB) - Always do this regardless of draft/final
            foreach (var newItem in command.MaterialUsed)
            {
                var existingItem = existingItems.FirstOrDefault(x => x.InventoryId == newItem.InventoryId);
                if (existingItem != null)
                {
                    existingItem.Quantity = newItem.Quantity;
                    await _materialUsedRepository.UpdateAsync(existingItem);
                }
                else
                {
                    await _materialUsedRepository.AddAsync(new MaterialUsed
                    {
                        OrderId = order.Id,
                        InventoryId = newItem.InventoryId,
                        Quantity = newItem.Quantity,
                        UnitPrice = 0,
                        TotalPrice = 0
                    });
                }
            }

            // Delete any old MaterialUsed that are not in the new list
            foreach (var oldItem in existingItems)
            {
                if (!newItemIds.Contains(oldItem.InventoryId))
                {
                    await _materialUsedRepository.DeleteAsync(oldItem);
                }
            }

            // STEP 4: Update Order Status & Remark
            if (command.IsDraft)
            {
                order.Status = "Draft";
                order.Remark = null;
            }
            else
            {
                order.Status = insufficient ? "Pending" : "Created";
                order.Remark = insufficient ? "Pending: Auto acquisition created for insufficient stock." : null;
            }

            // STEP 5: Create Acquisition if Insufficient
            if (!command.IsDraft && insufficient && order.Status == "Pending")
            {
                foreach (var group in acquisitionGroup)
                {
                    var acquisition = new Domain.Entities.Acquisition
                    {
                        Status = "Draft",
                        SupplierId = group.Key,
                        TotalItems = group.Value.Count,
                        Created = DateTime.UtcNow
                    };

                    await _acquisitionRepository.AddAsync(acquisition);

                    foreach (var item in group.Value)
                    {
                        item.AcquisitionId = acquisition.Id;
                        await _acquisitionItemRepository.AddAsync(item);
                    }

                    var supplier = await _supplierRepository.GetByIdAsync(acquisition.SupplierId);
                    var users1 = await _userService.GetUsersByRoleAsync("Inventory");
                    var users2 = await _userService.GetUsersByRoleAsync("Admin");
                    var emails = users1.Select(x => x.Data.Email)
                                       .Concat(users2.Select(x => x.Data.Email))
                                       .ToList();

                    var email = new EmailRequest
                    {
                        To = emails,
                        Subject = $"[Auto-Generated] Acquisition #{acquisition.Id} - {supplier.Name}",
                        Body = $@"
                            Dear Inventory Team,<br/><br/>
                            An acquisition <b>(#{acquisition.Id})</b> has been generated due to insufficient stock for Order <b>OPO{order.Id:D6}</b>.<br/>
                            Supplier: <b>{supplier.Name}</b><br/>
                            Total Items: <b>{acquisition.TotalItems}</b><br/><br/>
                            Please login to the system to review and process.<br/><br/>
                            Regards,<br/>Kee Woodworking System"
                    };

                    await _emailService.SendAsync(email);
                }
            }

            await _orderRepository.UpdateAsync(order);
            return new Response<int>(order.Id, "Order updated successfully");
        }
    }
}