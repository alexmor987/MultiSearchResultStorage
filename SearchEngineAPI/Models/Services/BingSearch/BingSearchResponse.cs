namespace Models.Services.BingSearch
{
    public class BingSearchResponse
    {
        public WebPages WebPages { get; set; }
    }

    public class WebPages
    {
        public string WebSearchUrl { get; set; }
        public int TotalEstimatedMatches { get; set; }
        public List<WebPage> Value { get; set; }
    }

    public class WebPage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string DisplayUrl { get; set; }
        public string Snippet { get; set; }
        public string DateLastCrawled { get; set; }
        public string Language { get; set; }
        public bool IsFamilyFriendly { get; set; }
    }

}
