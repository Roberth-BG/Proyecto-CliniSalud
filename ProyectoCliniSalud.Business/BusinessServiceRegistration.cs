using Microsoft.Extensions.DependencyInjection;
using ProyectoCliniSalud.Business.Interfaces;
using ProyectoCliniSalud.Business.Services;

namespace ProyectoCliniSalud.Business
{
    public static class BusinessServiceRegistration
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IRolService, RolService>();

            services.AddScoped<IEspecialidadService, EspecialidadService>();

            services.AddScoped<IUsuarioService, UsuarioService>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IPacienteService, PacienteService>();

            services.AddScoped<IMedicoService, MedicoService>();

            services.AddScoped<ICitaService, CitaService>();

            services.AddScoped<IAtencionService, AtencionService>();

            services.AddScoped<ITestimonioService, TestimonioService>();

            return services;
        }
    }
}