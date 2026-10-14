using Microsoft.Extensions.Logging;
using ProyectoCliniSalud.Business.Interfaces;
using ProyectoCliniSalud.DataAccess.Repositories;
using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Services
{
    public class CitaService : ICitaService
    {
        private readonly IGenericRepository<Cita> _citaRepository;
        private readonly IGenericRepository<Paciente> _pacienteRepository;
        private readonly IGenericRepository<Medico> _medicoRepository;
        private readonly ILogger<CitaService> _logger;

        public CitaService(
            IGenericRepository<Cita> citaRepository,
            IGenericRepository<Paciente> pacienteRepository,
            IGenericRepository<Medico> medicoRepository,
            ILogger<CitaService> logger)
        {
            _citaRepository = citaRepository;
            _pacienteRepository = pacienteRepository;
            _medicoRepository = medicoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Cita>> ObtenerCitasAsync()
        {
            try
            {
                var query = await _citaRepository.ConsultarAsync();
                return query.OrderByDescending(c => c.FechaCita).ThenBy(c => c.HoraCita).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar las citas.");
                throw;
            }
        }

        public async Task<Cita?> ObtenerCitaPorIdAsync(int id)
        {
            return await _citaRepository.ObtenerPorIdAsync(id);
        }

        public async Task<Cita?> ObtenerCitaPorCodigoTicketAsync(string codigoTicket)
        {
            var query = await _citaRepository.ConsultarAsync(c => c.CodigoTicket.ToLower() == codigoTicket.ToLower());
            return query.FirstOrDefault();
        }

        public async Task<IEnumerable<Cita>> BuscarCitasAsync(string? textoBusqueda, string? estado)
        {
            try
            {
                var query = await _citaRepository.ConsultarAsync();

                if (!string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    string textoLimpio = textoBusqueda.Trim().ToLower();
                    query = query.Where(c => c.CodigoTicket.ToLower().Contains(textoLimpio) ||
                                             c.MotivoConsulta.ToLower().Contains(textoLimpio));
                }

                if (!string.IsNullOrWhiteSpace(estado))
                {
                    string estadoLimpio = estado.Trim().ToLower();
                    query = query.Where(c => c.Estado.ToLower() == estadoLimpio);
                }

                return query.OrderByDescending(c => c.FechaCita).ThenBy(c => c.HoraCita).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar las citas.");
                throw;
            }
        }

        public async Task<IEnumerable<Cita>> ObtenerCitasPorPacienteAsync(int idPaciente)
        {
            try
            {
                var query = await _citaRepository.ConsultarAsync(c => c.IdPaciente == idPaciente);
                return query.OrderByDescending(c => c.FechaCita).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas del paciente ID {IdPaciente}", idPaciente);
                throw;
            }
        }

        public async Task<IEnumerable<Cita>> ObtenerCitasPorMedicoAsync(int idMedico, DateTime? fecha = null)
        {
            try
            {
                var query = await _citaRepository.ConsultarAsync(c => c.IdMedico == idMedico);

                if (fecha.HasValue)
                {
                    query = query.Where(c => c.FechaCita.Date == fecha.Value.Date);
                }

                return query.OrderBy(c => c.HoraCita).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas del médico ID {IdMedico}", idMedico);
                throw;
            }
        }

        public async Task<(IEnumerable<Cita> Citas, int TotalRegistros)> ObtenerCitasPaginadasAsync(int pagina, int tamanoPagina = 10)
        {
            try
            {
                var query = await _citaRepository.ConsultarAsync();
                int totalRegistros = query.Count();

                var paginadas = query
                    .OrderByDescending(c => c.FechaCita)
                    .ThenBy(c => c.HoraCita)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return (paginadas, totalRegistros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al paginar las citas.");
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje, Cita? CitaCreada)> CrearCitaAsync(Cita cita)
        {
            try
            {
                var paciente = await _pacienteRepository.ObtenerPorIdAsync(cita.IdPaciente);
                if (paciente == null)
                {
                    return (false, "El paciente especificado no existe.", null);
                }

                var medico = await _medicoRepository.ObtenerPorIdAsync(cita.IdMedico);
                if (medico == null)
                {
                    return (false, "El médico especificado no existe.", null);
                }

                cita.HoraCita = new TimeSpan(cita.HoraCita.Hours, cita.HoraCita.Minutes, 0);

                var citasExistentes = await _citaRepository.ConsultarAsync(c =>
                    c.IdMedico == cita.IdMedico &&
                    c.FechaCita.Date == cita.FechaCita.Date &&
                    c.HoraCita == cita.HoraCita &&
                    (c.Estado == "Reservada" || c.Estado == "Confirmada"));

                if (citasExistentes.Any())
                {
                    return (false, "El médico ya cuenta con una cita reservada o confirmada en esta fecha y horario.", null);
                }

                string randomSuffix = new Random().Next(1000, 9999).ToString();
                cita.CodigoTicket = $"TKT-{DateTime.Now:yyyyMMdd}-{randomSuffix}";
                cita.Estado = "Reservada";
                cita.FechaReserva = DateTime.Now;

                await _citaRepository.CrearAsync(cita);
                _logger.LogInformation("Cita creada exitosamente con ticket: {Ticket}", cita.CodigoTicket);

                return (true, "Cita médica reservada exitosamente.", cita);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar crear la cita médica.");
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarEstadoCitaAsync(int idCita, string nuevoEstado)
        {
            try
            {
                var cita = await _citaRepository.ObtenerPorIdAsync(idCita);
                if (cita == null)
                {
                    return (false, "La cita médica no existe.");
                }

                string estadoLimpio = nuevoEstado.Trim();

                string[] estadosPermitidos = { "Reservada", "Confirmada", "Atendida", "No Asistio", "Cancelada" };

                if (!estadosPermitidos.Contains(estadoLimpio, StringComparer.OrdinalIgnoreCase))
                {
                    return (false, "El estado proporcionado no es válido según las reglas institucionales.");
                }

                cita.Estado = estadoLimpio;
                var resultado = await _citaRepository.ActualizarAsync(cita);

                if (!resultado) return (false, "No se pudo actualizar el estado de la cita.");

                return (true, $"Estado de la cita actualizado a '{estadoLimpio}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado de la cita ID {Id}", idCita);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> CancelarCitaAsync(int idCita)
        {
            try
            {
                var cita = await _citaRepository.ObtenerPorIdAsync(idCita);
                if (cita == null)
                {
                    return (false, "La cita médica no existe.");
                }

                if (cita.Estado.Equals("Atendida", StringComparison.OrdinalIgnoreCase))
                {
                    return (false, "No se puede cancelar una cita que ya ha sido atendida.");
                }

                if (cita.Estado.Equals("Cancelada", StringComparison.OrdinalIgnoreCase))
                {
                    return (false, "La cita ya se encuentra cancelada.");
                }

                cita.Estado = "Cancelada";
                await _citaRepository.ActualizarAsync(cita);

                return (true, "Cita cancelada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar la cita ID {Id}", idCita);
                throw;
            }
        }
    }
}