using DataAccess.DB.Models;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Repos
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly WebSearchArchiveContext dbContext;

        protected RepositoryBase(WebSearchArchiveContext context)
        {
            dbContext = context;
        }

        public async Task<T> FindAsync(int id)
        {
            return await dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithNoTrackingAsync()
        {
            return await dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await dbContext.Set<T>().AddAsync(entity);
        }

        public void DeleteAsync(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }

        public void UpdateAsync(T entity)
        {
            dbContext.Set<T>().Update(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await dbContext.Set<T>().AddRangeAsync(entities);
        }
    }
}
