using BusinessLogic.Services.Interfaces;
using Models.Basic;

namespace BusinessLogic.Services
{
    public class SearchServiceMock : ISearchService
    {
        public async Task<ServerResponse> FetchSearchResults(string tranId, string query)
        {
            var serverResponse = new ServerResponse();

            var googleSearchTask = MockGoogleSearch(query);
            var bingSearchTask = MockBingSearch(query);

            await Task.WhenAll(googleSearchTask, bingSearchTask);

            var mockResults = new List<string>();
            mockResults.AddRange(googleSearchTask.Result);
            mockResults.AddRange(bingSearchTask.Result);

            serverResponse.Data = mockResults;
            serverResponse.IsError = false;

            return serverResponse;
        }

        private Task<List<string>> MockGoogleSearch(string query)
        {
            var results = new List<string>();
            if (query.ToLower().Contains("google"))
            {
                results.Add("Google Search Result 1");
                results.Add("Google Search Result 2");
                results.Add("Google Search Result 3");
            }
            return Task.FromResult(results);
        }

        private Task<List<string>> MockBingSearch(string query)
        {
            var results = new List<string>();
            if (query.ToLower().Contains("bing"))
            {
                results.Add("Bing Search Result 1");
                results.Add("Bing Search Result 2");
                results.Add("Bing Search Result 3");
            }
            return Task.FromResult(results);
        }
    }
}
