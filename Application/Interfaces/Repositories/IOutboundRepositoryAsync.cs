using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Outbound.Queries;
using Application.Wrappers;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IOutboundRepositoryAsync : IGenericRepositoryAsync<Outbound>
    {
        new Task<Outbound> GetByIdAsync(int id);
        Task<PagedResponse<List<Outbound>>> GetAllOutboundAsync(GetAllOutboundParameter filter);
    }
}
