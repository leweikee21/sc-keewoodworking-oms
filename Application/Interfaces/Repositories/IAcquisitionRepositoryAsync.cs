using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Acquisition.Queries;
using Application.Wrappers;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IAcquisitionRepositoryAsync : IGenericRepositoryAsync<Acquisition>
    {
        Task<int> CountAllAsync();
        Task<int> CountByStatusAsync(string status);
        new Task<Acquisition> GetByIdAsync(int id);
        Task<PagedResponse<List<Acquisition>>> GetAllAcquisitionAsync(GetAllAcquisitionParameter filter);
        Task <string> SavePdfAsync(int acquisitionId, byte[] pdf);
    }
}
