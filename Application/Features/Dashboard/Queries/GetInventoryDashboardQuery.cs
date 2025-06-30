using Application.DTOs.Dashboard;
using Application.DTOs.Inventory;
using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Dashboard.Queries
{
    public class GetInventoryDashboardQuery : IRequest<object>
    {

    }

    public class GetInventoryDashboardQueryHandler : IRequestHandler<GetInventoryDashboardQuery, object>
    {
        private readonly IInventoryRepositoryAsync _inventoryRepo;
        private readonly IAcquisitionRepositoryAsync _acquisitionRepo;
        public GetInventoryDashboardQueryHandler(IInventoryRepositoryAsync inventoryRepo, IAcquisitionRepositoryAsync acquisitionRepo)
        {
            _inventoryRepo = inventoryRepo;
            _acquisitionRepo = acquisitionRepo;
        }

        public async Task<object> Handle(GetInventoryDashboardQuery request, CancellationToken cancellationToken)
        {
            var totalItems = await _inventoryRepo.CountAllAsync();
            var aboveMinimum = await _inventoryRepo.CountAboveMinimumAsync();
            var belowMinimum = await _inventoryRepo.CountBelowMinimumAsync();
            var atMinimum = await _inventoryRepo.CountAtMinimumAsync();

            var totalAcq = await _acquisitionRepo.CountAllAsync();
            var draftAcq = await _acquisitionRepo.CountByStatusAsync("Draft");
            var createdAcq = await _acquisitionRepo.CountByStatusAsync("Created");
            var pendingAcq = draftAcq + createdAcq;
            var orderedAcq = await _acquisitionRepo.CountByStatusAsync("Sent");
            var receivedAcq = await _acquisitionRepo.CountByStatusAsync("Received");

            var response = new DashboardResponse
            {
                SummaryCards = new List<DashboardCard>
                {
                    new DashboardCard { Title = "Total Inventories", Value = totalItems },
                    new DashboardCard { Title = "Above Minimum", Value = aboveMinimum },
                    new DashboardCard { Title = "Below Minimum", Value = belowMinimum },
                    new DashboardCard { Title = "At Minimum", Value = atMinimum },
                    new DashboardCard { Title = "Total Acquisitions", Value = totalAcq },
                    new DashboardCard { Title = "Pending", Value = pendingAcq },
                    new DashboardCard { Title = "Ordered", Value = orderedAcq },
                    new DashboardCard { Title = "Received", Value = receivedAcq }
                },
                Chart = new ChartData
                {
                    Labels = new List<string> { "Materials", "Consumables", "Hardware" },
                    Values = await _inventoryRepo.GetStockByCategoryAsync(),
                    ChartType = "bar"
                },
                Notifications = await _inventoryRepo.GetInventoryNotifications()
            };

            return response;
        }
    }

}