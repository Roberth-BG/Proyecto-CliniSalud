using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Interfaces
{
    public interface ICitaService
    {
        Task<IEnumerable<Cita>> ObtenerCitasAsync();
        Task<Cita?> ObtenerCitaPorIdAsync(int id);
        Task<Cita?> ObtenerCitaPorCodigoTicketAsync(string codigoTicket);
        Task<IEnumerable<Cita>> BuscarCitasAsync(string? textoBusqueda, string? estado);
        Task<IEnumerable<Cita>> ObtenerCitasPorPacienteAsync(int idPaciente);
        Task<IEnumerable<Cita>> ObtenerCitasPorMedicoAsync(int idMedico, DateTime? fecha = null);
        Task<(IEnumerable<Cita> Citas, int TotalRegistros)> ObtenerCitasPaginadasAsync(int pagina, int tamanoPagina = 10);
        Task<(bool Exito, string Mensaje, Cita? CitaCreada)> CrearCitaAsync(Cita cita);
        Task<(bool Exito, string Mensaje)> ActualizarEstadoCitaAsync(int idCita, string nuevoEstado);
        Task<(bool Exito, string Mensaje)> CancelarCitaAsync(int idCita);
    }
}