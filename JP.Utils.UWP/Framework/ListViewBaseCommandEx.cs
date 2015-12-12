using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JP.Utils.Framework
{
    public class ListViewBaseCommandEx
    {
        public static ICommand GetItemClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ItemClickCommandProperty);
        }

        public static void SetItemClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ItemClickCommandProperty, value);
        }

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.RegisterAttached("ItemClickCommand", typeof(ICommand), typeof(ListViewBaseCommandEx), new PropertyMetadata(null, OnGridTappedCommandPropertyChanged));

        private static void OnGridTappedCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListViewBase currentBase = null;
            if(d is ListView)
            {
                currentBase = d as ListView;
            }
            else currentBase = d as GridView;

            if (currentBase != null)
            {
                currentBase.ItemClick -= Control_ItemClick;
                currentBase.ItemClick += Control_ItemClick;
            }
        }

        private static void Control_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListViewBase currentBase = null;
            if(sender is ListView)
            {
                currentBase = sender as ListView;
            }
            else
            {
                currentBase = sender as GridView;
            }
            if (currentBase != null)
            {
                var command = GetItemClickCommand(currentBase);
                
                var paramter = e.ClickedItem;

                object obj = null;

                if (paramter != null)
                {
                    obj = paramter;
                }
                else
                {
                    obj = currentBase.DataContext;
                }

                if (command != null && command.CanExecute(obj))
                    command.Execute(obj);
            }
        }

        #region CommandParameter
        public static readonly DependencyProperty ItemClickCommandParameterProperty =
           DependencyProperty.RegisterAttached("ItemClickCommandParameter", typeof(object), typeof(ListViewBaseCommandEx), null);

        public static object GetItemClickCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(ItemClickCommandParameterProperty);
        }

        public static void SetItemClickCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ItemClickCommandParameterProperty, value);
        }
        #endregion

    }
}
