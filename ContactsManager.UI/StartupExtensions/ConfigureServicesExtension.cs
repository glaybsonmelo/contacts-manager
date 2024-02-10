using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RespositoryContracts;
using ServiceContracts;
using Services;

namespace CRUDExample
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddControllersWithViews();

            //add services into IoC container
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();

            services.AddScoped<ICountriesService, CountriesService>();

            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddScoped<IPersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();


            //Database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            });
            return services;
        }
    }
}
