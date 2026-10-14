using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Interfaces
{
    public interface IEspecialidadService
    {
        Task<IEnumerable<Especialidad>> ObtenerEspecialidadesAsync();
        Task<Especialidad?> ObtenerEspecialidadPorIdAsync(int id);
        Task<IEnumerable<Especialidad>> BuscarEspecialidadesAsync(string? textoBusqueda, bool soloActivas);
        Task<(IEnumerable<Especialidad> Especialidades, int TotalRegistros)> ObtenerEspecialidadesPaginadasAsync(int pagina, int tamanoPagina = 10);
        Task<(bool Exito, string Mensaje)> CrearEspecialidadAsync(Especialidad especialidad);
        Task<(bool Exito, string Mensaje)> ActualizarEspecialidadAsync(Especialidad especialidad);
        Task<(bool Exito, string Mensaje)> DesactivarEspecialidadAsync(int id);
    }
}