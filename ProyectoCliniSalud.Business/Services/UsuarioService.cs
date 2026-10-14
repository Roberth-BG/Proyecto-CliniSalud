using Microsoft.Extensions.Logging;
using ProyectoCliniSalud.Business.Interfaces;
using ProyectoCliniSalud.DataAccess.Repositories;
using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IGenericRepository<Rol> _rolRepository;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(
            IGenericRepository<Usuario> usuarioRepository,
            IGenericRepository<Rol> rolRepository,
            ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _rolRepository = rolRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Usuario>> ObtenerUsuariosAsync()
        {
            try
            {
                var query = await _usuarioRepository.ConsultarAsync();
                return query.OrderBy(u => u.IdUsuario).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los usuarios.");
                throw;
            }
        }

        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
        {
            return await _usuarioRepository.ObtenerPorIdAsync(id);
        }

        public async Task<Usuario?> ObtenerUsuarioPorCorreoAsync(string email)
        {
            var query = await _usuarioRepository.ConsultarAsync(u => u.Email.ToLower() == email.ToLower());
            return query.FirstOrDefault();
        }

        public async Task<IEnumerable<Usuario>> BuscarUsuariosAsync(string? textoBusqueda, int? idRol, bool soloActivos)
        {
            try
            {
                var query = await _usuarioRepository.ConsultarAsync();

                if (!string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    string textoLimpio = textoBusqueda.Trim().ToLower();
                    query = query.Where(u => u.Email.ToLower().Contains(textoLimpio));
                }

                if (idRol.HasValue && idRol.Value > 0)
                {
                    query = query.Where(u => u.IdRol == idRol.Value);
                }

                if (soloActivos)
                {
                    query = query.Where(u => u.Activo);
                }

                return query.OrderBy(u => u.IdUsuario).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar los usuarios.");
                throw;
            }
        }

        public async Task<(IEnumerable<Usuario> Usuarios, int TotalRegistros)> ObtenerUsuariosPaginadosAsync(int pagina, int tamanoPagina = 10)
        {
            try
            {
                var query = await _usuarioRepository.ConsultarAsync();

                int totalRegistros = query.Count();

                var usuariosPaginados = query
                    .OrderBy(u => u.IdUsuario)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return (usuariosPaginados, totalRegistros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al paginar los usuarios.");
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> CrearUsuarioAsync(Usuario usuario, string passwordPlano)
        {
            try
            {
                var correoExistente = await _usuarioRepository.ConsultarAsync(u => u.Email.ToLower() == usuario.Email.ToLower());
                if (correoExistente.Any())
                {
                    return (false, "El correo electrónico ya se encuentra registrado por otro usuario.");
                }

                var rolExiste = await _rolRepository.ObtenerPorIdAsync(usuario.IdRol);
                if (rolExiste == null || !rolExiste.Activo)
                {
                    return (false, "El rol seleccionado no es válido o se encuentra inactivo.");
                }

                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordPlano);
                usuario.FechaRegistro = DateTime.Now;
                usuario.Activo = true;
                usuario.CuentaVerificada = false;
                usuario.IntentosFallidos = 0;

                await _usuarioRepository.CrearAsync(usuario);
                _logger.LogInformation("Usuario creado exitosamente: {Email}", usuario.Email);
                return (true, "Usuario registrado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar crear el usuario {Email}", usuario.Email);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarUsuarioAsync(Usuario usuario)
        {
            try
            {
                var correoDuplicado = await _usuarioRepository.ConsultarAsync(u => u.Email.ToLower() == usuario.Email.ToLower() && u.IdUsuario != usuario.IdUsuario);
                if (correoDuplicado.Any())
                {
                    return (false, "El correo electrónico ya está en uso por otro usuario.");
                }

                var usuarioActual = await _usuarioRepository.ObtenerPorIdAsync(usuario.IdUsuario);
                if (usuarioActual == null) return (false, "El usuario no existe.");

                usuarioActual.Email = usuario.Email;
                usuarioActual.IdRol = usuario.IdRol;
                usuarioActual.Activo = usuario.Activo;

                var resultado = await _usuarioRepository.ActualizarAsync(usuarioActual);
                if (!resultado) return (false, "No se pudo actualizar la información del usuario.");

                return (true, "Usuario actualizado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario ID {Id}", usuario.IdUsuario);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> CambiarPasswordAsync(int idUsuario, string passwordActual, string nuevoPassword)
        {
            try
            {
                var usuario = await _usuarioRepository.ObtenerPorIdAsync(idUsuario);
                if (usuario == null) return (false, "El usuario no existe.");

                bool passwordValido = BCrypt.Net.BCrypt.Verify(passwordActual, usuario.PasswordHash);
                if (!passwordValido)
                {
                    return (false, "La contraseña actual es incorrecta.");
                }

                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevoPassword);
                await _usuarioRepository.ActualizarAsync(usuario);

                return (true, "Contraseña actualizada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar la contraseña del usuario ID {Id}", idUsuario);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> DesactivarUsuarioAsync(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
                if (usuario == null) return (false, "El usuario no existe.");

                usuario.Activo = false;
                await _usuarioRepository.ActualizarAsync(usuario);

                return (true, "Usuario desactivado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar el usuario ID {Id}", id);
                throw;
            }
        }
    }
}