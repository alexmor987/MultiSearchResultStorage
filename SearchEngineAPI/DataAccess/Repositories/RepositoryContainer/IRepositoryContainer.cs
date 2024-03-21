using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories.RepositoryContainer
{
    /// <summary>
    /// List here all public Repositories
    /// </summary>
    public interface IRepositoryContainer
    {

        ISearchResultRepository SearchResults { get; }
       
        Task SaveAsync();
    }
}
