using System.Linq.Expressions;

namespace ProyectoCliniSalud.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> ObtenerTodosAsync();
        Task<T?> ObtenerPorIdAsync(int id);
        Task<T> CrearAsync(T entidad);
        Task<bool> ActualizarAsync(T entidad);
        Task<bool> EliminarAsync(int id);
        Task<IQueryable<T>> ConsultarAsync(Expression<Func<T, bool>>? filtro = null);
    }
}