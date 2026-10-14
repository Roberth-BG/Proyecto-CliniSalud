using Microsoft.Extensions.Logging;
using ProyectoCliniSalud.Business.Interfaces;
using ProyectoCliniSalud.DataAccess.Repositories;
using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Services
{
    public class AtencionService : IAtencionService
    {
        private readonly IGenericRepository<Atencion> _atencionRepository;
        private readonly IGenericRepository<Cita> _citaRepository;
        private readonly ILogger<AtencionService> _logger;

        public AtencionService(
            IGenericRepository<Atencion> atencionRepository,
            IGenericRepository<Cita> citaRepository,
            ILogger<AtencionService> logger)
        {
            _atencionRepository = atencionRepository;
            _citaRepository = citaRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Atencion>> ObtenerAtencionesAsync()
        {
            try
            {
                var query = await _atencionRepository.ConsultarAsync();
                return query.OrderByDescending(a => a.FechaAtencion).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar las atenciones médicas.");
                throw;
            }
        }

        public async Task<Atencion?> ObtenerAtencionPorIdAsync(int id)
        {
            return await _atencionRepository.ObtenerPorIdAsync(id);
        }

        public async Task<Atencion?> ObtenerAtencionPorIdCitaAsync(int idCita)
        {
            var query = await _atencionRepository.ConsultarAsync(a => a.IdCita == idCita);
            return query.FirstOrDefault();
        }

        public async Task<IEnumerable<Atencion>> BuscarAtencionesAsync(string? textoBusqueda)
        {
            try
            {
                var query = await _atencionRepository.ConsultarAsync();

                if (!string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    string textoLimpio = textoBusqueda.Trim().ToLower();
                    query = query.Where(a => a.Diagnostico.ToLower().Contains(textoLimpio) ||
                                             a.Tratamiento.ToLower().Contains(textoLimpio));
                }

                return query.OrderByDescending(a => a.FechaAtencion).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar las atenciones médicas.");
                throw;
            }
        }

        public async Task<(IEnumerable<Atencion> Atenciones, int TotalRegistros)> ObtenerAtencionesPaginadasAsync(int pagina, int tamanoPagina = 10)
        {
            try
            {
                var query = await _atencionRepository.ConsultarAsync();
                int totalRegistros = query.Count();

                var paginadas = query
                    .OrderByDescending(a => a.FechaAtencion)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return (paginadas, totalRegistros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al paginar las atenciones médicas.");
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje, Atencion? AtencionCreada)> RegistrarAtencionAsync(Atencion atencion)
        {
            try
            {
                var cita = await _citaRepository.ObtenerPorIdAsync(atencion.IdCita);
                if (cita == null)
                {
                    return (false, "La cita médica asociada no existe.", null);
                }

                var atencionExistente = await _atencionRepository.ConsultarAsync(a => a.IdCita == atencion.IdCita);
                if (atencionExistente.Any())
                {
                    return (false, "Ya existe una atención médica registrada para esta cita.", null);
                }

                atencion.FechaAtencion = DateTime.Now;

                await _atencionRepository.CrearAsync(atencion);

                cita.Estado = "Atendida";
                await _citaRepository.ActualizarAsync(cita);

                _logger.LogInformation("Atención médica registrada exitosamente para la cita ID {IdCita}", atencion.IdCita);
                return (true, "Atención médica registrada correctamente y cita marcada como atendida.", atencion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar registrar la atención médica.");
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarAtencionAsync(Atencion atencion)
        {
            try
            {
                var actual = await _atencionRepository.ObtenerPorIdAsync(atencion.IdAtencion);
                if (actual == null)
                {
                    return (false, "La atención médica no existe.");
                }

                actual.Diagnostico = atencion.Diagnostico;
                actual.Tratamiento = atencion.Tratamiento;

                var resultado = await _atencionRepository.ActualizarAsync(actual);
                if (!resultado) return (false, "No se pudo actualizar la atención médica.");

                return (true, "Atención médica actualizada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la atención médica ID {Id}", atencion.IdAtencion);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> EliminarAtencionAsync(int idAtencion)
        {
            try
            {
                var atencion = await _atencionRepository.ObtenerPorIdAsync(idAtencion);
                if (atencion == null)
                {
                    return (false, "La atención médica no existe.");
                }

                var cita = await _citaRepository.ObtenerPorIdAsync(atencion.IdCita);
                if (cita != null && cita.Estado.Equals("Atendida", StringComparison.OrdinalIgnoreCase))
                {
                    cita.Estado = "Confirmada";
                    await _citaRepository.ActualizarAsync(cita);
                }

                var resultado = await _atencionRepository.EliminarAsync(idAtencion);
                if (!resultado)
                {
                    return (false, "No se pudo eliminar la atención médica.");
                }

                _logger.LogInformation("Atención médica ID {Id} eliminada correctamente.", idAtencion);
                return (true, "Atención médica eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la atención médica ID {Id}", idAtencion);
                throw;
            }
        }
    }
}