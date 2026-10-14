using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Exito, string Mensaje, Usuario? Usuario)> LoginAsync(string email, string password);
        Task<bool> RegistrarIntentoLoginAsync(int idUsuario, bool exitoso);
        Task<(bool Exito, string Mensaje, int IdUsuario)> RegistrarPacientePublicoAsync(string email, string passwordPlano, int idRolPaciente = 3);
        Task<(bool Exito, string Mensaje)> ConfirmarCuentaAsync(string email);
        Task<(bool Exito, string Mensaje)> GenerarCodigoRecuperacionAsync(string email);
        Task<bool> ValidarCodigoRecuperacionAsync(string email, string codigoVerificacion);
        Task<(bool Exito, string Mensaje)> RestablecerPasswordAsync(string email, string codigoVerificacion, string nuevoPasswordPlano);
    }
}