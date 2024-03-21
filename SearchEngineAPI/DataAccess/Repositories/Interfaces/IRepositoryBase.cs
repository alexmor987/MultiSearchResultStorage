namespace DataAccess.Repositories.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T> FindAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllWithNoTrackingAsync();

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void DeleteAsync(T entity);
        void UpdateAsync(T entity);
    }
}
