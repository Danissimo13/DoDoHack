using DoDoHack.Data;
using DoDoHack.Services.Abstractions;
using DoDoHack.Services.Implementations;
using DoDoModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace DoDoHack.Extensions
{
    public static class StartupServicesExtensions
    {
        public static IServiceCollection AddHashService(this IServiceCollection services, string key)
        {
            var instance = new ShaHashService(key);
            services.AddSingleton<IHashService, ShaHashService>(provider => instance);

            return services;
        }

        public static IServiceCollection AddStartupDbEntities(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = services.BuildServiceProvider();
            var dbContext = provider.GetRequiredService<DodoBase>();
            var hashService = provider.GetRequiredService<IHashService>();

            var adminSet = dbContext.Set<Admin>();
            if (adminSet.Any() == false)
            {
                var adminDataSection = configuration.GetSection("Admin");

                var login = adminDataSection.GetValue<string>("Login");
                var password = adminDataSection.GetValue<string>("Password");
                var passwordHash = hashService.GetStringHash(password);

                adminSet.Add(new Admin() { Login = login, Password = passwordHash });
                dbContext.SaveChanges();
            }

            return services;
        }

        public static IServiceCollection AddOrderDistributionService(this IServiceCollection services)
        {
            services.AddScoped<IOrderDistributionService, OrderDistributionService>();

            return services;
        }

        public static IServiceCollection AddSignalUserIdProvider(this IServiceCollection services)
        {
            services.AddSingleton<IUserIdProvider, SignalUserIdProvider>();

            return services;
        }

        public static IServiceCollection AddEmailSender(this IServiceCollection services)
        {
            services.AddSingleton<IEmailSender, EmailSender>();

            return services;
        }

        public static IServiceCollection AddSupportService(this IServiceCollection services)
        {
            services.AddSingleton<ISupportService, SupportService>();

            return services;
        }

        public static IServiceCollection AddFileService(this IServiceCollection services)
        {
            services.AddSingleton<IFileService, FileService>();

            return services;
        }
    }
}
