using Microsoft.Extensions.Logging;
using ProyectoCliniSalud.Business.Interfaces;
using ProyectoCliniSalud.DataAccess.Repositories;
using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly IGenericRepository<Paciente> _pacienteRepository;
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly ILogger<PacienteService> _logger;

        public PacienteService(
            IGenericRepository<Paciente> pacienteRepository,
            IGenericRepository<Usuario> usuarioRepository,
            ILogger<PacienteService> logger)
        {
            _pacienteRepository = pacienteRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Paciente>> ObtenerPacientesAsync()
        {
            try
            {
                var query = await _pacienteRepository.ConsultarAsync();
                return query.OrderBy(p => p.IdPaciente).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los pacientes.");
                throw;
            }
        }

        public async Task<Paciente?> ObtenerPacientePorIdAsync(int id)
        {
            return await _pacienteRepository.ObtenerPorIdAsync(id);
        }

        public async Task<Paciente?> ObtenerPacientePorIdUsuarioAsync(int idUsuario)
        {
            var query = await _pacienteRepository.ConsultarAsync(p => p.IdUsuario == idUsuario);
            return query.FirstOrDefault();
        }

        public async Task<IEnumerable<Paciente>> BuscarPacientesAsync(string? textoBusqueda)
        {
            try
            {
                var query = await _pacienteRepository.ConsultarAsync();

                if (!string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    string textoLimpio = textoBusqueda.Trim().ToLower();
                    query = query.Where(p => p.Nombres.ToLower().Contains(textoLimpio) ||
                                             p.Apellidos.ToLower().Contains(textoLimpio) ||
                                             p.Dni.Contains(textoLimpio) ||
                                             p.Telefono.Contains(textoLimpio));
                }

                return query.OrderBy(p => p.IdPaciente).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar los pacientes.");
                throw;
            }
        }

        public async Task<(IEnumerable<Paciente> Pacientes, int TotalRegistros)> ObtenerPacientesPaginadosAsync(int pagina, int tamanoPagina = 9)
        {
            try
            {
                var query = await _pacienteRepository.ConsultarAsync();
                int totalRegistros = query.Count();

                var paginados = query
                    .OrderBy(p => p.IdPaciente)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return (paginados, totalRegistros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al paginar los pacientes.");
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> RegistrarPacienteAsync(Paciente paciente, string email, string passwordPlano)
        {
            try
            {
                var correoExistente = await _usuarioRepository.ConsultarAsync(u => u.Email.ToLower() == email.ToLower());
                if (correoExistente.Any())
                {
                    return (false, "El correo electrónico ya se encuentra registrado.");
                }

                var dniExistente = await _pacienteRepository.ConsultarAsync(p => p.Dni == paciente.Dni);
                if (dniExistente.Any())
                {
                    return (false, "El DNI ingresado ya está registrado.");
                }

                var nuevoUsuario = new Usuario
                {
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordPlano),
                    IdRol = 3,
                    CuentaVerificada = true,
                    Activo = true,
                    IntentosFallidos = 0,
                    FechaRegistro = DateTime.Now
                };

                await _usuarioRepository.CrearAsync(nuevoUsuario);

                paciente.IdUsuario = nuevoUsuario.IdUsuario;

                if (string.IsNullOrWhiteSpace(paciente.FotoUrl))
                {
                    paciente.FotoUrl = "/assets/img/pacientes/default.png";
                }

                await _pacienteRepository.CrearAsync(paciente);
                return (true, "Paciente registrado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el paciente con DNI {Dni}", paciente.Dni);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarPacienteAsync(Paciente paciente)
        {
            try
            {
                var dniDuplicado = await _pacienteRepository.ConsultarAsync(p => p.Dni == paciente.Dni && p.IdPaciente != paciente.IdPaciente);
                if (dniDuplicado.Any())
                {
                    return (false, "El DNI ya pertenece a otro paciente.");
                }

                var actual = await _pacienteRepository.ObtenerPorIdAsync(paciente.IdPaciente);
                if (actual == null) return (false, "El paciente no existe.");

                actual.Nombres = paciente.Nombres;
                actual.Apellidos = paciente.Apellidos;
                actual.Telefono = paciente.Telefono;
                actual.Dni = paciente.Dni;
                if (!string.IsNullOrWhiteSpace(paciente.FotoUrl))
                {
                    actual.FotoUrl = paciente.FotoUrl;
                }

                var resultado = await _pacienteRepository.ActualizarAsync(actual);
                if (!resultado) return (false, "No se pudo actualizar la información del paciente.");

                return (true, "Paciente actualizado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el paciente ID {Id}", paciente.IdPaciente);
                throw;
            }
        }
    }
}