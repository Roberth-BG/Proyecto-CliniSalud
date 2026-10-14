using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Interfaces
{
    public interface IPacienteService
    {
        Task<IEnumerable<Paciente>> ObtenerPacientesAsync();
        Task<Paciente?> ObtenerPacientePorIdAsync(int id);
        Task<Paciente?> ObtenerPacientePorIdUsuarioAsync(int idUsuario);
        Task<IEnumerable<Paciente>> BuscarPacientesAsync(string? textoBusqueda);
        Task<(IEnumerable<Paciente> Pacientes, int TotalRegistros)> ObtenerPacientesPaginadosAsync(int pagina, int tamanoPagina = 9);
        Task<(bool Exito, string Mensaje)> RegistrarPacienteAsync(Paciente paciente, string email, string passwordPlano);
        Task<(bool Exito, string Mensaje)> ActualizarPacienteAsync(Paciente paciente);
    }
}