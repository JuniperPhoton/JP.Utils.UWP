using JP.Utils.UI;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace JP.Utils.Framework
{
    public class ListViewStickyHeader
    {
        private static string _stickyElementName;
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
            DependencyProperty.RegisterAttached("StickyHeaderName", typeof(string), typeof(ListViewStickyHeader), new PropertyMetadata(null, OnHeaderPropertyChanged));

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _listViewBase = d as ListViewBase;
            _stickyElementName = e.NewValue as string;

            _listViewBase.RegisterPropertyChangedCallback(ListViewBase.HeaderProperty, (senderx, ex) =>
            {
                _header = _listViewBase.Header as FrameworkElement;
                _header.SizeChanged += _header_SizeChanged;
            });
        }

        private static void _header_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _scrollViewer = _listViewBase.GetScrollViewer();
            InitialStickyHeader();
        }

        private static void InitialStickyHeader()
        {
            if (_scrollViewer != null && _stickyElementName != null)
            {
                var header = FindHeader();

                //找到 Header 的父容器，设置它的 ZIndex 才能让其在 ListView 的 Items 之上
                var headerContainer = header.Parent as ContentControl;

                Canvas.SetZIndex(headerContainer, 1);

                var stickyHeader = FindStickyContent();

                if (stickyHeader == null) throw new ArgumentNullException("Make sure you have define x:Name of the UIElement to be sticky.");

                var stickyVisual = ElementCompositionPreview.GetElementVisual(stickyHeader);
                var compositor = stickyVisual.Compositor;

                //计算 StickyContent 距离 ScrollViewer Content 顶部的纵向距离
                var transform = stickyHeader.TransformToVisual((UIElement)_scrollViewer.Content);
                var offsetY = (float)transform.TransformPoint(new Point(0, 0)).Y;

                var scrollProperties = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);

                //当往下滚动的时候 ScrollingProperties.Translation.Y 不断负向增加，(ScrollingProperties.Translation.Y +OffsetY)
                //表明 StickyContent 距离顶部还有多少距离，当要滚出屏幕顶部的时候，(-OffsetY - ScrollingProperties.Translation.Y) 
                //计算还要把 Offset.Y 增加多少才让其 “Sticky“
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
            var stickyContent = header.FindName(_stickyElementName) as FrameworkElement;
            return stickyContent;
        }
    }
}
