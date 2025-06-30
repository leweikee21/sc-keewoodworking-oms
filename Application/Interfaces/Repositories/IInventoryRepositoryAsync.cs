using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Inventory.Queries;
using Application.Wrappers;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IInventoryRepositoryAsync : IGenericRepositoryAsync<Inventory>
    {
        Task<int> CountAllAsync();
        Task<int> CountAboveMinimumAsync();
        Task<int> CountBelowMinimumAsync();
        Task<int> CountAtMinimumAsync();
        Task<List<int>> GetStockByCategoryAsync();
        Task<List<string>> GetInventoryNotifications();
        new Task<Inventory> GetByIdAsync(int id);
        Task<PagedResponse<List<Inventory>>> GetAllInventoryAsync(GetAllInventoryParameter filter);
        Task<List<Inventory>> GetByCategoryAsync(string category);
        Task<List<Inventory>> GetBySupplierAsync(int supplierId);
        Task<Inventory> GetByCodeAsync(string code);
        Task<List<Inventory>> GetByNoSupplierAsync();
    }
}
