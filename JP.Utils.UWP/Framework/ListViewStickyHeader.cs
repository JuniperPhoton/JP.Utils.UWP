using JP.Utils.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace JP.Utils.Framework
{
    public class ListViewStickyHeader
    {
        public static string StickyElementName;
        private static ListViewBase _listViewBase;
        private static ScrollViewer _scrollViewer;
        private static FrameworkElement _header;

        public static string GetStickyHeaderName(DependencyObject obj)
        {
            return (string)obj.GetValue(StickyHeaderNameProperty);
        }

        public static void SetStickyHeaderName(DependencyObject obj, string value)
        {
            obj.SetValue(StickyHeaderNameProperty, value);
        }

        public static readonly DependencyProperty StickyHeaderNameProperty =
            DependencyProperty.RegisterAttached("StickyHeaderName", typeof(string), typeof(ListViewStickyHeader), new PropertyMetadata(null,OnHeaderPropertyChanged));

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _listViewBase = d as ListViewBase;
            StickyElementName = e.NewValue as string;
            _listViewBase.RegisterPropertyChangedCallback(ListViewBase.HeaderProperty, (senderx, ex) => 
            {
                _header = _listViewBase.Header as FrameworkElement;
                _header.SizeChanged += _header_SizeChanged;
            });
        }

        private static void _header_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _scrollViewer = _listViewBase.GetScrollViewer();
            InitialStickyHeader(_scrollViewer);
        }

        private static void InitialStickyHeader(ScrollViewer scrollViewer)
        {
            if (scrollViewer != null && StickyElementName != null)
            {
                var header = FindHeader();
                if (header == null) return;
                var headerContainer = header.Parent as ContentControl;

                Canvas.SetZIndex(headerContainer, 1);

                var stickyHeader = FindStickyContent();
                var stickyVisual = ElementCompositionPreview.GetElementVisual(stickyHeader);
                var compositor = stickyVisual.Compositor;

                var transform = stickyHeader.TransformToVisual((UIElement)scrollViewer.Content);
                var offsetY = (float)transform.TransformPoint(new Point(0, 0)).Y;

                var scrollProperties = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

                var scrollingAnimation = compositor.CreateExpressionAnimation(
                    "ScrollingProperties.Translation.Y +OffsetY> 0 ? 0 : -OffsetY - ScrollingProperties.Translation.Y");
                scrollingAnimation.SetReferenceParameter("ScrollingProperties", scrollProperties);
                scrollingAnimation.SetScalarParameter("OffsetY", offsetY);

                stickyVisual.StartAnimation("Offset.Y", scrollingAnimation);
            }
        }

        private static FrameworkElement FindHeader()
        {
            return _listViewBase.Header as FrameworkElement;
        }

        private static FrameworkElement FindStickyContent()
        {
            var header = FindHeader();
            var stickyContent = header.FindName(StickyElementName) as FrameworkElement;
            return stickyContent;
        }
    }
}
