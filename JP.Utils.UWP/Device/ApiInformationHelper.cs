using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace JP.Utils.Helper
{
    public static class ApiInformationHelper
    {
        public static bool HasHardwareButton
        {
            get
            {
                return CheckHardwareButton();
            }
        }

        public static bool HasStatusBar
        {
            get
            {
                return CheckStatusBar();
            }
        }

        private static bool CheckHardwareButton()
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                return true;
            }
            else return false;
        }

        private static bool CheckStatusBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                return true;
            }
            else return false;
        }
    }
}
