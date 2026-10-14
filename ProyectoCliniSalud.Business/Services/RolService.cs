using Microsoft.Extensions.Logging;
using ProyectoCliniSalud.Business.Interfaces;
using ProyectoCliniSalud.DataAccess.Repositories;
using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.Business.Services
{
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> _rolRepository;
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly ILogger<RolService> _logger;

        public RolService(
            IGenericRepository<Rol> rolRepository,
            IGenericRepository<Usuario> usuarioRepository,
            ILogger<RolService> logger)
        {
            _rolRepository = rolRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Rol>> ObtenerRolesAsync()
        {
            try
            {
                var query = await _rolRepository.ConsultarAsync();
                return query.OrderBy(r => r.IdRol).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los roles.");
                throw;
            }
        }

        public async Task<Rol?> ObtenerRolPorIdAsync(int id)
        {
            return await _rolRepository.ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<Rol>> BuscarRolesAsync(string? textoBusqueda, bool soloActivos)
        {
            try
            {
                var query = await _rolRepository.ConsultarAsync();

                if (!string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    string textoLimpio = textoBusqueda.Trim().ToLower();
                    query = query.Where(r => r.NombreRol.ToLower().Contains(textoLimpio));
                }

                if (soloActivos)
                {
                    query = query.Where(r => r.Activo);
                }

                return query.OrderBy(r => r.IdRol).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar los roles.");
                throw;
            }
        }

        public async Task<(IEnumerable<Rol> Roles, int TotalRegistros)> ObtenerRolesPaginadosAsync(int pagina, int tamanoPagina = 10)
        {
            try
            {
                var query = await _rolRepository.ConsultarAsync();

                int totalRegistros = query.Count();

                var rolesPaginados = query
                    .OrderBy(r => r.IdRol)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return (rolesPaginados, totalRegistros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al paginar los roles.");
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> CrearRolAsync(Rol rol)
        {
            try
            {
                var rolesExistentes = await _rolRepository.ConsultarAsync(r => r.NombreRol.ToLower() == rol.NombreRol.ToLower());
                if (rolesExistentes.Any())
                {
                    return (false, "El nombre del rol ya se encuentra registrado.");
                }

                await _rolRepository.CrearAsync(rol);
                _logger.LogInformation("Rol creado exitosamente: {NombreRol}", rol.NombreRol);
                return (true, "Rol creado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar crear el rol {NombreRol}", rol.NombreRol);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarRolAsync(Rol rol)
        {
            try
            {
                var rolesExistentes = await _rolRepository.ConsultarAsync(r => r.NombreRol.ToLower() == rol.NombreRol.ToLower() && r.IdRol != rol.IdRol);
                if (rolesExistentes.Any())
                {
                    return (false, "Ya existe otro rol con ese mismo nombre.");
                }

                var resultado = await _rolRepository.ActualizarAsync(rol);
                if (!resultado) return (false, "No se pudo actualizar el rol.");

                return (true, "Rol actualizado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol ID {IdRol}", rol.IdRol);
                throw;
            }
        }

        public async Task<(bool Exito, string Mensaje)> DesactivarRolAsync(int id)
        {
            try
            {
                var usuariosActivosConRol = await _usuarioRepository.ConsultarAsync(u => u.IdRol == id && u.Activo);
                if (usuariosActivosConRol.Any())
                {
                    return (false, "No se puede desactivar el rol porque existen usuarios activos vinculados a él.");
                }

                var rol = await _rolRepository.ObtenerPorIdAsync(id);
                if (rol == null) return (false, "El rol no existe.");

                rol.Activo = false;
                await _rolRepository.ActualizarAsync(rol);

                return (true, "Rol desactivado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar el rol ID {Id}", id);
                throw;
            }
        }
    }
}