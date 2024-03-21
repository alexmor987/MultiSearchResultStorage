using Models.Basic;

namespace Models.Search
{
    public class GetSearchResultsResponse : BaseResult
    {
        public GetSearchResultsModel Data { get; set; }
    }

    public class GetSearchResultsModel
    {
        public string Title { get; set; }
    }
    
}
