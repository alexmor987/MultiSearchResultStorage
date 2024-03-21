using BusinessLogic.Communication;
using BusinessLogic.Helpers;
using BusinessLogic.Services.Interfaces;
using Models.Basic;
using Models.Services.GoogleSearch;
using Newtonsoft.Json;

namespace BusinessLogic.Services
{
    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> logger;
        private readonly ICommunicationBL communicationBL;
        private readonly IConfiguration config;
        private readonly HttpClient httpClient;

        public SearchService(ILogger<SearchService> logger, ICommunicationBL communicationBL, IConfiguration config,HttpClient httpClient)
        {
            this.logger = logger;
            this.communicationBL = communicationBL;
            this.config = config;
            this.httpClient = httpClient;
        }

        public async Task<ServerResponse> FetchSearchResults(string tranId, string query)
        {
            var serverResponse = new ServerResponse();

            try
            {
                // Fetch settings for Search API
                var googleSettings = AppSettingsHelper.GetServiceSettings(config, "GoogleSearchAPI");
                var apiKey = googleSettings.Constants.FirstOrDefault(c => c.FieldName == "key")?.Value;
                var cx = googleSettings.Constants.FirstOrDefault(c => c.FieldName == "cx")?.Value;

                var url = $"https://www.googleapis.com/customsearch/v1?key={apiKey}&cx={cx}&q={query}";


                var httpResponse = await httpClient.GetAsync(url);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var json = await httpResponse.Content.ReadAsStringAsync();
                    var searchResults = JsonConvert.DeserializeObject<GoogleSearchResponse>(json);

                    serverResponse.Data = searchResults; 
                    serverResponse.IsError = false;
                }
                else
                {
                    serverResponse.SetError((int)httpResponse.StatusCode, "Failed to fetch search results");
                }
               
            }
            catch (Exception ex)
            {
                logger.LogError($"FetchSearchResults error: tranId: {tranId}, Query: {query}, Error: {ex}");
                serverResponse.SetError(500, $"Internal Server Error: {ex.Message}");
            }

            return serverResponse;
        }

    }
}

