using Application.DTOs.Order;
using Application.Features.Order.Queries;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Infrastructure.Persistence.Repositories
{
    public class OrderRepositoryAsync : GenericRepositoryAsync<Order>, IOrderRepositoryAsync
    {
        private readonly DbSet<Order> _order;

        public OrderRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _order = dbContext.Set<Order>();
        }

        public async Task<int> CountAllAsync()
        {
            return await _order.Where(x=> x.Status != "Draft").CountAsync();
        }

        public async Task<int> CountByStatusAsync(string status)
        { 
            return await _order.Where(x => x.Status == status).CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _order.SumAsync(x => x.TotalRevenue);
        }

        public async Task<List<string>> GetOrderNotifications()
        {
            var sevenDaysFromNow = DateTime.UtcNow.AddDays(7);
            var nearDue = await _order
                .Where(x => x.RequiredDelDate <= sevenDaysFromNow && x.Status != "Delivered")
                .ToListAsync();

            var list = new List<string>();
            foreach (var x in nearDue)
            {

                list.Add($"{x.Id.ToString().PadLeft(6, '0')} left less than 7 days to deliver");
            }
            return list;
        }

        public async Task<List<OrderCategoryCountDto>> GetTopOrdersByCategoryAsync()
        {
            return await _order
                .GroupBy(x => x.ModelCategory)
                .Select(g => new OrderCategoryCountDto
                {
                    Category = g.Key,
                    TotalOrders = g.Count()
                })
                .OrderByDescending(x => x.TotalOrders)
                .Take(5)
                .ToListAsync();
        }

        public async Task<List<OrderMonthlyTrendDto>> GetMonthlyOrderTrendAsync()
        {
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-5);

            // Fix: Ensure UTC DateTime Kind
            var startDate = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var rawData = await _order
                .Where(x => x.ReceivedDate >= startDate)
                .GroupBy(x => new { x.ReceivedDate.Year, x.ReceivedDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalOrders = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            return rawData.Select(x => new OrderMonthlyTrendDto
            {
                Month = $"{x.Year}-{x.Month.ToString().PadLeft(2, '0')}",
                TotalOrders = x.TotalOrders
            }).ToList();
        }

        public override async Task<Order> GetByIdAsync(int id)
        {
            return await _order
                            .Include(x => x.MaterialsUsed)
                            .ThenInclude(y => y.Inventory)
                            .FirstOrDefaultAsync(z => z.Id == id);
        }

        public async Task<PagedResponse<List<Order>>> GetAllOrderAsync(GetAllOrderParameter filter)
        {
            var query = _order
                            .Include(x => x.MaterialsUsed)
                            .ThenInclude(y => y.Inventory)
                            .AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.ToLower();
                if (searchTerm.StartsWith("opo"))
                {
                    if (int.TryParse(searchTerm.Substring(3), out int Id))
                    {
                        query = query.Where(u => u.Id == Id);
                    }
                }
                else
                {
                    query = query.Where(x =>
                        x.ModelCode.ToLower().Contains(searchTerm) ||
                        x.ModelCategory.ToLower().Contains(searchTerm));
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Status) && filter.Status.ToLower() != "all")
            {
                var status = filter.Status.ToLower();
                query = query.Where(x => x.Status.ToLower() == status);
            }

            var totalCount = await query.CountAsync();

            var order = await query
                .OrderByDescending(x => x.Created)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResponse<List<Order>>(order, filter.PageNumber, filter.PageSize, totalCount);
        }

        public async Task<List<Order>> GetByStatusAsync(string status)
        {
            return await _order
                            .Include(x => x.MaterialsUsed)
                            .ThenInclude(y => y.Inventory)
                            .Where(z => z.Status == status)
                            .ToListAsync();
        }

        public async Task<string> SavePdfAsync(int orderId, byte[] pdfBytes)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "orders");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"OPO{orderId:D6}_{DateTime.UtcNow:yyyy-MM-dd}.pdf";
            var fullPath = Path.Combine(folderPath, fileName);

            await System.IO.File.WriteAllBytesAsync(fullPath, pdfBytes);

            return $"/pdfs/orders/{fileName}";
        }
    }
}