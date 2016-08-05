using JP.Utils.Animation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TestApp
{
    public sealed partial class MainPage : Page , INotifyPropertyChanged
    {
        private Visual _border1Visual;
        private Visual _border2Visual;
        private Compositor _compositor;

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;

            _border1Visual = ElementCompositionPreview.GetElementVisual(Border1);
            _border2Visual = ElementCompositionPreview.GetElementVisual(Border2);
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            _border2Visual.Opacity = 0f;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await FlowsAnimator.CreateFadeAnimation().AnimateWith(Border1).From(1).To(0).For(3000).NowAsync();
            _border2Visual.Opacity = 1;
        }
    }
}
