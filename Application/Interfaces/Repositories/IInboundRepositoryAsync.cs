using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Inbound.Queries;
using Application.Wrappers;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IInboundRepositoryAsync : IGenericRepositoryAsync<Inbound>
    {
        new Task<Inbound> GetByIdAsync(int id);
        Task<PagedResponse<List<Inbound>>> GetAllInboundAsync(GetAllInboundParameter filter);
        Task<List<Inbound>> GetByInventoryIdAsync(int inventoryId);
    }
}
