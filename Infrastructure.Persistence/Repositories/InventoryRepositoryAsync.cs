using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Wrappers;
using Application.Features.Inventory.Queries;
using System.Linq;

namespace Infrastructure.Persistence.Repositories
{
    public class InventoryRepositoryAsync : GenericRepositoryAsync<Inventory>, IInventoryRepositoryAsync
    {
        private readonly DbSet<Inventory> _inventory;

        public InventoryRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _inventory = dbContext.Set<Inventory>();
        }

        public async Task<int> CountAllAsync()
        {
            return await _inventory.Where(x => x.IsDeleted == false).CountAsync();
        }

        public async Task<int> CountAboveMinimumAsync()
        {
            return await _inventory.Where(x => x.AvailableQty > x.MinQty && x.IsDeleted == false).CountAsync();
        }

        public async Task<int> CountBelowMinimumAsync()
        {
            return await _inventory.Where(x => x.AvailableQty < x.MinQty && x.IsDeleted == false).CountAsync();
        }

        public async Task<int> CountAtMinimumAsync()
        {
            return await _inventory.Where(x => x.AvailableQty == x.MinQty && x.IsDeleted == false).CountAsync();
        }

        public async Task<List<int>> GetStockByCategoryAsync()
        {
            var materials = await _inventory.Where(x => x.Category == "Materials" && x.IsDeleted == false).CountAsync();
            var consumables = await _inventory.Where(x => x.Category == "Consumables" && x.IsDeleted == false).CountAsync();
            var hardware = await _inventory.Where(x => x.Category == "Hardware" && x.IsDeleted == false).CountAsync();

            var countList = new List<int> { materials, consumables, hardware };

            return countList;
        }

        public async Task<List<string>> GetInventoryNotifications()
        {
            var belowMin = await _inventory.Where(x => x.AvailableQty < x.MinQty && x.IsDeleted == false).ToListAsync();
            var list = new List<string>();

            foreach (var item in belowMin)
            {
                list.Add($"{item.Name} is below minimum");
            }

            return list;
        }

        public override async Task<Inventory> GetByIdAsync(int id)
        {
            return await _inventory
                            .Include(x => x.Supplier)
                            .FirstOrDefaultAsync(y => y.Id == id);
        }

        public async Task<PagedResponse<List<Inventory>>> GetAllInventoryAsync(GetAllInventoryParameter filter)
        {
            var query = _inventory
                            .Include(x => x.Supplier)
                            .AsQueryable()
                            .Where(i => !i.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(searchTerm) ||
                    x.Code.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(filter.StockLevel) && filter.StockLevel.ToLower() != "all")
            {
                switch (filter.StockLevel.ToLower())
                {
                    case "below min":
                        query = query.Where(x => x.AvailableQty < x.MinQty);
                        break;
                    case "equal min":
                        query = query.Where(x => x.AvailableQty == x.MinQty);
                        break;
                    case "above min":
                        query = query.Where(x => x.AvailableQty > x.MinQty);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Category) && filter.Category.ToLower() != "all")
            {
                var category = filter.Category.ToLower();
                query = query.Where(x => x.Category.ToLower() == category);
            }

            var totalCount = await query.CountAsync();

            var inventory = await query
                .OrderBy(x => x.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResponse<List<Inventory>>(inventory, filter.PageNumber, filter.PageSize, totalCount);
        }

        public async Task<List<Inventory>> GetByCategoryAsync(string category)
        {
            return await _inventory
                            .Include(x => x.Supplier)
                            .Where(y => y.Category == category && y.IsDeleted == false)
                            .OrderBy(z => z.Name)
                            .ToListAsync();
        }

        public async Task<List<Inventory>> GetBySupplierAsync(int supplierId)
        {
            return await _inventory
                            .Include(x => x.Supplier)
                            .Where(y => y.SupplierId == supplierId && y.IsDeleted == false)
                            .OrderBy(z => z.Name)
                            .ToListAsync();
        }

        public async Task<Inventory> GetByCodeAsync(string code)
        {
            return await _inventory
                            .Include(x => x.Supplier)
                            .FirstOrDefaultAsync(i => i.Code.ToLower() == code.ToLower());
        }

        public async Task<List<Inventory>> GetByNoSupplierAsync()
        {
            return await _inventory
                            .Where(x => x.SupplierId == null && x.IsDeleted == false)
                            .OrderBy(y => y.Name)
                            .ToListAsync();
        }
    }
}