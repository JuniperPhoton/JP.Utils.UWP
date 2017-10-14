using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;

namespace JP.Utils.Application
{
    public class ApplicationPackageManager
    {
        public static async Task<BackgroundAccessStatus> CheckAppVersion()
        {
            String appVersion = String.Format("{0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Revision);

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["AppVersion"] as string != appVersion)
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["AppVersion"] = appVersion;
                BackgroundExecutionManager.RemoveAccess();
            }

            return await BackgroundExecutionManager.RequestAccessAsync();
        }
    }
}