using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Interfaces
{
    public interface IMedicoService
    {
        Task<IEnumerable<Medico>> ObtenerMedicosAsync();
        Task<Medico?> ObtenerMedicoPorIdAsync(int id);
        Task<Medico?> ObtenerMedicoPorIdUsuarioAsync(int idUsuario);
        Task<IEnumerable<Medico>> BuscarMedicosAsync(string? textoBusqueda, int? idEspecialidad);
        Task<(IEnumerable<Medico> Medicos, int TotalRegistros)> ObtenerMedicosPaginadosAsync(int pagina, int tamanoPagina = 9);
        Task<(bool Exito, string Mensaje)> RegistrarMedicoAsync(Medico medico, string email, string passwordPlano);
        Task<(bool Exito, string Mensaje)> ActualizarMedicoAsync(Medico medico);
    }
}