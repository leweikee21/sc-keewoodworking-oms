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
using Application.Features.Supplier.Queries;
using System.Linq;

namespace Infrastructure.Persistence.Repositories
{
    public class SupplierRepositoryAsync : GenericRepositoryAsync<Supplier>, ISupplierRepositoryAsync
    {
        private readonly DbSet<Supplier> _suppliers;

        public SupplierRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _suppliers = dbContext.Set<Supplier>();
        }

        public override async Task<Supplier> GetByIdAsync(int id)
        {
            return await _suppliers
                            .Include(x => x.Inventories)
                            .FirstOrDefaultAsync(y => y.Id == id);
        }

        public async Task<Supplier> GetByEmailAsync(string email)
        {
            return await _suppliers
                            .Include(x => x.Inventories)
                            .FirstOrDefaultAsync(y => y.Email.ToLower() == email.ToLower());
        }

        public async Task<PagedResponse<List<Supplier>>> GetAllSupplierAsync(GetAllSupplierParameter filter)
        {
            var query = _suppliers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.ToLower();
                query = query.Where(x => 
                    x.Name.ToLower().Contains(searchTerm) ||
                    x.Email.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var suppliers = await query
                .OrderBy(p => p.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResponse<List<Supplier>>(suppliers, filter.PageNumber, filter.PageSize, totalCount);
        }
    }
}