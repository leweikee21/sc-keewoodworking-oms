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
using Application.Features.Acquisition.Queries;
using System.Linq;
using System.IO;

namespace Infrastructure.Persistence.Repositories
{
    public class AcquisitionRepositoryAsync : GenericRepositoryAsync<Acquisition>, IAcquisitionRepositoryAsync
    {
        private readonly DbSet<Acquisition> _acquisition;

        public AcquisitionRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _acquisition = dbContext.Set<Acquisition>();
        }

        public async Task<int> CountAllAsync()
        {
            return await _acquisition.CountAsync();
        }

        public async Task<int> CountByStatusAsync(string status)
        {
            return await _acquisition.Where(x => x.Status == status).CountAsync();
        }

        public override async Task<Acquisition> GetByIdAsync(int id)
        {
            return await _acquisition
                            .Include(s => s.Supplier)
                            .Include(x => x.Items)
                            .ThenInclude(y => y.Inventory)
                            .FirstOrDefaultAsync(z => z.Id == id);
        }

        public async Task<PagedResponse<List<Acquisition>>> GetAllAcquisitionAsync(GetAllAcquisitionParameter filter)
        {
            var query = _acquisition
                            .Include(x => x.Supplier)
                            .Include(x => x.Items)
                            .ThenInclude(y => y.Inventory)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.ToLower();
                if (searchTerm.StartsWith("ac"))
                {
                    if (int.TryParse(searchTerm.Substring(2), out int acqId))
                    {
                        query = query.Where(u => u.Id == acqId);
                    }
                }
                else
                {
                    query = query.Where(x =>
                        x.Supplier.Name.ToLower().Contains(searchTerm));
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Status) && filter.Status.ToLower() != "all")
            {
                var status = filter.Status.ToLower();
                query = query.Where(x => x.Status.ToLower() == status);
            }

            var totalCount = await query.CountAsync();

            var acquisition = await query
                .OrderByDescending(p => p.Created)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResponse<List<Acquisition>>(acquisition, filter.PageNumber, filter.PageSize, totalCount);
        }

        public async Task<string> SavePdfAsync(int acquisitionId, byte[] pdfBytes)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "acquisitions");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"AC{acquisitionId:D4}_{DateTime.UtcNow:yyyy-MM-dd}.pdf";
            var fullPath = Path.Combine(folderPath, fileName);

            await File.WriteAllBytesAsync(fullPath, pdfBytes);

            return $"/pdfs/acquisitions/{fileName}";
        }
    }
}