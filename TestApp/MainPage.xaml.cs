using JP.Utils.Debug;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

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

            Logger.LogAsync("");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Method1();
        }

        public async void Method1()
        {
           await Method2();
        }

        private async Task Method2()
        {
            try
            {
                throw new ArgumentOutOfRangeException();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                await Logger.LogAsync(ex);
            }
        }
    }
}
