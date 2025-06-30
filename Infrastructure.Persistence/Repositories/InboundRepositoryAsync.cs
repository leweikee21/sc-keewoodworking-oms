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
using Application.Features.Inbound.Queries;
using System.Linq;

namespace Infrastructure.Persistence.Repositories
{
    public class InboundRepositoryAsync : GenericRepositoryAsync<Inbound>, IInboundRepositoryAsync
    {
        private readonly DbSet<Inbound> _inbound;

        public InboundRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _inbound = dbContext.Set<Inbound>();
        }

        public override async Task<Inbound> GetByIdAsync(int id)
        {
            return await _inbound
                            .Include(x => x.Inventory)
                            .FirstOrDefaultAsync(y => y.Id == id);
        }

        public async Task<PagedResponse<List<Inbound>>> GetAllInboundAsync(GetAllInboundParameter filter)
        {
            var query = _inbound
                            .Include(i => i.Inventory)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.ToLower();
                if (searchTerm.StartsWith("ac"))
                {
                    if (int.TryParse(searchTerm.Substring(2), out int acqId))
                    {
                        query = query.Where(u => u.AcquisitionId == acqId);
                    }
                }
                else
                {
                    query = query.Where(x => 
                        x.Inventory.Code.ToLower().Contains(searchTerm) ||
                        x.Inventory.Name.ToLower().Contains(searchTerm));
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Category) && filter.Category.ToLower() != "all")
            {
                var category = filter.Category.ToLower();
                query = query.Where(x => x.Inventory.Category.ToLower() == category);
            }

            var totalCount = await query.CountAsync();

            var inbound = await query
                .OrderByDescending(x => x.Created)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResponse<List<Inbound>>(inbound, filter.PageNumber, filter.PageSize, totalCount);
        }

        public async Task<List<Inbound>> GetByInventoryIdAsync(int inventoryId)
        {
            return await _inbound
                            .Include(x => x.Inventory)
                            .Where(y => y.InventoryId == inventoryId)
                            .OrderBy(z => z.Created)
                            .ToListAsync();
        }
    }
}