using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TestApp.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TestApp
{
    public sealed partial class StartPage : BasePage
    {
        public StartPage()
        {
            this.InitializeComponent();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var index = NavigationGridView.IndexFromContainer(NavigationGridView.ContainerFromItem(e.ClickedItem));
            switch(index)
            {
                case 0:
                    {
                        Frame.Navigate(typeof(ListViewExPage));
                    };break;
            }
        }
    }
}
