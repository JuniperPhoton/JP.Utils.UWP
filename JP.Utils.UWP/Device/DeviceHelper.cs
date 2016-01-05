using Windows.ApplicationModel.Resources.Core;
using Windows.System.Profile;

namespace JP.Utils.Helper
{
    public static class DeviceHelper
    {
        /// <summary>
        /// 获取程序当前运行环境是否是桌面环境。
        /// </summary>
        public static bool IsDesktop
        {
            get
            {
                var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                return qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Desktop";
            }
        }

        /// <summary>
        /// 获取程序当前运行环境是否是移动设备环境。
        /// </summary>
        public static bool IsMobile
        {
            get
            {
                var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                return qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile";
            }
        }

        private static string[] GetDeviceOsVersion()
        {
            string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(sv);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            return new string[] { v1.ToString(),v2.ToString(),v3.ToString(),v4.ToString()};
        }

        public static string OSVersion
        {
            get
            {
                var versions = GetDeviceOsVersion();
                return versions.ToString();
            }
        }

        public static bool IsTH1OS
        {
            get
            {
                var versions = GetDeviceOsVersion();
                return versions[2] == "10240" ? true : false;
            }
        }

        public static bool IsTH2OS
        {
            get
            {
                var versions = GetDeviceOsVersion();
                return versions[2] == "10586" ? true : false;
            }
        }
    }
}
