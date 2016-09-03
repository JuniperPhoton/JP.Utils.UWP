using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace JP.Utils.Network
{
    public static class FileDownloadUtil
    {
        /// <summary>
        /// 根据 URL 获取流
        /// </summary>
        /// <param name="url">URL 地址</param>
        /// <returns>返回IRandomAccessStream</returns>
        public static async Task<IRandomAccessStream> GetIRandomAccessStreamFromUrlAsync(string url, 
            CancellationToken? token)
        {
            if (string.IsNullOrEmpty(url)) throw new UriFormatException("The url is null or empty.");

            using (HttpClient client = new HttpClient())
            {
                if (token == null) token = CTSFactory.MakeCTS().Token;

                var downloadTask = client.GetAsync(new Uri(url), token.Value);

                token?.ThrowIfCancellationRequested();

                var response = await downloadTask;
                response.EnsureSuccessStatusCode();

                var streamTask = response.Content.ReadAsStreamAsync();

                token?.ThrowIfCancellationRequested();

                var stream = await streamTask;

                return stream.AsRandomAccessStream();
            }
        }

        public static async Task<IRandomAccessStream> GetIRandomAccessStreamFromUrlAsync(string url)
        {
            return await GetIRandomAccessStreamFromUrlAsync(url, null);
        }
    }
}
