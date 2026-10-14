using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Interfaces
{
    public interface IAtencionService
    {
        Task<IEnumerable<Atencion>> ObtenerAtencionesAsync();
        Task<Atencion?> ObtenerAtencionPorIdAsync(int id);
        Task<Atencion?> ObtenerAtencionPorIdCitaAsync(int idCita);
        Task<IEnumerable<Atencion>> BuscarAtencionesAsync(string? textoBusqueda);
        Task<(IEnumerable<Atencion> Atenciones, int TotalRegistros)> ObtenerAtencionesPaginadasAsync(int pagina, int tamanoPagina = 10);
        Task<(bool Exito, string Mensaje, Atencion? AtencionCreada)> RegistrarAtencionAsync(Atencion atencion);
        Task<(bool Exito, string Mensaje)> ActualizarAtencionAsync(Atencion atencion);
        Task<(bool Exito, string Mensaje)> EliminarAtencionAsync(int idAtencion);
    }
}