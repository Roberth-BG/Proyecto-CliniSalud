using Microsoft.Extensions.Logging;
using ProyectoCliniSalud.Business.Interfaces;
using ProyectoCliniSalud.DataAccess.Repositories;
using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Services
{
    public class EspecialidadService : IEspecialidadService
    {
        private readonly IGenericRepository<Especialidad> _especialidadRepository;
        private readonly IGenericRepository<Medico> _medicoRepository;
        private readonly ILogger<EspecialidadService> _logger;

        public EspecialidadService(
            IGenericRepository<Especialidad> especialidadRepository,
            IGenericRepository<Medico> medicoRepository,
            ILogger<EspecialidadService> logger)
        {
            _especialidadRepository = especialidadRepository;
            _medicoRepository = medicoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Especialidad>> ObtenerEspecialidadesAsync()
        {
            try
            {
                var query = await _especialidadRepository.ConsultarAsync();
                return query.OrderBy(e => e.IdEspecialidad).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar las especialidades.");
                throw;
            }
        }

        public async Task<Especialidad?> ObtenerEspecialidadPorIdAsync(int id)
        {
            return await _especialidadRepository.ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<Especialidad>> BuscarEspecialidadesAsync(string? textoBusqueda, bool soloActivas)
        {
            try
            {
                var query = await _especialidadRepository.ConsultarAsync();

                if (!string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    string textoLimpio = textoBusqueda.Trim().ToLower();
                    query = query.Where(e => e.NombreEspecialidad.ToLower().Contains(textoLimpio));
                }

                if (soloActivas)
                {
                    query = query.Where(e => e.Activo);
                }

                return query.OrderBy(e => e.IdEspecialidad).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar las especialidades.");
                throw;
            }
        }

        public async Task<(IEnumerable<Especialidad> Especialidades, int TotalRegistros)> ObtenerEspecialidadesPaginadasAsync(int pagina, int tamanoPagina = 10)
        {
            try
            {
                var query = await _especialidadRepository.ConsultarAsync();

                int totalRegistros = query.Count();

                var especialidadesPaginadas = query
                    .OrderBy(e => e.IdEspecialidad)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return (especialidadesPaginadas, totalRegistros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al paginar las especialidades.");
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> CrearEspecialidadAsync(Especialidad especialidad)
        {
            try
            {
                var existentes = await _especialidadRepository.ConsultarAsync(e => e.NombreEspecialidad.ToLower() == especialidad.NombreEspecialidad.ToLower());
                if (existentes.Any())
                {
                    return (false, "El nombre de la especialidad ya se encuentra registrado.");
                }

                await _especialidadRepository.CrearAsync(especialidad);
                _logger.LogInformation("Especialidad creada exitosamente: {NombreEspecialidad}", especialidad.NombreEspecialidad);
                return (true, "Especialidad creada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar crear la especialidad {NombreEspecialidad}", especialidad.NombreEspecialidad);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarEspecialidadAsync(Especialidad especialidad)
        {
            try
            {
                var existentes = await _especialidadRepository.ConsultarAsync(e => e.NombreEspecialidad.ToLower() == especialidad.NombreEspecialidad.ToLower() && e.IdEspecialidad != especialidad.IdEspecialidad);
                if (existentes.Any())
                {
                    return (false, "Ya existe otra especialidad con ese mismo nombre.");
                }

                var resultado = await _especialidadRepository.ActualizarAsync(especialidad);
                if (!resultado) return (false, "No se pudo actualizar la especialidad.");

                return (true, "Especialidad actualizada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la especialidad ID {IdEspecialidad}", especialidad.IdEspecialidad);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> DesactivarEspecialidadAsync(int id)
        {
            try
            {
                var medicosActivosConEspecialidad = await _medicoRepository.ConsultarAsync(m => m.IdEspecialidad == id && m.Activo);
                if (medicosActivosConEspecialidad.Any())
                {
                    return (false, "No se puede desactivar la especialidad porque existen médicos activos vinculados a ella.");
                }

                var especialidad = await _especialidadRepository.ObtenerPorIdAsync(id);
                if (especialidad == null) return (false, "La especialidad no existe.");

                especialidad.Activo = false;
                await _especialidadRepository.ActualizarAsync(especialidad);

                return (true, "Especialidad desactivada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar la especialidad ID {Id}", id);
                throw;
            }
        }
    }
}