using JP.Utils.UI;
using System;
using System.Collections;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace JP.Utils.Framework
{
    //Usage:
    /*
    <ListView
             attach:ListViewReorderItemAttach.UIElementUsedToMove="ReorderElement"
             ItemsSource="{x:Bind Collection,Mode=OneWay}"
             ItemContainerStyle="{StaticResource ListViewItemStyle1}">
             <ListView.ItemTemplate>
                 <DataTemplate>
                     <Grid Height="50" Background="#FFECECEC" Margin="5">
                         <Grid.ColumnDefinitions>
                             <ColumnDefinition/>
                             <ColumnDefinition Width="50"/>
                         </Grid.ColumnDefinitions>
                         <TextBlock Text="{Binding}" Margin="5" VerticalAlignment="Center"/>
                         <Grid ManipulationMode="TranslateY" x:Name="ReorderElement" Background="#FF2E2E2E" Grid.Column="1">
                             <SymbolIcon Foreground="White" Symbol="More"/>
                         </Grid>
                     </Grid>
                 </DataTemplate>
             </ListView.ItemTemplate>
         </ListView>
     */
    public class ListViewReorderItemAttach
    {
        public static string GetUIElementUsedToMove(DependencyObject obj)
        {
            return (string)obj.GetValue(UIElementUsedToMoveProperty);
        }

        public static void SetUIElementUsedToMove(DependencyObject obj, string value)
        {
            obj.SetValue(UIElementUsedToMoveProperty, value);
        }

        public static readonly DependencyProperty UIElementUsedToMoveProperty =
            DependencyProperty.RegisterAttached("UIElementUsedToMove", typeof(string), typeof(ListViewReorderItemAttach), new PropertyMetadata(null, OnPropertyChanged));

        private static ListViewBase _listViewBase;
        private static ScrollViewer _scrollViewer;
        private static Compositor _compositor;
        private static Visual _movingVisual;
        private static int _movingItemIndex;
        private static string _thumbName;
        private static int _zindex;
        private static bool[] _isItemsAnimated;
        private static double _distanceToTopBeforeMoving = 0f;
        private static double _distanceToTopAfterMoving = 0f;
        private static TransitionCollection _transitionCollection;

        public static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _listViewBase = d as ListViewBase;
            _transitionCollection = _listViewBase.ItemContainerTransitions;
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
            _movingItemIndex= _listViewBase.IndexFromContainer(movingItemContainer);

            //Disable transition animation to make the ending smooth.
            DisableTransition();

            //The starting position
            _distanceToTopBeforeMoving =movingItemContainer.TransformToVisual(_scrollViewer.Content as UIElement).TransformPoint(new Point(0, 0)).Y;

            var items = _listViewBase.Items;

            //Fade out all items
            for (int i = 0; i < items.Count; i++)
            {
                if (_movingItemIndex == i) continue;

                var item = items[i];
                var itemContainer = _listViewBase.ContainerFromItem(item) as ListViewItem;
                var containerVisual = ElementCompositionPreview.GetElementVisual(itemContainer);

                var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeAnimation.InsertKeyFrame(1f, 0.3f);
                fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);

                containerVisual.StartAnimation("Opacity", fadeAnimation);
            }

            _isItemsAnimated = new bool[_listViewBase.Items.Count];
        }

        private static void UIElementToReorder_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var uielement = sender as FrameworkElement;
            var grid = uielement.Parent as FrameworkElement;

            //Make sure it's on top.
            var currentItem = _listViewBase.ContainerFromItem(grid.DataContext) as FrameworkElement;
            Canvas.SetZIndex(currentItem, _zindex++);

            var currentIndex = _listViewBase.IndexFromContainer(currentItem);

            _distanceToTopAfterMoving = currentItem.TransformToVisual(_scrollViewer.Content as UIElement).TransformPoint(new Point(0, 0)).Y;

            //Move element by dy.
            _movingVisual.Offset = new Vector3(0f, (float)(_movingVisual.Offset.Y + e.Delta.Translation.Y), 0f);

            for (int i = 0; i < _listViewBase.Items.Count; i++)
            {
                if (i == currentIndex) continue;

                //Caluclate the position betwee moving item and others and do some animations.
                AnimateItemOffset(i);
            }

            //TODO: Scroll the ScrollViewer while the moving item is about to exit the visible area.

            //var pointToSV = container.TransformToVisual(_scrollViewer.Content as UIElement).TransformPoint(new Point(0, 0));
            //if (pointToSV.Y + visual.Offset.Y + 200 > _scrollViewer.ActualHeight)
            //{
            //    var scrollViewerY = _scrollViewer.VerticalOffset;
            //    _scrollViewer.ChangeView(null, 10 + scrollViewerY, null);
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
                containerVisual.Offset = new Vector3(0f,0f,0f);
            }

            var indexToIinsert = _movingItemIndex + (e.Cumulative.Translation.Y < 0 ? (int)(Math.Ceiling(e.Cumulative.Translation.Y / 55)) : (int)(Math.Floor(e.Cumulative.Translation.Y / 55)));
            if (indexToIinsert < 0) indexToIinsert = 0;
            
            //Now move item from ItemsSource 
            //Moving from Items property will cause an exception.
            var collection = _listViewBase.ItemsSource as IList;
            if (collection == null) throw new ArgumentNullException("The ItemsSource should inherit from IList.");

            var movingItem = collection[_movingItemIndex];
            collection.Remove(movingItem);
            collection.Insert(indexToIinsert, movingItem);
        }

        private static void AnimateItemOffset(int itemIndex)
        {
            var item = _listViewBase.ContainerFromIndex(itemIndex) as FrameworkElement;
            var itemVisual = ElementCompositionPreview.GetElementVisual(item);
            var offsetYToTop = (float)(item.TransformToVisual(_scrollViewer.Content as UIElement).TransformPoint(new Point(0, 0))).Y;

            var targetOffsetY = 0f;
            if (_distanceToTopAfterMoving > _distanceToTopBeforeMoving)
            {
                if (offsetYToTop > _distanceToTopBeforeMoving && offsetYToTop < _distanceToTopAfterMoving)
                {
                    targetOffsetY = -50f;
                }
                else targetOffsetY = 0f;
            }
            else
            {
                if (offsetYToTop > _distanceToTopAfterMoving && offsetYToTop < _distanceToTopBeforeMoving)
                {
                    targetOffsetY = 50f;
                }
                else targetOffsetY = 0f;
            }

            //The one are being animated should not be animated at this time.
            //Note that mass number of animations being triggered wil slow down the app, specifically in Mobile device.
            if (!_isItemsAnimated[itemIndex])
            {
                if (itemVisual.Offset.Y == targetOffsetY) return;

                var movingAnim = _compositor.CreateScalarKeyFrameAnimation();
                movingAnim.InsertKeyFrame(1f, targetOffsetY);
                movingAnim.Duration = TimeSpan.FromMilliseconds(200);

                //Create a batch to know when the animation would end.
                var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                itemVisual.StartAnimation("Offset.Y", movingAnim);
                _isItemsAnimated[itemIndex] = true;
                batch.End();

                batch.Completed += (sender, e) =>
                {
                    _isItemsAnimated[itemIndex] = false;
                };
            }
        }

        private static void EnableTransition()
        {
            _listViewBase.ItemContainerTransitions = _transitionCollection;
        }

        private static void DisableTransition()
        {
            _transitionCollection = _listViewBase.ItemContainerTransitions;
            _listViewBase.ItemContainerTransitions=null;
        }
    }
}
