using Models.Basic;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using global::BusinessLogic.Helpers;
using global::BusinessLogic.Services.Interfaces;
using Models.Services.BingSearch;

namespace SearchEngineAPI.BusinessLogic.Services
{
    namespace BusinessLogic.Services
    {
        public class BingSearchService : ISearchService
        {
            private readonly ILogger<BingSearchService> logger;
            private readonly IConfiguration config;
            private readonly HttpClient httpClient;

            public BingSearchService(ILogger<BingSearchService> logger, IConfiguration config, HttpClient httpClient)
            {
                this.logger = logger;
                this.config = config;
                this.httpClient = httpClient;
            }

            public async Task<ServerResponse> FetchSearchResults(string tranId, string query)
            {
                var serverResponse = new ServerResponse();

                try
                {
                    var bingSettings = AppSettingsHelper.GetServiceSettings(config, "BingSearchAPI");
                    var apiKey = bingSettings.Constants.FirstOrDefault(c => c.FieldName == "apiKey")?.Value;

                    if (string.IsNullOrEmpty(apiKey))
                    {
                        throw new InvalidOperationException("Bing API key is not configured properly.");
                    }

                    var url = bingSettings.Url + $"?q={query}"; 

                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                    var httpResponse = await httpClient.GetAsync(url);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var json = await httpResponse.Content.ReadAsStringAsync();
                        var searchResults = JsonConvert.DeserializeObject<BingSearchResponse>(json); 

                        serverResponse.Data = searchResults;
                        serverResponse.IsError = false;
                    }
                    else
                    {
                        serverResponse.SetError((int)httpResponse.StatusCode, "Failed to fetch search results from Bing");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"FetchSearchResults error: tranId: {tranId}, Query: {query}, Error: {ex}");
                    serverResponse.SetError(500, $"Internal Server Error: {ex.Message}");
                }
                finally
                {
                    httpClient.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");
                }

                return serverResponse;
            }
        }
    }

}
