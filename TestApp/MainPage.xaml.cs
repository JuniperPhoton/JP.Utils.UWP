using JP.Utils.Debug;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace TestApp
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<string> _list;
        public ObservableCollection<string> List
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(List)));
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            _list = new ObservableCollection<string>();
            for (int i = 0; i < 51; i++)
            {
                _list.Add(i.ToString());
            }
            listview.ItemsSource = _list;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
