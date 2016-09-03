using JP.Utils.Debug;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using TestApp.Common;
using TestApp.Model;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace TestApp
{
    public partial class ListViewExPage : BasePage
    {
        private ObservableCollection<ListViewExData> _list;
        public ObservableCollection<ListViewExData> List
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
                RaisePropertyChanged(() => nameof(List));
            }
        }

        public ListViewExPage()
        {
            this.InitializeComponent();
            _list = new ObservableCollection<ListViewExData>();
            for (int i = 0; i < 51; i++)
            {
                _list.Add(new ListViewExData() { Name = i.ToString() });
            }
            listview.ItemsSource = _list;
        }
    }
}
