using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Order;
using Application.Features.Order.Queries;
using Application.Wrappers;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IOrderRepositoryAsync : IGenericRepositoryAsync<Order>
    {
        Task<int> CountAllAsync();
        Task<int> CountByStatusAsync(string status);
        Task<List<string>> GetOrderNotifications();
        Task<decimal> GetTotalRevenueAsync();
        Task<List<OrderCategoryCountDto>> GetTopOrdersByCategoryAsync();
        Task<List<OrderMonthlyTrendDto>> GetMonthlyOrderTrendAsync();
        new Task<Order> GetByIdAsync(int id);
        Task<PagedResponse<List<Order>>> GetAllOrderAsync(GetAllOrderParameter filter);
        Task<List<Order>> GetByStatusAsync(string status);
        Task<string> SavePdfAsync(int orderId, byte[] pdf);
    }
}
