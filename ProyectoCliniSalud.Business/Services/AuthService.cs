using Microsoft.Extensions.Logging;
using ProyectoCliniSalud.Business.Interfaces;
using ProyectoCliniSalud.DataAccess.Repositories;
using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IGenericRepository<Usuario> usuarioRepository,
            ILogger<AuthService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<(bool Exito, string Mensaje, Usuario? Usuario)> LoginAsync(string email, string password)
        {
            try
            {
                var query = await _usuarioRepository.ConsultarAsync(u => u.Email.ToLower() == email.ToLower());
                var usuario = query.FirstOrDefault();

                if (usuario == null)
                {
                    return (false, "Credenciales incorrectas.", null);
                }

                if (!usuario.Activo)
                {
                    return (false, "La cuenta se encuentra inactiva.", null);
                }

                if (usuario.FechaBloqueo.HasValue)
                {
                    var minutosTranscurridos = (DateTime.Now - usuario.FechaBloqueo.Value).TotalMinutes;
                    if (minutosTranscurridos < 15)
                    {
                        return (false, "La cuenta está temporalmente bloqueada por múltiples intentos fallidos.", null);
                    }
                }

                bool passwordValido = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
                if (!passwordValido)
                {
                    await RegistrarIntentoLoginAsync(usuario.IdUsuario, false);
                    return (false, "Credenciales incorrectas.", null);
                }

                await RegistrarIntentoLoginAsync(usuario.IdUsuario, true);

                return (true, "Inicio de sesión exitoso.", usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el proceso de login para el correo {Email}", email);
                throw;
            }
        }

        public async Task<bool> RegistrarIntentoLoginAsync(int idUsuario, bool exitoso)
        {
            try
            {
                var usuario = await _usuarioRepository.ObtenerPorIdAsync(idUsuario);
                if (usuario == null) return false;

                if (exitoso)
                {
                    usuario.IntentosFallidos = 0;
                    usuario.FechaBloqueo = null;
                }
                else
                {
                    usuario.IntentosFallidos++;
                    if (usuario.IntentosFallidos >= 3)
                    {
                        usuario.FechaBloqueo = DateTime.Now; 
                    }
                }

                await _usuarioRepository.ActualizarAsync(usuario);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el intento de login para el usuario ID {Id}", idUsuario);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje, int IdUsuario)> RegistrarPacientePublicoAsync(string email, string passwordPlano, int idRolPaciente = 3)
        {
            try
            {
                var existente = await _usuarioRepository.ConsultarAsync(u => u.Email.ToLower() == email.ToLower());
                if (existente.Any())
                {
                    return (false, "El correo electrónico ya se encuentra registrado.", 0);
                }

                var nuevoUsuario = new Usuario
                {
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordPlano),
                    IdRol = idRolPaciente,
                    CuentaVerificada = false,
                    Activo = true,
                    IntentosFallidos = 0,
                    FechaRegistro = DateTime.Now
                };

                await _usuarioRepository.CrearAsync(nuevoUsuario);
                return (true, "Paciente registrado correctamente.", nuevoUsuario.IdUsuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar paciente público con correo {Email}", email);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> ConfirmarCuentaAsync(string email)
        {
            try
            {
                var query = await _usuarioRepository.ConsultarAsync(u => u.Email.ToLower() == email.ToLower());
                var usuario = query.FirstOrDefault();

                if (usuario == null) return (false, "El usuario no existe.");

                usuario.CuentaVerificada = true;
                usuario.CodigoVerificacion = null;
                usuario.ExpiracionCodigo = null;

                await _usuarioRepository.ActualizarAsync(usuario);
                return (true, "Cuenta confirmada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar la cuenta del correo {Email}", email);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> GenerarCodigoRecuperacionAsync(string email)
        {
            try
            {
                var query = await _usuarioRepository.ConsultarAsync(u => u.Email.ToLower() == email.ToLower() && u.Activo);
                var usuario = query.FirstOrDefault();

                if (usuario == null) return (false, "El correo no está registrado o el usuario está inactivo.");

                string codigo = new Random().Next(100000, 999999).ToString();

                usuario.CodigoVerificacion = codigo;
                usuario.ExpiracionCodigo = DateTime.Now.AddMinutes(15); 

                await _usuarioRepository.ActualizarAsync(usuario);


                return (true, "Código de recuperación generado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar código de recuperación para {Email}", email);
                throw;
            }
        }

        public async Task<bool> ValidarCodigoRecuperacionAsync(string email, string codigoVerificacion)
        {
            try
            {
                var query = await _usuarioRepository.ConsultarAsync(u => u.Email.ToLower() == email.ToLower() && u.Activo);
                var usuario = query.FirstOrDefault();

                if (usuario == null ||
                    string.IsNullOrEmpty(usuario.CodigoVerificacion) ||
                    !usuario.ExpiracionCodigo.HasValue)
                {
                    return false;
                }

                bool esValido = usuario.CodigoVerificacion == codigoVerificacion &&
                                usuario.ExpiracionCodigo.Value >= DateTime.Now;

                return esValido;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar el código de recuperación para {Email}", email);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> RestablecerPasswordAsync(string email, string codigoVerificacion, string nuevoPasswordPlano)
        {
            try
            {
                bool codigoValido = await ValidarCodigoRecuperacionAsync(email, codigoVerificacion);
                if (!codigoValido)
                {
                    return (false, "El código es inválido o ha expirado.");
                }

                var query = await _usuarioRepository.ConsultarAsync(u => u.Email.ToLower() == email.ToLower());
                var usuario = query.FirstOrDefault();

                if (usuario == null) return (false, "El usuario no existe.");

                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevoPasswordPlano);
                usuario.CodigoVerificacion = null;
                usuario.ExpiracionCodigo = null;
                usuario.IntentosFallidos = 0;
                usuario.FechaBloqueo = null;

                await _usuarioRepository.ActualizarAsync(usuario);
                return (true, "Contraseña restablecida exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restablecer la contraseña para {Email}", email);
                throw;
            }
        }
    }
}