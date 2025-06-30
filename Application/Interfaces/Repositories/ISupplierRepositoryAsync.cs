using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Supplier.Queries;
using Application.Wrappers;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ISupplierRepositoryAsync : IGenericRepositoryAsync<Supplier>
    {
        new Task<Supplier> GetByIdAsync(int id);
        Task<Supplier> GetByEmailAsync(string email);
        Task<PagedResponse<List<Supplier>>> GetAllSupplierAsync(GetAllSupplierParameter filter);
    }
}
