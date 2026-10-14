using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> ObtenerRolesAsync();
        Task<Rol?> ObtenerRolPorIdAsync(int id);
        Task<IEnumerable<Rol>> BuscarRolesAsync(string? textoBusqueda, bool soloActivos);
        Task<(IEnumerable<Rol> Roles, int TotalRegistros)> ObtenerRolesPaginadosAsync(int pagina, int tamanoPagina = 10);
        Task<(bool Exito, string Mensaje)> CrearRolAsync(Rol rol);
        Task<(bool Exito, string Mensaje)> ActualizarRolAsync(Rol rol);
        Task<(bool Exito, string Mensaje)> DesactivarRolAsync(int id);
    }
}