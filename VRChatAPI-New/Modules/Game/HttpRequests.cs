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

        public static async Task StartDownload(string fileUrl, string fileLocation, ProgressBar progressBar)
        {
            string urlWanted = fileUrl.Replace(StaticGameValues.ApiUrl.AbsoluteUri, "");
            using var response = await StaticGameValues.HttpClient.GetAsync(urlWanted, HttpCompletionOption.ResponseHeadersRead);
            await DownloadFileFromHttpResponseMessage(response, fileLocation, progressBar);
        }

        private static async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response, string fileLocation, ProgressBar progressBar)
        {
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength;

            using var contentStream = await response.Content.ReadAsStreamAsync();
            await ProcessContentStream(totalBytes, contentStream, fileLocation, progressBar);
        }

        private static void SafeProgress(ProgressBar progress, int value)
        {
            try
            {
                if (progress.InvokeRequired)
                {
                    progress.Invoke((MethodInvoker)delegate
                    {
                        progress.Value = value;
                    });
                }
            } catch { }
        }

        private static async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream, string fileLocation, ProgressBar progressBar)
        {
            var totalBytesRead = 0L;
            var readCount = 0L;
            var buffer = new byte[1024];
            var isMoreToRead = true;

            using var fileStream = new FileStream(fileLocation, FileMode.Create, FileAccess.Write, FileShare.None);
            do
            {
                var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    isMoreToRead = false;
                    SafeProgress(progressBar, Convert.ToInt32(Math.Round(totalBytesRead / (double)totalDownloadSize * 100)));
                    continue;
                }

                await fileStream.WriteAsync(buffer, 0, bytesRead);

                totalBytesRead += bytesRead;
                readCount += 1;

                if (readCount % 100 == 0)
                {
                    SafeProgress(progressBar, Convert.ToInt32(Math.Round(totalBytesRead / (double)totalDownloadSize * 100)));
                }
            }
            while (isMoreToRead);
        }
    }
}