using System.Net;
using System.Net.Http;

namespace VRChatAPI_New.Modules.Game
{
    public static class SARSGameClient
    {

        public static void SetupClient(string macAddress, string gameVersion, string unityVersion)
        {
            HttpClientHandler handler = SetupClientHandler();
            StaticGameValues.HttpClient = new HttpClient(handler)
            {
                BaseAddress = StaticGameValues.ApiUrl
            };
            SetupRequestHeaders(macAddress, gameVersion, unityVersion);
        }

        public static HttpClientHandler SetupClientHandler()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = true,
                CookieContainer = StaticGameValues.CookieContainer,
                UseProxy = false
            };
            return httpClientHandler;
        }

        public static void SetupRequestHeaders(string macAddress, string gameVersion, string unityVersion)
        {
            var requestHeaders = StaticGameValues.HttpClient.DefaultRequestHeaders;
            requestHeaders.Clear();
            
            requestHeaders.AcceptEncoding.ParseAdd("identity");
            requestHeaders.TE.ParseAdd("identity");
            requestHeaders.Host = "api.vrchat.cloud";
            requestHeaders.Add("X-MacAddress", macAddress);
            requestHeaders.Add("X-Client-Version", gameVersion);          
            requestHeaders.Add("X-Platform", "standalonewindows");           
            requestHeaders.UserAgent.ParseAdd("VRC.Core.BestHTTP");
            requestHeaders.Add("X-Unity-Version", unityVersion);
        }
    }
}