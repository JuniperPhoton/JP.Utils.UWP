using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JP.Utils.Data
{
    public class LocalSettingHelper
    {
        private static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        /// <summary>
        /// 检查是否有某Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasValue(string key)
        {
            return LocalSettings.Values.ContainsKey(key);
        }

        /// <summary>
        /// 添加键值，如果存在键则更新值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static bool AddValue(string key, string value,bool isCheckExist=false)
        {
            if(isCheckExist)
            {
                if(LocalSettings.Values.ContainsKey(key))
                {
                    return false;
                }
            }
            LocalSettings.Values[key] = value;
            return true;
        }

        public static bool AddValue(string key, bool value,bool isCheckExist=false)
        {
            if (isCheckExist)
            {
                if (LocalSettings.Values.ContainsKey(key))
                {
                    return false;
                }
            }
            LocalSettings.Values[key] = value;
            return true;
        }

        /// <summary>
        /// 删除键值
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteValue(string key)
        {
            LocalSettings.Values.Remove(key);
        }

        /// <summary>
        /// 获取某键的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (string)LocalSettings.Values[key];
            }
            return default(string);
        }

        public static void CleanUpAll()
        {
            LocalSettings.Values.Clear();
        }
    }
}
