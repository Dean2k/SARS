using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRChatAPI_New.Exception;

namespace VRChatAPI_New.Modules.Game
{
    public static class HttpRequests
    {
        public static async Task<string> GetStringAsync(string url)
        {
            try
            {
                using var webResponse = await StaticGameValues.HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                using var httpStream = await webResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using var streamReader = new StreamReader(httpStream);
                return await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        public static async Task<string> PostAsync(string url, HttpContent content)
        {
            try
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
                using var webResponse = await StaticGameValues.HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                if (!webResponse.IsSuccessStatusCode)
                {
                    throw new PostError($"Error posting data to {StaticGameValues.HttpClient.BaseAddress}{url}");
                }
                using var httpStream = await webResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using var streamReader = new StreamReader(httpStream);
                return await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        public static async Task<string> PutAsync(string url, HttpContent content)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, url) { Content = content };
                using var response = await StaticGameValues.HttpClient.SendAsync(request, CancellationToken.None).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    throw new PostError($"Error putting data to {StaticGameValues.HttpClient.BaseAddress}{url}");
                }
                response.EnsureSuccessStatusCode();
                using var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using var streamReader = new StreamReader(contentStream);
                return await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}