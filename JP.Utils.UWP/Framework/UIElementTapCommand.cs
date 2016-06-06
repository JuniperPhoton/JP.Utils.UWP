using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace JP.Utils.Framework
{
    public class UIElementTapCommand
    {
        public static ICommand GetItemTapCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ItemTapCommandProperty);
        }

        public static void SetItemTapCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ItemTapCommandProperty, value);
        }

        public static readonly DependencyProperty ItemTapCommandProperty =
            DependencyProperty.RegisterAttached("ItemTapCommand", typeof(ICommand), 
                typeof(UIElementTapCommand), new PropertyMetadata(null, OnTappedCommandPropertyChanged));

        private static void OnTappedCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement currentBase = d as UIElement;

            if (currentBase != null)
            {
                currentBase.Tapped += Control_ItemTap;
            }
        }

        private static void Control_ItemTap(object sender, TappedRoutedEventArgs e)
        {
            UIElement currentBase = sender as UIElement;
            e.Handled = true;
            if (currentBase != null)
            {
                var command = GetItemTapCommand(currentBase);

                if (command != null)
                    command.Execute(currentBase);
            }
        }
    }
}
