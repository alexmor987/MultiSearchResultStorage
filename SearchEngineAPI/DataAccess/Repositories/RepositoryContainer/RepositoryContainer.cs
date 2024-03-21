using DataAccess.DB.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.Repositories.Repos;

namespace DataAccess.Repositories.RepositoryContainer
{
    public class RepositoryContainer : IRepositoryContainer
    {
        private WebSearchArchiveContext dbContext;
       
        private ISearchResultRepository searchResultRepository;
        public RepositoryContainer(WebSearchArchiveContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ISearchResultRepository SearchResults
        {
            get
            {
                if (searchResultRepository == null)
                {
                    searchResultRepository = new SearchResultRepository(dbContext);
                }
                return searchResultRepository;
            }
        }

        public Task SaveAsync()
        {
            return dbContext.SaveChangesAsync();
        }
    }
}
