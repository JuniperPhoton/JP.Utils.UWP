using System;
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
            DependencyProperty.RegisterAttached("ItemClickCommand", typeof(ICommand), typeof(ListViewBaseCommandEx),
                new PropertyMetadata(null, OnItemClickedCommandPropertyChanged));

        private static void OnItemClickedCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListViewBase currentBase = d as ListViewBase;
            if (currentBase == null)
            {
                throw new ArgumentNullException("Must be used on ListView/GridView!");
            }

            currentBase.ItemClick += Control_ItemClick;
        }

        private static void Control_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListViewBase currentBase = sender as ListViewBase;

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

        #endregion CommandParameter
    }
}