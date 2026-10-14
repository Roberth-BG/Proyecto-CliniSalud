using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> ObtenerUsuariosAsync();
        Task<Usuario?> ObtenerUsuarioPorIdAsync(int id);
        Task<Usuario?> ObtenerUsuarioPorCorreoAsync(string correo);
        Task<IEnumerable<Usuario>> BuscarUsuariosAsync(string? textoBusqueda, int? idRol, bool soloActivos);
        Task<(IEnumerable<Usuario> Usuarios, int TotalRegistros)> ObtenerUsuariosPaginadosAsync(int pagina, int tamanoPagina = 10);
        Task<(bool Exito, string Mensaje)> CrearUsuarioAsync(Usuario usuario, string passwordPlano);
        Task<(bool Exito, string Mensaje)> ActualizarUsuarioAsync(Usuario usuario);
        Task<(bool Exito, string Mensaje)> CambiarPasswordAsync(int idUsuario, string passwordActual, string nuevoPassword);
        Task<(bool Exito, string Mensaje)> DesactivarUsuarioAsync(int id);
    }
}