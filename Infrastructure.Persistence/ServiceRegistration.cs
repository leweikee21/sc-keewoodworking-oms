using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            #region Repositories
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IProductRepositoryAsync, ProductRepositoryAsync>();
            services.AddTransient<ISupplierRepositoryAsync, SupplierRepositoryAsync>();
            services.AddTransient<IInventoryRepositoryAsync, InventoryRepositoryAsync>();
            services.AddTransient<IInboundRepositoryAsync, InboundRepositoryAsync>();
            services.AddTransient<IOutboundRepositoryAsync, OutboundRepositoryAsync>();
            services.AddTransient<IAcquisitionRepositoryAsync, AcquisitionRepositoryAsync>();
            services.AddTransient<IAcquisitionItemRepositoryAsync, AcquisitionItemRepositoryAsync>();
            services.AddTransient<IOrderRepositoryAsync, OrderRepositoryAsync>();
            services.AddTransient<IMaterialUsedRepositoryAsync, MaterialUsedRepositoryAsync>();
            #endregion
        }
    }
}
