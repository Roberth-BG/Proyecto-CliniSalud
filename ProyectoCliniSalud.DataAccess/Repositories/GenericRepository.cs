using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using ProyectoCliniSalud.DataAccess.Data;
using System.Linq.Expressions;

namespace ProyectoCliniSalud.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly CliniSaludDbContext _context;
        private readonly ILogger<GenericRepository<T>> _logger; 

        public GenericRepository(CliniSaludDbContext context, ILogger<GenericRepository<T>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<T>> ObtenerTodosAsync()
        {
            try
            {
                _logger.LogInformation("Consultando todos los registros de {Entity}", typeof(T).Name);
                return await _context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de {Entity}", typeof(T).Name);
                throw;
            }
        }

        public async Task<T?> ObtenerPorIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> CrearAsync(T entidad)
        {
            try
            {
                await _context.Set<T>().AddAsync(entidad);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Se creó un nuevo registro en {Entity}", typeof(T).Name);
                return entidad;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un registro en {Entity}", typeof(T).Name);
                throw;
            }
        }

        public async Task<bool> ActualizarAsync(T entidad)
        {
            try
            {
                _context.Set<T>().Update(entidad);
                var filasAfectadas = await _context.SaveChangesAsync();
                _logger.LogInformation("Se actualizó un registro en {Entity}", typeof(T).Name);
                return filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar un registro en {Entity}", typeof(T).Name);
                throw;
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var entidad = await ObtenerPorIdAsync(id);
            if (entidad == null) return false;

            try
            {
                _context.Set<T>().Remove(entidad);
                var filasAfectadas = await _context.SaveChangesAsync();
                _logger.LogInformation("Se eliminó un registro en {Entity} con ID {Id}", typeof(T).Name, id);
                return filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar un registro en {Entity} con ID {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public async Task<IQueryable<T>> ConsultarAsync(Expression<Func<T, bool>>? filtro = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (filtro != null)
            {
                query = query.Where(filtro);
            }
            return await Task.FromResult(query);
        }
    }
}