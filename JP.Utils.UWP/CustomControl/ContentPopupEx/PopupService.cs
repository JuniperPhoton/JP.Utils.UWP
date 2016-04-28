using JP.UWP.CustomControl;

namespace JP.UWP.CustomControl
{
    public static class PopupService
    {
        public static ContentPopupEx CurrentShownCPEX { get; set; }

        public static void TryHideCPEX()
        {
            if(CurrentShownCPEX!= null)
            {
                CurrentShownCPEX.Hide();
            }
        }
    }
}
