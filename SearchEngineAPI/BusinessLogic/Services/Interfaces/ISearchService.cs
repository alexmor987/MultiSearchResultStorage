using Models.Basic;

namespace BusinessLogic.Services.Interfaces
{
    public interface ISearchService
    {
        /// <summary>
        /// Fetch search results for a given query from external search services.
        /// </summary>
        /// <param name="tranId">Transaction ID for logging and tracking purposes.</param>
        /// <param name="query">The search query.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a ServerResponse object with the search results.</returns>
        Task<ServerResponse> FetchSearchResults(string tranId, string query);
    }
}
