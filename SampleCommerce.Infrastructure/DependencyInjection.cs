using SampleCommerce.Infrastructure.External.DummyJson;
using SampleCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using SampleCommerce.Infrastructure.Services.Stores;

namespace SampleCommerce.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDatabase(configuration)
                .AddServices();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddMemoryCache();

            // - DummyJsonProductsClient (HttpClient)
            services.AddHttpClient<DummyJsonProductClient>(client =>
            {
                client.BaseAddress = new Uri("https://dummyjson.com/");
                client.Timeout = TimeSpan.FromSeconds(20);
            });

            services.AddScoped<IProductClient, CachedProductClient>();

            services.AddScoped<FavoritesStore>();
            services.AddScoped<BasketStore>();

            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}