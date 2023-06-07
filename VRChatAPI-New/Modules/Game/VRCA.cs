using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VRChatAPI_New.Exception;
using VRChatAPI_New.Models;

namespace VRChatAPI_New.Modules.Game
{
    public static class VRCA
    {
        public static async Task DownloadVrcaFile(string url, string fileLocation)
        {
            StaticGameValues.CheckSetup();
            HttpRequests.DownloadFile(url, fileLocation);
        }

        public static async Task<VRChatFileInformation> GetVersions(string url)
        {
            StaticGameValues.CheckSetup();

            string urlWanted = url.Replace(StaticGameValues.ApiUrl.AbsoluteUri, "");

            var webResponse = await HttpRequests.GetStringAsync(urlWanted).ConfigureAwait(false);
            if (webResponse.ToLower().Contains("missing credentials"))
            {
                throw new InvalidLogin("Invalid token");
            }
            return JsonConvert.DeserializeObject<VRChatFileInformation>(webResponse);
        }
    }
}
