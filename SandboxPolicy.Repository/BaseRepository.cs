using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace SandboxPolicy.Repository
{
    public abstract class BaseRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly IDbSet<T> _dbSet;

        protected BaseRepository(DbContext dbContext)
        {
            _context = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public virtual T Insert(T entity)
        {
            _dbSet.Add(entity);
            return entity;
        }

        public virtual T Update(T entity)
        {
            var entry = _context.Entry(entity);
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
            return entity;
        }

        public virtual void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);

            _dbSet.Remove(entity);
        }

        public virtual bool Contains(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Any(filter);
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Where(filter).AsQueryable();
        }
    }
}
