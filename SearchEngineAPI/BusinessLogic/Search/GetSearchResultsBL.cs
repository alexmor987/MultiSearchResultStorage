using BusinessLogic.Search.Interfaces;
using BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Basic;
using DataAccess.Repositories.RepositoryContainer;
using Models.Services.GoogleSearch;
using DataAccess.DB.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BusinessLogic.Search
{
    public class GetSearchResultsBL : IGetSearchResultsBL
    {
        private readonly ILogger<GetSearchResultsBL> logger;
        private readonly ISearchService searchService;
        private readonly IRepositoryContainer repositoryContainer;
        private readonly IDistributedCache cache;


        public GetSearchResultsBL(ILogger<GetSearchResultsBL> logger, ISearchService searchService, IRepositoryContainer repositoryContainer, IDistributedCache cache)
        {
            this.logger = logger;
            this.searchService = searchService;
            this.repositoryContainer = repositoryContainer;
            this.cache = cache;
        }

        public async Task<ServerResponse> GetSearchResults(string tranId, string query)
        {
            ServerResponse serverResponse = new ServerResponse();
            string cacheKey = $"SearchResults-{query}";
            try
            {
                string cachedResponse = await cache.GetStringAsync(cacheKey);
                if (cachedResponse != null)
                {
                    serverResponse.Data = JsonConvert.DeserializeObject<List<string>>(cachedResponse);
                    return serverResponse;
                }
                var serviceResult = await searchService.FetchSearchResults(tranId, query);

                if (serviceResult.IsError)
                {
                    serverResponse.SetError(serviceResult.ErrorCode);
                }
                else
                {
                    var searchResults = serviceResult.Data as GoogleSearchResponse; 
                    if (searchResults != null && searchResults.Items != null)
                    {
                        // Take the top 5 titles
                        var topTitles = searchResults.Items.Take(5).Select(item => item.Title).ToList();

                        // Store in the database
                        foreach (var title in topTitles)
                        {
                            var searchResultRecord = new SearchResults
                            {
                                SearchEngine = "Google", 
                                Title = title,
                                Entereddate = DateTime.UtcNow
                            };

                            await repositoryContainer.SearchResults.AddAsync(searchResultRecord);
                            await repositoryContainer.SaveAsync();
                        }

                        var cacheOptions = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                        await cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(topTitles), cacheOptions);

                        serverResponse.Data = topTitles;
                    }
                    else
                    {
                        serverResponse.SetError(204, "No search results found.");
                    }
                }

                return serverResponse;
            }
            catch (Exception ex)
            {
                serverResponse.SetError(500);
                logger.LogError($"GetSearchResults ({tranId}) error. {ex.ToString()}");

                return serverResponse;
            }
        }
    }
}
