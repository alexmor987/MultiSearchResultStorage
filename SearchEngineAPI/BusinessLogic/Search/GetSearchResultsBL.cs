using BusinessLogic.Search.Interfaces;
using BusinessLogic.Services.Interfaces;
using Models.Basic;
using DataAccess.Repositories.RepositoryContainer;
using DataAccess.DB.Models;
using Microsoft.Extensions.Caching.Distributed;
using BusinessLogic.Services;
using SearchEngineAPI.BusinessLogic.Services.BusinessLogic.Services;
using Models.Services.BingSearch;
using Models.Services.GoogleSearch;
using Newtonsoft.Json;

namespace BusinessLogic.Search
{
    public class GetSearchResultsBL : IGetSearchResultsBL
    {
        private readonly ILogger<GetSearchResultsBL> logger;
        private readonly GoogleSearchService googleSearchService;
        private readonly BingSearchService bingSearchService;
        private readonly IRepositoryContainer repositoryContainer;
        private readonly IDistributedCache cache;

        public GetSearchResultsBL(
            ILogger<GetSearchResultsBL> logger,
            GoogleSearchService googleSearchService,
            BingSearchService bingSearchService,
            IRepositoryContainer repositoryContainer,
            IDistributedCache cache)
        {
            this.logger = logger;
            this.googleSearchService = googleSearchService;
            this.bingSearchService = bingSearchService;
            this.repositoryContainer = repositoryContainer;
            this.cache = cache;
        }

        public async Task<ServerResponse> GetSearchResults(string tranId, string query)
        {
            ServerResponse serverResponse = new ServerResponse();
            try
            {
                // Perform Google and Bing searches in parallel
                var googleSearchTask = PerformSearch(googleSearchService, tranId, query, "Google");
                var bingSearchTask = PerformSearch(bingSearchService, tranId, query, "Bing");

                await Task.WhenAll(googleSearchTask, bingSearchTask);

                // Combine results from Google and Bing, avoiding duplicates
                var combinedResults = googleSearchTask.Result.Union(bingSearchTask.Result).ToList();

                serverResponse.Data = combinedResults;

                return serverResponse;
            }
            catch (Exception ex)
            {
                serverResponse.SetError(500);
                logger.LogError($"GetSearchResults ({tranId}) error. {ex.ToString()}");

                return serverResponse;
            }
        }

        private async Task<List<string>> PerformSearch(ISearchService searchService, string tranId, string query, string searchEngine)
        {
            List<string> results = new List<string>();
            string cacheKey = $"SearchResults-{searchEngine}-{query}";
            string cachedResponse = await cache.GetStringAsync(cacheKey);
            if (cachedResponse != null)
            {
                return JsonConvert.DeserializeObject<List<string>>(cachedResponse);
            }

            var serviceResult = await searchService.FetchSearchResults(tranId, query);
            if (!serviceResult.IsError && serviceResult.Data != null)
            {
                if (serviceResult.Data is BingSearchResponse bingResponse)
                {
                    results = bingResponse.WebPages.Value.Select(sr => sr.Name).Take(5).ToList();
                }
                else if (serviceResult.Data is GoogleSearchResponse googleResponse)
                {

                    results = googleResponse.Items.Select(sr => sr.Title).Take(5).ToList();
                }
                foreach (var title in results)
                {
                    var searchResultRecord = new SearchResults
                    {
                        SearchEngine = searchEngine,
                        Title = title,
                        Entereddate = DateTime.UtcNow
                    };

                    await repositoryContainer.SearchResults.AddAsync(searchResultRecord);
                    await repositoryContainer.SaveAsync();
                }


                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                await cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(results), cacheOptions);
            }

            return results;
        }
    }
}
