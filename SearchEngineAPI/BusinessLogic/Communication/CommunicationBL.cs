using BusinessLogic.Helpers;
using Helpers;
using Helpers.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Basic;
using Models.Services.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BusinessLogic.Communication
{
    public class CommunicationBL : ICommunicationBL
    {
        private readonly ILogger<CommunicationBL> logger;
        private readonly IConfiguration config;

        // private List<ServicesSettingsModel> settings = null;


        public CommunicationBL(ILogger<CommunicationBL> logger, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;

        }

        #region PUBLIC METHODS

        public async Task<ServerResponse> SendAsync(string tranId, string serviceName, object paramsDTO = null, 
                                                   List<KeyValuePair<string, string>> queryStringParameters = null,
                                                    List<KeyValuePair<string, string>> headers = null,
                                                    List<KeyValuePair<string, string>> urlReplacements = null)
        {
            ServerResponse serverResponse = new ServerResponse();

            try
            {
                logger.LogDebug($"SendAsync start. TranId: {tranId} ServiceName: {serviceName}. Params: {JsonHelper.ConverObjToJson(paramsDTO)}. QueryStringParams: {JsonHelper.ConverObjToJson(queryStringParameters)}");

                //  settings = config.GetSection("ServicesSettings").Get<ServicesSettingsModel[]>().ToList();

                var concreteServiceSettings = AppSettingsHelper.GetServiceSettings(config, serviceName);

                if (concreteServiceSettings == null)
                {
                    serverResponse.SetError(500, $"No settings defined for service :{serviceName}. TranId :{tranId}");
                    return serverResponse;
                }


                serverResponse = await ExecuteCall(tranId, concreteServiceSettings, paramsDTO, queryStringParameters, headers, urlReplacements);
            }
            catch (Exception ex)
            {
                logger.LogError($"ExecuteCall method error.TranId: {tranId} -- {ex}");
                serverResponse.SetError(500, "Internal Server Error");
            }

            return serverResponse;
        }


        #endregion PUBLIC METHODS

        #region PRIVATE METHODS

        private async Task<ServerResponse> ExecuteCall(string tranId, ServicesSettingsModel settings, object paramsDTO, 
                                                      List<KeyValuePair<string, string>> queryStringParameters,
                                                       List<KeyValuePair<string, string>> headers = null,
                                                       List<KeyValuePair<string, string>> urlReplacements = null)
        {
            ServerResponse serverResponse = new ServerResponse();

            try
            {
                string endpointUrl = GetUrl(settings.Url, queryStringParameters, settings,urlReplacements);

                using (HttpClient client = HttpClientInitializer.GetHttpClient(settings.UseCert, settings.CertName))
                {
                    var httpRequest = PrepareRequest(paramsDTO, settings.Method.ToLower(), endpointUrl, settings.BodyType.ToLower(), 
                                                     queryStringParameters, headers);

                    //Call service
                    var actionResponse = await client.SendAsync(httpRequest);

                    if (actionResponse.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        logger.LogError($"ExecuteCall (tranId:{tranId}) - Error occurred when call {endpointUrl} service. \r\n postResult: {JsonHelper.ConverObjToJson(actionResponse)}");
                        serverResponse.SetError((int)actionResponse.StatusCode, "The target API return error code");

                        return serverResponse;
                    }

                    serverResponse.Data = await actionResponse.Content.ReadAsStringAsync();

                    logger.LogDebug($"ExecuteCall -tranId:{tranId} - Proxy Response : {serverResponse.Data}");

                    return serverResponse;
                    // return PrepareResult(resultContent);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"ExecuteCall method error. tranId:{tranId} {ex}");
                serverResponse.SetError(500, "Internal Server Error");

                return serverResponse;
            }
        }

        private HttpRequestMessage PrepareRequest(object requestParams, string method, string url, string bodyType, 
                                                  List<KeyValuePair<string, string>> queryStringParameters,
                                                  List<KeyValuePair<string, string>> headers = null)
        {
            HttpContent httpContent = null;
            HttpMethod httpMethod = null;

            switch (method)
            {
                case "get":
                    httpMethod = HttpMethod.Get;
                    break;
                case "put":
                    httpMethod = HttpMethod.Put;
                    if (bodyType != "none")
                    {
                        switch (bodyType)
                        {
                            case "none":
                                httpContent = null;
                                break;
                            case "x-www-form-urlencoded":

                                if (requestParams != null)
                                    httpContent = new FormUrlEncodedContent((List<KeyValuePair<string, string>>)requestParams);
                                else
                                    httpContent = new FormUrlEncodedContent(queryStringParameters);
                                break;
                            case "raw":
                                httpContent = new StringContent(JsonConvert.SerializeObject(requestParams), Encoding.UTF8, "application/json");
                                break;
                        }
                    }
                    break;
                case "post":
                case "patch":
                    {
                        httpMethod = HttpMethod.Post;
                        if (bodyType != "none")
                        {
                            switch (bodyType)
                            {
                                case "none":
                                    httpContent = null;
                                    break;
                                case "x-www-form-urlencoded":

                                    if (requestParams != null)
                                        httpContent = new FormUrlEncodedContent((List<KeyValuePair<string, string>>)requestParams);
                                    else
                                        httpContent = new FormUrlEncodedContent(queryStringParameters);
                                    break;
                                case "raw":
                                    httpContent = new StringContent(JsonConvert.SerializeObject(requestParams), Encoding.UTF8, "application/json");
                                    break;
                            }
                        }
                        break;
                    }
                case "delete":
                    {
                        httpMethod = HttpMethod.Delete;
                        if (bodyType != "none")
                        {
                            switch (bodyType)
                            {
                                case "none":
                                    httpContent = GetEmtyContent();
                                    break;
                                case "x-www-form-urlencoded":

                                    if (requestParams != null)
                                        httpContent = new FormUrlEncodedContent((List<KeyValuePair<string, string>>)requestParams);
                                    else
                                        httpContent = new FormUrlEncodedContent(queryStringParameters);
                                    break;
                                case "raw":
                                    if (requestParams == null)
                                        httpContent = GetEmtyContent();
                                    else
                                        httpContent = new StringContent(JsonConvert.SerializeObject(requestParams), Encoding.UTF8, "application/json");
                                    break;
                            }
                        }
                        break;
                    }
                  
            }

            var request = new HttpRequestMessage(httpMethod, url) { Content = httpContent };

           

            #region HEADERS
            //request.Headers.Clear();

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    try
                    {
                       // request.Headers.Add(header.Key, header.Value);

                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"ExecuteCall: Failed to add header: {header} to thr Request. Message {ex}");

                    }

                }
            }


            #endregion

            return request;
        }

        private string GetUrl(string baseUrl, List<KeyValuePair<string, string>> queryStringParameters,
                             ServicesSettingsModel settings,
                            List<KeyValuePair<string, string>> urlReplacements = null)
        {
            if (urlReplacements != null && urlReplacements.Count > 0)
            {
                foreach (var item in urlReplacements)
                {
                    baseUrl = baseUrl.Replace(item.Key, item.Value);
                }
            }

            if (settings.BodyType == "x-www-form-urlencoded" ||
                queryStringParameters == null || 
                queryStringParameters.Count == 0)
            {
                return baseUrl;
            }
               

            string queryString = string.Empty;
            foreach (var parameter in queryStringParameters)
            {
                queryString += parameter.Key + "=" + parameter.Value + "&";
            }
            queryString = queryString.Substring(0, queryString.Length - 1);

            return baseUrl += "?" + queryString;
        }

        private StringContent GetEmtyContent()
        {
            var obj = new PseadoModel();
            return new StringContent(JsonConvert.SerializeObject(obj),
                                     Encoding.UTF8,
                                     "application/json");

        }
        #endregion PRIVATE METHODS
    }

    internal class PseadoModel
    {
        public int Prop { get; set; }
    }

}
