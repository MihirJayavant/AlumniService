#pragma warning disable IDE1006

using System.Linq.Expressions;
using Database;
using Microsoft.EntityFrameworkCore;

namespace AlumniBackendServices.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationContext context;

        public Repository(ApplicationContext context) => this.context = context;

        public virtual async Task Add(TEntity entity) =>
                    await context.Set<TEntity>().AddAsync(entity);


        public virtual async Task AddRange(IEnumerable<TEntity> entities) =>
                    await context.Set<TEntity>().AddRangeAsync(entities);


        public virtual async Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate) =>
                    await context.Set<TEntity>()
                                    .Where(predicate)
                                    .Take(100)
                                    .ToListAsync();


        public virtual async Task<TEntity?> Get(int id) =>
                    await context.Set<TEntity>().FindAsync(id);

        public virtual async Task<IEnumerable<TEntity>> GetAll() =>
                    await context.Set<TEntity>()
                                    .Take(100)
                                    .AsNoTracking()
                                    .ToListAsync();


        public virtual void Remove(TEntity entity) =>
                    context.Set<TEntity>().Remove(entity);
        public virtual void RemoveRange(IEnumerable<TEntity> entities) =>
                    context.Set<TEntity>().RemoveRange(entities);
    }
}
