using DataAccess.DB.Models;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories.Repos
{
    public class SearchResultRepository: RepositoryBase<SearchResults>, ISearchResultRepository
    {
        public SearchResultRepository(WebSearchArchiveContext context) : base(context)
        {
            
        }
    }
}
