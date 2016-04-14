using JP.Utils.UI;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;

namespace JP.Utils.Framework
{
    public class ListViewReorderItemAttach
    {
        public static string GetMovingThumbName(DependencyObject obj)
        {
            return (string)obj.GetValue(MovingThumbNameProperty);
        }

        public static void SetMovingThumbName(DependencyObject obj, string value)
        {
            obj.SetValue(MovingThumbNameProperty, value);
        }

        public static readonly DependencyProperty MovingThumbNameProperty =
            DependencyProperty.RegisterAttached("MovingThumbName", typeof(string), typeof(ListViewReorderItemAttach), new PropertyMetadata(null,OnPropertyChanged));

        private static ListViewBase _listViewBase;
        private static ScrollViewer _scrollViewer;
        private static Compositor _compositor;
        private static Visual _movingVisual;
        private static string _thumbName;
        private static int _zindex;

        public static void OnPropertyChanged(DependencyObject d,DependencyPropertyChangedEventArgs e)
        {
            _listViewBase = d as ListViewBase;
            _thumbName = e.NewValue as string;
            _listViewBase.ContainerContentChanging += _listViewBase_ContainerContentChanging;
        }

        private static void _listViewBase_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            _scrollViewer = _listViewBase.GetScrollViewer();
            var listViewVisual = ElementCompositionPreview.GetElementVisual(sender);
            _compositor = listViewVisual.Compositor;

            if (!args.InRecycleQueue)
            {
                args.ItemContainer.Loaded -= ItemContainer_Loaded;
                args.ItemContainer.Loaded += ItemContainer_Loaded;
            }
        }

        private static void ItemContainer_Loaded(object sender, RoutedEventArgs e)
        {
            var itemsPanel = (ItemsStackPanel)_listViewBase.ItemsPanelRoot;
            var itemContainer = (ListViewItem)sender;
            var itemIndex = _listViewBase.IndexFromContainer(itemContainer);

            var item = itemContainer.ContentTemplateRoot as FrameworkElement;
            var uielementToReorder = item.FindName(_thumbName) as Grid;

            uielementToReorder.ManipulationStarted -= UIElementToReorder_ManipulationStarted;
            uielementToReorder.ManipulationDelta -= UIElementToReorder_ManipulationDelta;
            uielementToReorder.ManipulationCompleted -= UIElementToReorder_ManipulationCompleted;

            uielementToReorder.ManipulationStarted += UIElementToReorder_ManipulationStarted;
            uielementToReorder.ManipulationDelta += UIElementToReorder_ManipulationDelta;
            uielementToReorder.ManipulationCompleted += UIElementToReorder_ManipulationCompleted;
        }


        private static void UIElementToReorder_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            var uiElement = sender as FrameworkElement;
            var rootElement = uiElement.Parent as FrameworkElement;

            var movingItemContainer = _listViewBase.ContainerFromItem(rootElement.DataContext) as UIElement;
            _movingVisual = ElementCompositionPreview.GetElementVisual(movingItemContainer);
            var currentIndex = _listViewBase.IndexFromContainer(movingItemContainer);

            var items = _listViewBase.Items;
            for (int i = 0; i < items.Count; i++)
            {
                if (currentIndex == i) continue;

                var item = items[i];
                var itemContainer = _listViewBase.ContainerFromItem(item) as UIElement;
                var itemContainerVisual = ElementCompositionPreview.GetElementVisual(itemContainer);

                var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeAnimation.InsertKeyFrame(1f, 0.2f);
                fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);
                itemContainerVisual.StartAnimation("Opacity", fadeAnimation);

                var itemPosition = itemContainer.TransformToVisual(_scrollViewer.Content as UIElement).TransformPoint(new Point(0, 0)).Y;

                var movingItemOriPosition = movingItemContainer.TransformToVisual(_scrollViewer.Content as UIElement).TransformPoint(new Point(0, 0)).Y;
                var movingItemCurrentPosition = movingItemOriPosition + _movingVisual.Offset.Y;

                var expressAnimation = _compositor.CreateExpressionAnimation();
                expressAnimation.Expression =
                    "((itemPosition>movingItemOriPosition)&&(itemPosition<(movingItemOriPosition + MovingVisual.Offset.Y+50f)))?"+
                        "((this.CurrentValue<=-50f)?-50f:(-MovingVisual.Offset.Y/(index-currentIndex))):" +
                        "0f";
                expressAnimation.SetScalarParameter("itemPosition", (float)itemPosition);
                expressAnimation.SetScalarParameter("movingItemOriPosition", (float)movingItemOriPosition);
                expressAnimation.SetReferenceParameter("MovingVisual", _movingVisual);
                expressAnimation.SetReferenceParameter("AnimVisual", itemContainerVisual);
                expressAnimation.SetScalarParameter("index", i);
                expressAnimation.SetScalarParameter("currentIndex", currentIndex);
                itemContainerVisual.StartAnimation("Offset.Y", expressAnimation);
            }
        }

        private static void UIElementToReorder_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var uielement = sender as FrameworkElement;
            var grid = uielement.Parent as FrameworkElement;

            var container = _listViewBase.ContainerFromItem(grid.DataContext) as UIElement;

            var index = _listViewBase.IndexFromContainer(container);
            Canvas.SetZIndex(container, _zindex++);

            var originalY = _movingVisual.Offset.Y;
            var newY = originalY + e.Delta.Translation.Y;
            _movingVisual.Offset = new Vector3(0f, (float)newY, 0f);

            var pointToSV = container.TransformToVisual(_scrollViewer.Content as UIElement).TransformPoint(new Point(0, 0));
            //if (pointToSV.Y + visual.Offset.Y +200> _scrollViewer.ActualHeight)
            //{
            //    var scrollViewerY = _scrollViewer.VerticalOffset;
            //    _scrollViewer.ChangeView(null, 10+ scrollViewerY, null);
            //}
        }

        private static void UIElementToReorder_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            foreach (var item in _listViewBase.Items)
            {
                var container = _listViewBase.ContainerFromItem(item) as ListViewItem;

                var containerVisual = ElementCompositionPreview.GetElementVisual(container);

                var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeAnimation.InsertKeyFrame(1f, 1f);
                fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);
                containerVisual.StartAnimation("Opacity", fadeAnimation);
            }
        }
    }
}
