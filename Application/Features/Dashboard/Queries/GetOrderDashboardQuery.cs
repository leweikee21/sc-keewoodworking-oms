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
    public class GetOrderDashboardQuery : IRequest<object>
    {

    }

    public class GetOrderDashboardQueryHandler : IRequestHandler<GetOrderDashboardQuery, object>
    {
        private readonly IOrderRepositoryAsync _orderRepo;
        public GetOrderDashboardQueryHandler(IOrderRepositoryAsync orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<object> Handle(GetOrderDashboardQuery request, CancellationToken cancellationToken)
        {
            var totalOrders = await _orderRepo.CountAllAsync();
            var pendingOrders = await _orderRepo.CountByStatusAsync("Pending");
            var createdOrders = await _orderRepo.CountByStatusAsync("Created");
            var inProgress = await _orderRepo.CountByStatusAsync("In Progress");
            var completed = await _orderRepo.CountByStatusAsync("Completed");

            var response = new DashboardResponse
            {
                SummaryCards = new List<DashboardCard>
                {
                    new DashboardCard { Title = "Total Orders", Value = totalOrders },
                    new DashboardCard { Title = "Pending", Value = pendingOrders },
                    new DashboardCard { Title = "Queuing", Value = createdOrders },
                    new DashboardCard { Title = "In Progress", Value = inProgress },
                    new DashboardCard { Title = "Completed", Value = completed }
                },
                Chart = new ChartData
                {
                    Labels = new List<string> { "Pending", "Created", "In Progress", "Completed", "Invoiced", "Delivered" },
                    Values = new List<int>
                    {
                        pendingOrders,
                        createdOrders,
                        inProgress,
                        completed,
                        await _orderRepo.CountByStatusAsync("Invoiced"),
                        await _orderRepo.CountByStatusAsync("Delivered")
                    },
                    ChartType = "pie"
                },
                Notifications = await _orderRepo.GetOrderNotifications()
            };

            return response;
        }
    }

}