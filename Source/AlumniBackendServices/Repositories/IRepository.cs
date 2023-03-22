using System.Linq.Expressions;

namespace AlumniBackendServices.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> Get(int id);
    Task<IEnumerable<TEntity>> GetAll();
    Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);

    Task Add(TEntity entity);
    Task AddRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}
