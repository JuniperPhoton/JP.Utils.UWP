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

        public static bool AddValue(string key, string value, bool notOverride = false)
        {
            if (notOverride)
            {
                if (HasValue(key))
                {
                    return false;
                }
            }
            LocalSettings.Values[key] = value;
            return true;
        }

        public static bool AddValue(string key, object value, bool notOverride = false)
        {
            if (notOverride)
            {
                if (HasValue(key))
                {
                    return false;
                }
            }
            LocalSettings.Values[key] = value;
            return true;
        }

        public static bool AddValue(string key, bool value, bool notOverride = false)
        {
            if (notOverride)
            {
                if (HasValue(key))
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
        /// 获取某键的值，如果没有，返回 null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (string)LocalSettings.Values[key];
            }
            return null;
        }

        /// <summary>
        /// 清空设置
        /// </summary>
        public static void CleanUpAll()
        {
            LocalSettings.Values.Clear();
        }
    }
}