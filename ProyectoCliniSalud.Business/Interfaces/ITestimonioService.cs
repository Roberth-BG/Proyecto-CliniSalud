using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Interfaces
{
    public interface ITestimonioService
    {
        Task<IEnumerable<Testimonio>> ObtenerTestimoniosAsync();
        Task<Testimonio?> ObtenerTestimonioPorIdAsync(int id);
        Task<IEnumerable<Testimonio>> ObtenerTestimoniosPorPacienteAsync(int idPaciente);
        Task<IEnumerable<Testimonio>> ObtenerTestimoniosPorMedicoAsync(int idMedico);
        Task<IEnumerable<Testimonio>> ObtenerTestimoniosAprobadosAsync();
        Task<IEnumerable<Testimonio>> BuscarTestimoniosAsync(string? textoBusqueda, string? estado);
        Task<(IEnumerable<Testimonio> Testimonios, int TotalRegistros)> ObtenerTestimoniosPaginadosAsync(int pagina, int tamanoPagina = 10);
        Task<(bool Exito, string Mensaje, Testimonio? TestimonioCreado)> CrearTestimonioAsync(Testimonio testimonio);
        Task<(bool Exito, string Mensaje)> ActualizarEstadoTestimonioAsync(int idTestimonio, string nuevoEstado);
        Task<(bool Exito, string Mensaje)> EliminarTestimonioAsync(int idTestimonio);
    }
}