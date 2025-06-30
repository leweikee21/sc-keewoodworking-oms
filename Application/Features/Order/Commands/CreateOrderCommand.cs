using Application.DTOs.Account;
using Application.DTOs.Email;
using Application.DTOs.Orders;
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Order.Commands 
{
    public partial class CreateOrderCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string ModelCode { get; set; }
        public string ModelCategory { get; set; }
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

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Response<int>>
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

        public CreateOrderCommandHandler(IAuthenticatedUserService authenticateUser, IOrderRepositoryAsync orderRepository,
                                         IInventoryRepositoryAsync inventoryRepository, IMaterialUsedRepositoryAsync materialUsedRepository,
                                         IAcquisitionRepositoryAsync acquisitionRepository, IAcquisitionItemRepositoryAsync acquisitionItemRepository,
                                         ISupplierRepositoryAsync supplierRepository,
                                         IEmailService emailService, IUserService userService, IMapper mapper)
        {
            _authenticatedUser = authenticateUser;
            _orderRepository = orderRepository;
            _inventoryRepository = inventoryRepository;
            _materialUsedRepository = materialUsedRepository;
            _acquisitionRepository = acquisitionRepository;
            _acquisitionItemRepository = acquisitionItemRepository;
            _supplierRepository = supplierRepository;
            _emailService = emailService;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(command.Id);
            if (existingOrder != null)
            {
                throw new ValidationException(new List<ValidationFailure>
                {
                    new ValidationFailure("", "Order already exists. Please use a different ID.")
                });
            }

            var order = new Domain.Entities.Order
            {
                Id = command.Id,
                Status = command.IsDraft ? "Draft" : "Created",
                Model = command.Model,
                ModelCode = command.ModelCode,
                ModelCategory = command.ModelCategory,
                Quantity = command.Quantity,
                UnitPrice = command.UnitPrice,
                UnitWages = command.UnitWages,
                OtherCost = command.OtherCost,
                HardwareCost = command.HardwareCost,
                TotalPrice = command.TotalPrice,
                RequiredDelDate = DateTime.SpecifyKind(command.RequiredDelDate, DateTimeKind.Utc),
                ReceivedDate = DateTime.SpecifyKind(command.ReceivedDate, DateTimeKind.Utc),
                Created = DateTime.UtcNow,
                CreatedBy = _authenticatedUser.UserId
            };

            await _orderRepository.AddAsync(order);

            bool insufficient = false;
            var acquisitionGroup = new Dictionary<int, List<AcquisitionItem>>();

            // STEP 1: Always insert MaterialUsed, regardless of draft or not
            foreach (var item in command.MaterialUsed)
            {
                await _materialUsedRepository.AddAsync(new MaterialUsed
                {
                    OrderId = order.Id,
                    InventoryId = item.InventoryId,
                    Quantity = item.Quantity,
                    UnitPrice = 0, // To be updated later after completion
                    TotalPrice = 0
                });
            }

            // STEP 2: For non-drafts, handle inventory reservation and shortage check
            if (!command.IsDraft)
            {
                foreach (var item in command.MaterialUsed)
                {
                    var inventory = await _inventoryRepository.GetByIdAsync(item.InventoryId);

                    if (inventory.AvailableQty < item.Quantity)
                    {
                        insufficient = true;

                        if (!acquisitionGroup.ContainsKey((int)inventory.SupplierId))
                            acquisitionGroup[(int)inventory.SupplierId] = new List<AcquisitionItem>();

                        acquisitionGroup[(int)inventory.SupplierId].Add(new AcquisitionItem
                        {
                            InventoryId = inventory.Id,
                            Quantity = item.Quantity,
                        });
                    }
                    else
                    {
                        inventory.ReservedQty += item.Quantity;
                        inventory.AvailableQty = inventory.TotalQty - inventory.ReservedQty;
                        await _inventoryRepository.UpdateAsync(inventory);
                    }
                }

                // Update order status and remark based on shortage
                order.Status = insufficient ? "Pending" : "Created";
                order.Remark = insufficient ? "Pending: Auto acquisition created for insufficient stock." : null;

                // STEP 3: Auto-create acquisitions if insufficient
                if (insufficient)
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
                            Subject = $"[Auto-Generated] Acquisition #AC{acquisition.Id: D4} - {supplier.Name}",
                            Body = $@"
                                    Dear Inventory Team,<br/><br/>
                                    An acquisition <b>(#AC{acquisition.Id: D4})</b> has been generated due to insufficient stock for Order <b>#OPO{order.Id:D6}</b>.<br/>
                                    Supplier: <b>{supplier.Name}</b><br/>
                                    Total Items: <b>{acquisition.TotalItems}</b><br/><br/>
                                    Please login to the system to review and process.<br/><br/>
                                    Regards,<br/>Kee Woodworking System"
                        };

                        await _emailService.SendAsync(email);
                    }
                }

                await _orderRepository.UpdateAsync(order);
            }

            return new Response<int>(order.Id, "Order created successfully");
        }
    }
}