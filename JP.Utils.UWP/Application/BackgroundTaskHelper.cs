using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace JP.Utils.Application
{
    public enum OSKind
    {
        Windows,
        WindowsPhone
    }
    public class BackgroundTaskHelper
    {
        public static void UnRegisterBackgroundTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                }
            }
        }

        public static async void RegisterBackgroundTask(OSKind kind)
        {
            try
            {
                BackgroundAccessStatus status = BackgroundAccessStatus.Unspecified;
                if (kind == OSKind.WindowsPhone)
                {
                    status = await ApplicationPackageManager.CheckAppVersion();
                }
                else if (kind == OSKind.Windows)
                {
                    status = await BackgroundExecutionManager.RequestAccessAsync();
                }

                if (status == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                    status == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
                {
                    foreach (var task in BackgroundTaskRegistration.AllTasks)
                    {
                        if (task.Value.Name == taskName)
                        {
                            task.Value.Unregister(true);
                        }
                    }

                    BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                    taskBuilder.Name = taskName;
                    taskBuilder.TaskEntryPoint = (kind == OSKind.Windows) ? taskEntryPointWindows : taskEntryPointWindowsPhone;
                    taskBuilder.SetTrigger(new TimeTrigger(15, false));
                    var registration = taskBuilder.Register();
                }
            }
            catch (Exception)
            {
               
                //new MessageDialog(e.Message).ShowAsync();
            }

        }

        private const string taskName = "NativeTileUpdater";
        private const string taskEntryPointWindowsPhone = "ListBackgroundTasks.NativeTileUpdater";
        private const string taskEntryPointWindows = "ListBackgroundTasks.NativeTileUpdaterWin";

    }
}
