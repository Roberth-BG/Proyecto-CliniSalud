using Microsoft.Extensions.Logging;
using ProyectoCliniSalud.Business.Interfaces;
using ProyectoCliniSalud.DataAccess.Repositories;
using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Services
{
    public class TestimonioService : ITestimonioService
    {
        private readonly IGenericRepository<Testimonio> _testimonioRepository;
        private readonly IGenericRepository<Paciente> _pacienteRepository;
        private readonly IGenericRepository<Medico> _medicoRepository;
        private readonly ILogger<TestimonioService> _logger;

        public TestimonioService(
            IGenericRepository<Testimonio> testimonioRepository,
            IGenericRepository<Paciente> pacienteRepository,
            IGenericRepository<Medico> medicoRepository,
            ILogger<TestimonioService> logger)
        {
            _testimonioRepository = testimonioRepository;
            _pacienteRepository = pacienteRepository;
            _medicoRepository = medicoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Testimonio>> ObtenerTestimoniosAsync()
        {
            try
            {
                var query = await _testimonioRepository.ConsultarAsync();
                return query.OrderByDescending(t => t.Fecha).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los testimonios.");
                throw;
            }
        }

        public async Task<Testimonio?> ObtenerTestimonioPorIdAsync(int id)
        {
            return await _testimonioRepository.ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<Testimonio>> ObtenerTestimoniosPorPacienteAsync(int idPaciente)
        {
            try
            {
                var query = await _testimonioRepository.ConsultarAsync(t => t.IdPaciente == idPaciente);
                return query.OrderByDescending(t => t.Fecha).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener testimonios del paciente ID {IdPaciente}", idPaciente);
                throw;
            }
        }

        public async Task<IEnumerable<Testimonio>> ObtenerTestimoniosPorMedicoAsync(int idMedico)
        {
            try
            {
                var query = await _testimonioRepository.ConsultarAsync(t => t.IdMedico == idMedico && t.Estado == "Aprobado");
                return query.OrderByDescending(t => t.Fecha).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener testimonios del médico ID {IdMedico}", idMedico);
                throw;
            }
        }

        public async Task<IEnumerable<Testimonio>> ObtenerTestimoniosAprobadosAsync()
        {
            try
            {
                var query = await _testimonioRepository.ConsultarAsync(t => t.Estado == "Aprobado");
                return query.OrderByDescending(t => t.Fecha).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los testimonios aprobados.");
                throw;
            }
        }

        public async Task<IEnumerable<Testimonio>> BuscarTestimoniosAsync(string? textoBusqueda, string? estado)
        {
            try
            {
                var query = await _testimonioRepository.ConsultarAsync();

                if (!string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    string textoLimpio = textoBusqueda.Trim().ToLower();
                    query = query.Where(t => t.Comentario.ToLower().Contains(textoLimpio));
                }

                if (!string.IsNullOrWhiteSpace(estado))
                {
                    string estadoLimpio = estado.Trim().ToLower();
                    query = query.Where(t => t.Estado.ToLower() == estadoLimpio);
                }

                return query.OrderByDescending(t => t.Fecha).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar los testimonios.");
                throw;
            }
        }

        public async Task<(IEnumerable<Testimonio> Testimonios, int TotalRegistros)> ObtenerTestimoniosPaginadosAsync(int pagina, int tamanoPagina = 10)
        {
            try
            {
                var query = await _testimonioRepository.ConsultarAsync();
                int totalRegistros = query.Count();

                var paginados = query
                    .OrderByDescending(t => t.Fecha)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return (paginados, totalRegistros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al paginar los testimonios.");
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje, Testimonio? TestimonioCreado)> CrearTestimonioAsync(Testimonio testimonio)
        {
            try
            {
                var paciente = await _pacienteRepository.ObtenerPorIdAsync(testimonio.IdPaciente);
                if (paciente == null)
                {
                    return (false, "El paciente especificado no existe.", null);
                }

                if (testimonio.IdMedico.HasValue && testimonio.IdMedico.Value > 0)
                {
                    var medico = await _medicoRepository.ObtenerPorIdAsync(testimonio.IdMedico.Value);
                    if (medico == null)
                    {
                        return (false, "El médico especificado no existe.", null);
                    }
                }
                else
                {
                    testimonio.IdMedico = null; 
                }

                if (testimonio.Calificacion < 1 || testimonio.Calificacion > 5)
                {
                    return (false, "La calificación debe estar en un rango de 1 a 5 estrellas.", null);
                }

                testimonio.Estado = "Pendiente";
                testimonio.Fecha = DateTime.Now;

                await _testimonioRepository.CrearAsync(testimonio);
                _logger.LogInformation("Testimonio creado exitosamente por el paciente ID {IdPaciente}", testimonio.IdPaciente);

                return (true, "Testimonio enviado correctamente y pendiente de aprobación.", testimonio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar crear el testimonio.");
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarEstadoTestimonioAsync(int idTestimonio, string nuevoEstado)
        {
            try
            {
                var testimonio = await _testimonioRepository.ObtenerPorIdAsync(idTestimonio);
                if (testimonio == null)
                {
                    return (false, "El testimonio no existe.");
                }

                string estadoLimpio = nuevoEstado.Trim();
                string[] estadosPermitidos = { "Pendiente", "Aprobado", "Rechazado" };

                if (!estadosPermitidos.Contains(estadoLimpio, StringComparer.OrdinalIgnoreCase))
                {
                    return (false, "El estado proporcionado no es válido.");
                }

                testimonio.Estado = estadoLimpio;
                var resultado = await _testimonioRepository.ActualizarAsync(testimonio);

                if (!resultado) return (false, "No se pudo actualizar el estado del testimonio.");

                return (true, $"Testimonio marcado como '{estadoLimpio}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado del testimonio ID {Id}", idTestimonio);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> EliminarTestimonioAsync(int idTestimonio)
        {
            try
            {
                var testimonio = await _testimonioRepository.ObtenerPorIdAsync(idTestimonio);
                if (testimonio == null)
                {
                    return (false, "El testimonio no existe.");
                }

                var resultado = await _testimonioRepository.EliminarAsync(idTestimonio);
                if (!resultado)
                {
                    return (false, "No se pudo eliminar el testimonio.");
                }

                _logger.LogInformation("Testimonio ID {Id} eliminado correctamente.", idTestimonio);
                return (true, "Testimonio eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el testimonio ID {Id}", idTestimonio);
                throw;
            }
        }
    }
}