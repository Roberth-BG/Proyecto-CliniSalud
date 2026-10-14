using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProyectoCliniSalud.DataAccess.Data;
using ProyectoCliniSalud.DataAccess.Repositories;

namespace ProyectoCliniSalud.DataAccess
{
    public static class DataAccessServiceRegistration
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CliniSaludDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}