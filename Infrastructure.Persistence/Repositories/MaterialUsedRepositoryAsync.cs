using Application.Features.Order.Queries;
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
    public class MaterialUsedRepositoryAsync : GenericRepositoryAsync<MaterialUsed>, IMaterialUsedRepositoryAsync
    {
        private readonly DbSet<MaterialUsed> _materialUsed;

        public MaterialUsedRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _materialUsed = dbContext.Set<MaterialUsed>();
        }
    }
}