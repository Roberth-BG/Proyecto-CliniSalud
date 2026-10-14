using Microsoft.Extensions.Logging;
using ProyectoCliniSalud.Business.Interfaces;
using ProyectoCliniSalud.DataAccess.Repositories;
using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly IGenericRepository<Medico> _medicoRepository;
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IGenericRepository<Especialidad> _especialidadRepository;
        private readonly ILogger<MedicoService> _logger;

        public MedicoService(
            IGenericRepository<Medico> medicoRepository,
            IGenericRepository<Usuario> usuarioRepository,
            IGenericRepository<Especialidad> especialidadRepository,
            ILogger<MedicoService> logger)
        {
            _medicoRepository = medicoRepository;
            _usuarioRepository = usuarioRepository;
            _especialidadRepository = especialidadRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Medico>> ObtenerMedicosAsync()
        {
            try
            {
                var query = await _medicoRepository.ConsultarAsync();
                return query.OrderBy(m => m.IdMedico).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los médicos.");
                throw;
            }
        }

        public async Task<Medico?> ObtenerMedicoPorIdAsync(int id)
        {
            return await _medicoRepository.ObtenerPorIdAsync(id);
        }

        public async Task<Medico?> ObtenerMedicoPorIdUsuarioAsync(int idUsuario)
        {
            var query = await _medicoRepository.ConsultarAsync(m => m.IdUsuario == idUsuario);
            return query.FirstOrDefault();
        }

        public async Task<IEnumerable<Medico>> BuscarMedicosAsync(string? textoBusqueda, int? idEspecialidad)
        {
            try
            {
                var query = await _medicoRepository.ConsultarAsync();

                if (!string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    string textoLimpio = textoBusqueda.Trim().ToLower();
                    query = query.Where(m => m.Nombres.ToLower().Contains(textoLimpio) ||
                                             m.Apellidos.ToLower().Contains(textoLimpio) ||
                                             m.Cmp.Contains(textoLimpio) ||
                                             m.Consultorio.ToLower().Contains(textoLimpio));
                }

                if (idEspecialidad.HasValue && idEspecialidad.Value > 0)
                {
                    query = query.Where(m => m.IdEspecialidad == idEspecialidad.Value);
                }

                return query.OrderBy(m => m.IdMedico).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar los médicos.");
                throw;
            }
        }

        public async Task<(IEnumerable<Medico> Medicos, int TotalRegistros)> ObtenerMedicosPaginadosAsync(int pagina, int tamanoPagina = 9)
        {
            try
            {
                var query = await _medicoRepository.ConsultarAsync();
                int totalRegistros = query.Count();

                var paginados = query
                    .OrderBy(m => m.IdMedico)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return (paginados, totalRegistros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al paginar los médicos.");
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> RegistrarMedicoAsync(Medico medico, string email, string passwordPlano)
        {
            try
            {
                var correoExistente = await _usuarioRepository.ConsultarAsync(u => u.Email.ToLower() == email.ToLower());
                if (correoExistente.Any())
                {
                    return (false, "El correo electrónico ya se encuentra registrado.");
                }

                var cmpExistente = await _medicoRepository.ConsultarAsync(m => m.Cmp == medico.Cmp);
                if (cmpExistente.Any())
                {
                    return (false, "El número de CMP ya se encuentra registrado.");
                }

                var especialidadExiste = await _especialidadRepository.ObtenerPorIdAsync(medico.IdEspecialidad);
                if (especialidadExiste == null || !especialidadExiste.Activo)
                {
                    return (false, "La especialidad seleccionada no es válida o está inactiva.");
                }

                var nuevoUsuario = new Usuario
                {
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordPlano),
                    IdRol = 2,
                    CuentaVerificada = true,
                    Activo = true,
                    IntentosFallidos = 0,
                    FechaRegistro = DateTime.Now
                };

                await _usuarioRepository.CrearAsync(nuevoUsuario);

                medico.IdUsuario = nuevoUsuario.IdUsuario;
                if (string.IsNullOrWhiteSpace(medico.FotoUrl))
                {
                    medico.FotoUrl = "/assets/img/medicos/default.png";
                }

                await _medicoRepository.CrearAsync(medico);
                return (true, "Médico registrado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el médico con CMP {Cmp}", medico.Cmp);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarMedicoAsync(Medico medico)
        {
            try
            {
                var cmpDuplicado = await _medicoRepository.ConsultarAsync(m => m.Cmp == medico.Cmp && m.IdMedico != medico.IdMedico);
                if (cmpDuplicado.Any())
                {
                    return (false, "El CMP ya pertenece a otro médico.");
                }

                var actual = await _medicoRepository.ObtenerPorIdAsync(medico.IdMedico);
                if (actual == null) return (false, "El médico no existe.");

                actual.Nombres = medico.Nombres;
                actual.Apellidos = medico.Apellidos;
                actual.Cmp = medico.Cmp;
                actual.Telefono = medico.Telefono;
                actual.Consultorio = medico.Consultorio;
                actual.IdEspecialidad = medico.IdEspecialidad;
                if (!string.IsNullOrWhiteSpace(medico.FotoUrl))
                {
                    actual.FotoUrl = medico.FotoUrl;
                }

                var resultado = await _medicoRepository.ActualizarAsync(actual);
                if (!resultado) return (false, "No se pudo actualizar la información del médico.");

                return (true, "Médico actualizado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el médico ID {Id}", medico.IdMedico);
                throw;
            }
        }
    }
}