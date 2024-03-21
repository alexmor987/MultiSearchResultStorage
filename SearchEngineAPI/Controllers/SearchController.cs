using Microsoft.AspNetCore.Mvc;
using Models.Basic;
using BusinessLogic.Search.Interfaces;
using Models.Search;

namespace SearchEngineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase 
    {
        private readonly ILogger<SearchController> logger;
        private readonly IGetSearchResultsBL getSearchResultsBL;
        private string tranId = string.Empty;

        public SearchController(ILogger<SearchController> logger, IGetSearchResultsBL getSearchResultsBL)
        {
            this.logger = logger;
            this.getSearchResultsBL = getSearchResultsBL;
        }

        [HttpPost]
        [Route("GetSearchResults")]
        public async Task<ActionResult<ServerResponse>> GetSearchResults([FromBody] SearchRequest requestData)
        {
            logger.LogDebug($"GetSearchResults Controller Start");
            ServerResponse serverResponse = new ServerResponse();

            try
            {
                tranId = HttpContext.Request.Headers["tranid"].FirstOrDefault();

                serverResponse = await getSearchResultsBL.GetSearchResults(tranId, requestData.Query);

                return Ok(serverResponse);
            }
            catch (Exception ex)
            {
                serverResponse.SetError(500);

                logger.LogError($"GetSearchResults Controller Error. TranId: {tranId} - {ex.ToString()}");

                return Ok(serverResponse);
            }
            finally
            {
                logger.LogDebug($"GetSearchResults Controller End. TranId: {tranId}");
            }
        }
    }
}
