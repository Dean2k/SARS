using System;
using System.Net;
using System.Net.Http;
using System.Text;
using VRChatAPI_New.Exception;

namespace VRChatAPI_New
{
    public class StaticGameValues
    {
        public static bool LoggedInOnce { get; set; }
        public static string AuthKey { get; set; }
        public static string TwoFactorKey { get; set; }
        public static string MacAddress { get; set; }
        public static string GameVersion { get; set; }
        public static string UnityVersion { get; set; }
        public static string ServerVersion { get; set; }
        public static string Store { get; set; }
        public static Uri ApiUrl { get; set; }
        public static CookieContainer CookieContainer { get; set; }
        public static HttpClient HttpClient { get; set; }

        public static StringContent JsonToHtmlContent(string json)
        {
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        internal static void CheckSetup()
        {
            if (HttpClient == null || CookieContainer == null || UnityVersion == null || GameVersion == null && ApiUrl == null)
            {
                throw new NotSetup("Please ensure you run the SetupDownloader function first.");
            }
        }
    }
}