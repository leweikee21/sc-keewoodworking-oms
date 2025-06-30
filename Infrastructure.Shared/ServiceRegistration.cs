using Application.Interfaces;
using Domain.Settings;
using Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration _config)
        {
            services.Configure<MailSettings>(_config.GetSection("MailSettings"));
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IPdfService, PdfService>();

            services.Configure<CloudinarySettings>(_config.GetSection("CloudinarySettings"));

            var cloudinarySettings = _config.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            var account = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
            var cloudinary = new Cloudinary(account);
            services.AddSingleton(cloudinary);
        }
    }
}
