using Application.DTOs.Email;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Shared.Services
{
    public class OrderStatusUpdateBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<OrderStatusUpdateBackgroundService> _logger;

        public OrderStatusUpdateBackgroundService(IServiceProvider services, ILogger<OrderStatusUpdateBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderStatusUpdateBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    try
                    {
                        var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderRepositoryAsync>();
                        var inventoryRepo = scope.ServiceProvider.GetRequiredService<IInventoryRepositoryAsync>();
                        var materialRepo = scope.ServiceProvider.GetRequiredService<IMaterialUsedRepositoryAsync>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                        var userService = scope.ServiceProvider.GetService<IUserService>();

                        var pendingOrders = await orderRepo.GetByStatusAsync("Pending");

                        foreach (var order in pendingOrders)
                        {
                            //var materials = await materialRepo.GetByOrderIdAsync(order.Id);
                            var materials = order.MaterialsUsed.ToList();

                            bool allAvailable = true;

                            foreach (var item in materials)
                            {
                                //var inventory = await inventoryRepo.GetByIdAsync(item.InventoryId);
                                var inventory = item.Inventory;
                                if (inventory == null || inventory.AvailableQty < item.Quantity)
                                {
                                    allAvailable = false;
                                    break;
                                }
                            }

                            if (allAvailable)
                            {
                                foreach (var item in materials)
                                {
                                    //var inventory = await inventoryRepo.GetByIdAsync(item.InventoryId);
                                    var inventory = item.Inventory;
                                    inventory.ReservedQty += item.Quantity;
                                    inventory.AvailableQty = inventory.TotalQty - inventory.ReservedQty;
                                    await inventoryRepo.UpdateAsync(inventory);
                                }

                                order.Status = "Created";
                                order.Remark = null;
                                order.LastModified = DateTime.UtcNow;
                                await orderRepo.UpdateAsync(order);

                                var users1 = await userService.GetUsersByRoleAsync("Order");
                                var users2 = await userService.GetUsersByRoleAsync("Admin");
                                var emails = users1.Select(x => x.Data.Email)
                                                    .Concat(users2.Select(x => x.Data.Email))
                                                    .ToList();

                                var email = new EmailRequest
                                {
                                    To = emails,
                                    Subject = $"[Notification] Order #OPO{order.Id:D6} Successfully Created",
                                    Body = $@"
                                            Dear Order Management Team,<br/><br/>
                                            We are pleased to inform you that <strong>Order #OPO{order.Id:D6}</strong> has been successfully created.<br/>     
                                            This action was triggered as all required materials are now available in inventory.<br/><br/>
                                            Please login to the system to review and proceed with the necessary processing steps.<br/><br/>
                                            Regards, <br/>
                                            Kee Woodworking System"
                                };
                                await emailService.SendAsync(email);

                                _logger.LogInformation($"Order #OPO{order.Id:D6} status auto-updated to 'Created'.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred in OrderStatusUpdateBackgroundService");
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Runs every 5 minutes
            }
        }
    }
}
