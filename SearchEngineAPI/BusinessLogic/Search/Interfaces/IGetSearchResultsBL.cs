using Models.Basic;

namespace BusinessLogic.Search.Interfaces
{
    public interface IGetSearchResultsBL
    {
        Task<ServerResponse> GetSearchResults(string tranId, string query);
    }
}