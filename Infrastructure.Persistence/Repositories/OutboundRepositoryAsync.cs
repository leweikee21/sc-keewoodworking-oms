using Application.Features.Order.Queries;
using Application.Features.Outbound.Queries;
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
    public class OutboundRepositoryAsync : GenericRepositoryAsync<Outbound>, IOutboundRepositoryAsync
    {
        private readonly DbSet<Outbound> _outbound;

        public OutboundRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _outbound = dbContext.Set<Outbound>();
        }

        public override async Task<Outbound> GetByIdAsync(int id)
        {
            return await _outbound
                            .Include(x => x.Inventory)
                            .Include(y => y.Inbound)
                            .FirstOrDefaultAsync(z => z.Id == id);
        }

        public async Task<PagedResponse<List<Outbound>>> GetAllOutboundAsync(GetAllOutboundParameter filter)
        {
            var query = _outbound
                            .Include(x => x.Inventory)
                            .Include(y => y.Inbound)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.ToLower();
                if (searchTerm.StartsWith("opo"))
                {
                    if (int.TryParse(searchTerm.Substring(3), out int Id))
                    {
                        query = query.Where(u => u.OrderId == Id);
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

            var outbounds = await query
                .OrderByDescending(x => x.Created)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResponse<List<Outbound>>(outbounds, filter.PageNumber, filter.PageSize, totalCount);
        }
    }
}