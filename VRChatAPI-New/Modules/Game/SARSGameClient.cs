using System.Net;
using System.Net.Http;

namespace VRChatAPI_New.Modules.Game
{
    public static class SARSGameClient
    {
        public static void SetupClient()
        {
            HttpClientHandler handler = SetupClientHandler();
            StaticGameValues.HttpClient = new HttpClient(handler)
            {
                BaseAddress = StaticGameValues.ApiUrl
            };
            SetupRequestHeaders();
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

        public static void SetupRequestHeaders()
        {
            var requestHeaders = StaticGameValues.HttpClient.DefaultRequestHeaders;
            requestHeaders.Clear();
            requestHeaders.UserAgent.ParseAdd("VRC.Core.BestHTTP");
            requestHeaders.AcceptEncoding.ParseAdd("gzip, identity");
            requestHeaders.TE.ParseAdd("identity");
            requestHeaders.Host = "api.vrchat.cloud";
            requestHeaders.Add("X-MacAddress", StaticGameValues.MacAddress);
            requestHeaders.Add("X-Client-Version", StaticGameValues.GameVersion);
            requestHeaders.Add("X-Platform", "standalonewindows");
            requestHeaders.Add("X-Unity-Version", StaticGameValues.UnityVersion);
            requestHeaders.Add("X-GameServer-Version", StaticGameValues.ServerVersion);
            requestHeaders.Add("X-Store", StaticGameValues.Store);
        }
    }
}