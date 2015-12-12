using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using JP.Utils.Data.Json;
using Windows.Storage.Streams;
using Windows.Web.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using JP.Utils.Debug;
using JP.Exceptions;
using JP.Utils.Network;
using System.Threading;

namespace JP.API
{
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
                catch(Exception)
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
        public static async Task<CommonRespMsg> SendGetRequestAsync(string url,CancellationToken token)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
                return await SendRequest(request,token);
            }
            catch(APIException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(APIHelper), nameof(SendGetRequestAsync)+url);
                return new CommonRespMsg() { IsSuccessful = false, ExtraErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <typeparam name="T">返回的对象</typeparam>
        /// <param name="url">URL</param>
        /// <param name="paras">POST 参数</param>
        /// <returns></returns>
        public static async Task<CommonRespMsg> SendPostRequestAsync(string url, List<KeyValuePair<string, string>> paras,CancellationToken token)
        {
            try
            {
                paras.Add(new KeyValuePair<string, string>("device", VersionHelper.Device));
                paras.Add(new KeyValuePair<string, string>("device_type", VersionHelper.DeviceType));
                paras.Add(new KeyValuePair<string, string>("version", VersionHelper.Version));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                request.Content = new HttpFormUrlEncodedContent(paras);

                return await SendRequest(request,token);
            }
            catch(Exception ex)
            {
                return new CommonRespMsg() { IsSuccessful = false, ExtraErrorMsg = ex.Message };
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
            return await SendRequest(request,token);
        }

        private static async Task<CommonRespMsg> SendRequest(HttpRequestMessage request,CancellationToken token)
        {
            var msgToReturn = new CommonRespMsg();
            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage resp = null;
                    if(token== null)
                    {
                        resp=await client.SendRequestAsync(request);
                    }
                    else resp = await client.SendRequestAsync(request).AsTask(token);
                    resp.EnsureSuccessStatusCode();
                    var bytes = await resp.Content.ReadAsBufferAsync();
                    var content = Encoding.UTF8.GetString(bytes.ToArray());
                    msgToReturn.JsonSrc = content;

                    JsonObject resultObj;
                    var ok = JsonObject.TryParse(content, out resultObj);
                    if (ok)
                    {
                        var errorCode = JsonParser.GetStringFromJsonObj(resultObj, "errorCode");
                        var errorMsg = JsonParser.GetStringFromJsonObj(resultObj, "errorMsg");

                        if (!string.IsNullOrEmpty(errorCode))
                        {
                            if (errorCode != "0")
                            {
                                throw new APIException(int.Parse(errorCode), errorMsg);
                            }
                            msgToReturn.ErrorCode = int.Parse(errorCode);
                            msgToReturn.ErrorMsg = errorMsg;
                        }
                    }
                    resp.Dispose();
                }
            }
            catch(Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(APIHelper), nameof(SendRequest)+request.RequestUri);
                msgToReturn.IsSuccessful = false;
                msgToReturn.ExtraErrorMsg += e.Message;
                throw e;
            }
            
            return msgToReturn;
        }
    }
}
