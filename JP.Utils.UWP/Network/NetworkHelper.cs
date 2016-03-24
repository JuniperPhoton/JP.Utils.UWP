using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace JP.Utils.Network
{
    public class NetworkHelper
    {
        /// <summary>
        /// 返回SHA1加密后的字符串
        /// </summary>
        /// <returns></returns>
        public string CreateSha1(string sourceString)
        {
            IBuffer input = CryptographicBuffer.ConvertStringToBinary(sourceString, BinaryStringEncoding.Utf8);
            var hasher = HashAlgorithmProvider.OpenAlgorithm("SHA1");
            IBuffer hashed = hasher.HashData(input);

            return CryptographicBuffer.EncodeToBase64String(hashed);
        }

        /// <summary>
        /// 返回时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetRequestTime()
        {
            //DateTime timeStamp = new DateTime(1970, 1, 1);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp.ToString();
        }

        public static bool HasNetWork
        {
            get
            {
                try
                {
                    var isAvailable = false;
                    var profile = NetworkInformation.GetInternetConnectionProfile();
                    if (profile!= null)
                    {
                        isAvailable = profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
                    }
                    return isAvailable;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
