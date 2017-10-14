using JP.Utils.UI;
using Windows.UI.Xaml;

namespace JP.Utils.Framework
{
    public static class ScrollViewerStyleChanger
    {
        private static Style NewStyle = null;

        public static Style GetStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(StyleProperty);
        }

        public static void SetStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(StyleProperty, value);
        }

        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.RegisterAttached("Style", typeof(Style), typeof(ScrollViewerStyleChanger),
                new PropertyMetadata(null, OnStylePropertyChanged));

        private static void OnStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FrameworkElement).Loaded += ScrollViewerStyleChanger_Loaded;
            NewStyle = e.NewValue as Style;
        }

        private static void ScrollViewerStyleChanger_Loaded(object sender, RoutedEventArgs e)
        {
            var sv = (sender as FrameworkElement).GetScrollViewer();
            if (sv != null && NewStyle != null)
                sv.Style = NewStyle;
        }
    }
}