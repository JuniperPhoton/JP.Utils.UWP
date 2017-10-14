using JP.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace JP.Utils.UWP.Network
{
    [Obsolete("Please use FileDownloadUtil instead.")]
    public static class APIHelper
    {
        /// <summary>
        /// 根据 URL 获取流
        /// </summary>
        /// <param name="url">URL 地址</param>
        /// <returns>返回IRandomAccessStream</returns>
        public static async Task<IRandomAccessStream> GetIRandomAccessStreamFromUrlAsync(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new UriFormatException();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(new Uri(url));
                    response.EnsureSuccessStatusCode();

                    var buffer = await response.Content.ReadAsBufferAsync();
                    var streamImage = buffer.AsStream();

                    return streamImage.AsRandomAccessStream();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 发送 Get 请求
        /// </summary>
        /// <typeparam name="T">返回的对象</typeparam>
        /// <param name="url">URL</param>
        /// <returns></returns>
        public static async Task<CommonRespMsg> SendGetRequestAsync(string url, CancellationToken token)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
                return await SendRequest(request, token);
            }
            catch (TaskCanceledException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                return new CommonRespMsg() { ResponseCode = 400, RequestErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <typeparam name="T">返回的对象</typeparam>
        /// <param name="url">URL</param>
        /// <param name="paras">POST 参数</param>
        /// <returns></returns>
        public static async Task<CommonRespMsg> SendPostRequestAsync(string url, List<KeyValuePair<string, string>> paras, CancellationToken token)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                request.Content = new HttpFormUrlEncodedContent(paras);

                return await SendRequest(request, token);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                return new CommonRespMsg() { ResponseCode = 400, RequestErrorMsg = ex.Message };
            }
        }

        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <typeparam name="T">返回的对象</typeparam>
        /// <param name="url">URL</param>
        /// <param name="paras">POST 参数</param>
        /// <returns></returns>
        public static async Task<CommonRespMsg> SendPostRequestAsync(string url, string paramsInRaw, CancellationToken token)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
            request.Content = new HttpStringContent(paramsInRaw);
            return await SendRequest(request, token);
        }

        public static async Task<CommonRespMsg> SendRequest(HttpRequestMessage request, CancellationToken token)
        {
            var msgToReturn = new CommonRespMsg();
            try
            {
                using (var client = new HttpClient())
                {
                    //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; U; Android 5.1; zh-cn; XT1085 Build/LPE23.32-53) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1");
                    HttpResponseMessage resp = null;
                    if (token == null)
                    {
                        resp = await client.SendRequestAsync(request);
                    }
                    else resp = await client.SendRequestAsync(request).AsTask(token);
                    resp.EnsureSuccessStatusCode();

                    var bytes = await resp.Content.ReadAsBufferAsync();
                    var content = Encoding.UTF8.GetString(bytes.ToArray());
                    msgToReturn.JsonSrc = content;

                    resp.Dispose();
                }
            }
            catch (TaskCanceledException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                msgToReturn.ResponseCode = 400;
                msgToReturn.RequestErrorMsg += e.Message;
            }
            return msgToReturn;
        }
    }
}