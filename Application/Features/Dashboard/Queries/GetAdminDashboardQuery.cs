using Application.DTOs.Dashboard;
using Application.DTOs.Inventory;
using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Dashboard.Queries
{
    public class GetAdminDashboardQuery : IRequest<object>
    {

    }

    public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, object>
    {
        private readonly IOrderRepositoryAsync _orderRepo;
        private readonly IInventoryRepositoryAsync _inventoryRepo;
        private readonly IAcquisitionRepositoryAsync _acquisitionRepo;
        public GetAdminDashboardQueryHandler(IOrderRepositoryAsync orderRepo, IInventoryRepositoryAsync inventoryRepo, IAcquisitionRepositoryAsync acquisitionRepo)
        {
            _orderRepo = orderRepo;
            _inventoryRepo = inventoryRepo;
            _acquisitionRepo = acquisitionRepo;
        }

        public async Task<object> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            var totalOrders = await _orderRepo.CountAllAsync();
            var totalRevenue = await _orderRepo.GetTotalRevenueAsync();

            var topCategories = await _orderRepo.GetTopOrdersByCategoryAsync();
            var monthlyTrend = await _orderRepo.GetMonthlyOrderTrendAsync();

            var response = new DashboardResponse
            {
                SummaryCards = new List<DashboardCard>
                {
                    new DashboardCard { Title = "Total Orders", Value = totalOrders },
                    new DashboardCard { Title = "Total Revenue (RM)", Value = (int)totalRevenue },
                },
                Chart = new ChartData
                {
                    Labels = topCategories.Select(x => x.Category).ToList(),
                    Values = topCategories.Select(x => x.TotalOrders).ToList(),
                    ChartType = "pie"
                },
                AdditionalCharts = new List<ChartData>
                {
                    new ChartData
                    {
                        Labels = monthlyTrend.Select(x => x.Month).ToList(),
                        Values = monthlyTrend.Select(x => x.TotalOrders).ToList(),
                        ChartType = "line"
                    }
                }
            };

            return response;
        }
    }

}