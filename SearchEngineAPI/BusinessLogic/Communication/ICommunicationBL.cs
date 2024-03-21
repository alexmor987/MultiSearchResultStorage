using Models.Basic;

namespace BusinessLogic.Communication
{
    public interface ICommunicationBL
    {
        Task<ServerResponse> SendAsync(string tranId, string serviceName, object paramsDTO, 
            List<KeyValuePair<string, string>> queryStringParameters = null, List<KeyValuePair<string, string>> headers = null, List<KeyValuePair<string, string>> urlReplacements = null);
    }
}