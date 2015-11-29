using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace JP.Utils.UI
{
    public static class ScrollViewExtensions
    {
        public static ScrollViewer GetScrollViewer(this DependencyObject element)
        {
            if (element is ScrollViewer)
            {
                return (ScrollViewer)element;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }

            return null;
        }

        public static double DistanceFromTop(this ScrollViewer scrollViewer, UIElement element)
        {
            var transform = element.TransformToVisual(scrollViewer);
            var position = transform.TransformPoint(new Point(0, 0));

            return position.Y;
        }

        public static bool IsItemVisible(this FrameworkElement container, FrameworkElement element)
        {
            var elementBounds = element.TransformToVisual(container).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
            var containerBounds = new Rect(0, 0, container.ActualWidth, container.ActualHeight);

            return (elementBounds.Top < containerBounds.Bottom && elementBounds.Bottom > containerBounds.Top);
        }
    }
}
