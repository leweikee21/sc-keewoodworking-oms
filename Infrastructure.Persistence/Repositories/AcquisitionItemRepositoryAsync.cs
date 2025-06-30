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
using System.Linq;
using System.IO;

namespace Infrastructure.Persistence.Repositories
{
    public class AcquisitionItemRepositoryAsync : GenericRepositoryAsync<AcquisitionItem>, IAcquisitionItemRepositoryAsync
    {
        private readonly DbSet<AcquisitionItem> _acquisitionItem;

        public AcquisitionItemRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _acquisitionItem = dbContext.Set<AcquisitionItem>();
        }
    }
}