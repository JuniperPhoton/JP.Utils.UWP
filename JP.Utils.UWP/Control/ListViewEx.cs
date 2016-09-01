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
using JP.Utils.UWP.Data;

namespace JP.Utils.Control
{
    public class ListViewEx : ListView
    {
        public string StickyHeaderName
        {
            get { return (string)GetValue(StickyHeaderNameProperty); }
            set { SetValue(StickyHeaderNameProperty, value); }
        }

        public static readonly DependencyProperty StickyHeaderNameProperty =
            DependencyProperty.Register("StickyHeaderName", typeof(string), typeof(ListViewEx),
                new PropertyMetadata(null));

        public string ReorderUIElementName
        {
            get { return (string)GetValue(ReorderUIElementNameProperty); }
            set { SetValue(ReorderUIElementNameProperty, value); }
        }

        public static readonly DependencyProperty ReorderUIElementNameProperty =
            DependencyProperty.Register("ReorderUIElementName", typeof(string), typeof(ListViewEx),
                new PropertyMetadata(null));

        public event Action OnReorderStopped;

        private ScrollViewer _scrollViewer;
        private Compositor _compositor;
        private Visual _movingVisual;
        private int _movingItemIndex;
        private int _zindex;
        private bool[] _isItemsAnimated;
        private int _distanceToTopBeforeMoving = 0;
        private int _distanceToTopWhenMoving = 0;
        private int _passingByItemsCount = 0;
        private FrameworkElement _movingItem;

        public ListViewEx()
        {
            this.ContainerContentChanging += this_ContainerContentChanging;
            this.RegisterPropertyChangedCallback(HeaderProperty, (sender, e) =>
             {
                 ((FrameworkElement)Header).SizeChanged += header_SizeChanged;
             });
        }

        #region Sticky Header
        private void header_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _scrollViewer = this.GetScrollViewer();
            InitialStickyHeader();
        }

        private void InitialStickyHeader()
        {
            if (_scrollViewer != null && StickyHeaderName != null)
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

                //当往上滚动的时候 ScrollingProperties.Translation.Y 不断负向增加，(ScrollingProperties.Translation.Y +OffsetY)
                //表明 StickyContent 距离顶部还有多少距离，当要滚出屏幕顶部的时候，(-OffsetY - ScrollingProperties.Translation.Y) 
                //计算还要把 Offset.Y 增加多少才让其 Sticky
                var scrollingAnimation = compositor.CreateExpressionAnimation(
                    "ScrollingProperties.Translation.Y +OffsetY> 0 ? 0 : -OffsetY - ScrollingProperties.Translation.Y");
                scrollingAnimation.SetReferenceParameter("ScrollingProperties", scrollProperties);
                scrollingAnimation.SetScalarParameter("OffsetY", offsetY);

                stickyVisual.StartAnimation("Offset.Y", scrollingAnimation);
            }
        }

        private FrameworkElement FindHeader()
        {
            return this.Header as FrameworkElement;
        }

        private FrameworkElement FindStickyContent()
        {
            var header = FindHeader();
            var stickyContent = header.FindName(StickyHeaderName) as FrameworkElement;
            return stickyContent;
        }

        #endregion

        #region Reorder
        private void this_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            _scrollViewer = this.GetScrollViewer();
            var listViewVisual = ElementCompositionPreview.GetElementVisual(sender);
            _compositor = listViewVisual.Compositor;

            if (!args.InRecycleQueue)
            {
                args.ItemContainer.Loaded -= ItemContainer_Loaded;
                args.ItemContainer.Loaded += ItemContainer_Loaded;
            }
        }

        private void ItemContainer_Loaded(object sender, RoutedEventArgs e)
        {
            if (ReorderUIElementName == null) return;

            var itemsPanel = (ItemsStackPanel)this.ItemsPanelRoot;
            var itemContainer = (ListViewItem)sender;
            var itemIndex = this.IndexFromContainer(itemContainer);

            var item = itemContainer.ContentTemplateRoot as FrameworkElement;
            var elementToReorder = item.FindName(ReorderUIElementName) as UIElement;

            if (elementToReorder == null)
                throw new ArgumentNullException($"Can find the the UIElement(named {ReorderUIElementName} used to be maniputed.");

            elementToReorder.ManipulationMode = ManipulationModes.TranslateY;

            elementToReorder.ManipulationStarted -= UIElementToReorder_ManipulationStarted;
            elementToReorder.ManipulationDelta -= UIElementToReorder_ManipulationDelta;
            elementToReorder.ManipulationCompleted -= UIElementToReorder_ManipulationCompleted;

            elementToReorder.ManipulationStarted += UIElementToReorder_ManipulationStarted;
            elementToReorder.ManipulationDelta += UIElementToReorder_ManipulationDelta;
            elementToReorder.ManipulationCompleted += UIElementToReorder_ManipulationCompleted;
        }

        private void UIElementToReorder_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            var uiElement = sender as FrameworkElement;
            var rootElement = uiElement.Parent as FrameworkElement;

            _movingItem = this.ContainerFromItem(rootElement.DataContext) as FrameworkElement;
            _movingVisual = ElementCompositionPreview.GetElementVisual(_movingItem);
            _movingItemIndex = this.IndexFromContainer(_movingItem);

            //Disable transition animation to make the ending smooth.
            DisableTransition();

            //The starting position
            _distanceToTopBeforeMoving = (int)(_movingItem.TransformToVisual(_scrollViewer.Content as UIElement).
                TransformPoint(new Point(0, 0)).Y);

            var items = this.Items;

            //Fade out all items
            for (int i = 0; i < items.Count; i++)
            {
                if (_movingItemIndex == i) continue;

                var item = items[i];
                var itemContainer = this.ContainerFromItem(item) as ListViewItem;
                if (itemContainer == null) continue;
                var containerVisual = ElementCompositionPreview.GetElementVisual(itemContainer);

                var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeAnimation.InsertKeyFrame(1f, 0.3f);
                fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);

                containerVisual.StartAnimation("Opacity", fadeAnimation);
            }

            _isItemsAnimated = new bool[this.Items.Count];
        }

        private void UIElementToReorder_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var uiElement = sender as FrameworkElement;
            var grid = uiElement.Parent as FrameworkElement;

            Canvas.SetZIndex(_movingItem, _zindex++);

            _distanceToTopWhenMoving = (int)(_movingItem.TransformToVisual(_scrollViewer.Content as UIElement)
                .TransformPoint(new Point(0, 0)).Y);

            _movingVisual.Offset = new Vector3(0f, (float)(_movingVisual.Offset.Y + e.Delta.Translation.Y), 0f);

            var passingCount = (int)_movingVisual.Offset.Y / (int)(uiElement.ActualHeight / 2);

            if (Math.Abs(passingCount - _passingByItemsCount) == 1)
            {
                _passingByItemsCount = passingCount;
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (i == _movingItemIndex) continue;
                    AnimateEachItem(i);
                }
            }

            var distanceToListView = (int)_movingItem.TransformToVisual(_scrollViewer.Content as UIElement)
                .TransformPoint(new Point(0, 0)).Y;

            //TODO: Scroll the ScrollViewer while the moving item is about to exit the visible area.
            if (distanceToListView + _movingItem.ActualHeight + 20 > _scrollViewer.ActualHeight)
            {
                //var scrollViewerY = _scrollViewer.VerticalOffset;
                //_scrollViewer.ChangeView(null, e.Delta.Translation.Y + scrollViewerY + 10f, null);
                //_movingVisual.Offset = new Vector3(0f, (float)(_movingVisual.Offset.Y + e.Delta.Translation.Y), 0f);
            }
        }

        private void UIElementToReorder_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            _passingByItemsCount = 0;

            var indexToInsert = (int)Math.Floor((_distanceToTopWhenMoving + _movingItem.ActualHeight / 2)
                / _movingItem.ActualHeight);

            if (indexToInsert < 0) indexToInsert = 0;
            if (indexToInsert >= this.Items.Count) indexToInsert = this.Items.Count - 1;

#if DEBUG
            System.Diagnostics.Debug.WriteLine("INDEX TO INSERT------------" + indexToInsert);
#endif

            var collection = this.ItemsSource as IList;
            if (collection == null)
            {
                throw new ArgumentNullException("The ItemsSource should inherit from IList.");
            }

            var movingItem = collection[_movingItemIndex];
            var itemToInsert = collection[indexToInsert];

            var itemToInsertOffsetY = (ContainerFromItem(itemToInsert) as ListViewItem)
                .TransformToVisual(this)
                .TransformPoint(new Point(0, 0)).Y - GetVisual((ContainerFromItem(itemToInsert) as ListViewItem)).Offset.Y;

            var movingitemToInsertOffsetY = (ContainerFromItem(movingItem) as ListViewItem)
                .TransformToVisual(this)
                .TransformPoint(new Point(0, 0)).Y;

            var deltaDistance = (float)itemToInsertOffsetY - (float)movingitemToInsertOffsetY;

            var backAnimation = _compositor.CreateScalarKeyFrameAnimation();
            backAnimation.InsertKeyFrame(1f, _movingVisual.Offset.Y + deltaDistance);
            backAnimation.Duration = TimeSpan.FromMilliseconds(300);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _movingVisual.StartAnimation("Offset.y", backAnimation);
            batch.Completed += (s, ex) =>
              {
                  RestoreItemsStatus();

                  collection.Remove(movingItem);
                  collection.Insert(indexToInsert, movingItem);

                  OnReorderStopped?.Invoke();
              };
            batch.End();
        }

        private void AnimateEachItem(int itemIndex)
        {
#if DEBUG
            string debugInfo = "[NEW]----------------------------" + Environment.NewLine;
#endif

            var item = this.ContainerFromIndex(itemIndex) as ListViewItem;
            if (item == null) return;
            var itemVisual = ElementCompositionPreview.GetElementVisual(item);

            var itemOffsetYToTop = (int)((item.TransformToVisual(_scrollViewer.Content as UIElement)
                .TransformPoint(new Point(0, 0))).Y);

            var deltaOffsetY = 0f;
            var targetOffsetY = itemVisual.Offset.Y + deltaOffsetY;

            // Moving downward
            if (_distanceToTopWhenMoving > _distanceToTopBeforeMoving)
            {
#if DEBUG
                debugInfo += "MOVING DOWN" + Environment.NewLine;
#endif
                if (itemOffsetYToTop >= _distanceToTopBeforeMoving
                    && itemOffsetYToTop <= _distanceToTopWhenMoving + _movingItem.ActualHeight / 2)
                {
#if DEBUG
                    debugInfo += "MOVING DOWN,itemOffsetYToTop:" + itemOffsetYToTop + Environment.NewLine;
                    debugInfo += "MOVING DOWN,_distanceToTopBeforeMoving:" + _distanceToTopBeforeMoving + Environment.NewLine;
                    debugInfo += "MOVING DOWN,_distanceToTopWhenMoving:" + _distanceToTopWhenMoving + Environment.NewLine;
#endif
                    deltaOffsetY = (float)-item.ActualHeight;
                    targetOffsetY = itemVisual.Offset.Y + deltaOffsetY;
                }
                else
                {
#if DEBUG
                    debugInfo += "MOVING DOWN,NOT INCLUDE" + Environment.NewLine;
                    debugInfo += "MOVING DOWN,NOT INCLUDE,itemOffsetYToTop:" + itemOffsetYToTop + Environment.NewLine
                        + "_distanceToTopBeforeMoving" + _distanceToTopBeforeMoving + Environment.NewLine
                        + "_distanceToTopWhenMoving" + _distanceToTopWhenMoving +
                        Environment.NewLine;
#endif
                    targetOffsetY = 0f;
                }
            }
            // Moving topward
            else
            {
#if DEBUG
                debugInfo += "MOVING UP" + Environment.NewLine;
#endif
                if (itemOffsetYToTop >= _distanceToTopWhenMoving - _movingItem.ActualHeight / 2
                    && itemOffsetYToTop <= _distanceToTopBeforeMoving)
                {
#if DEBUG
                    debugInfo += "MOVING UP,itemOffsetYToTop:" + itemOffsetYToTop + Environment.NewLine;
#endif
                    deltaOffsetY = (float)item.ActualHeight;
                    targetOffsetY = itemVisual.Offset.Y + deltaOffsetY;
                }
                else
                {
#if DEBUG
                    debugInfo += "MOVING UP,NOT INCLUDE" + Environment.NewLine;
                    debugInfo += "MOVING UP,NOT INCLUDE,itemOffsetYToTop:" + itemOffsetYToTop + Environment.NewLine
                        + "_distanceToTopBeforeMoving" + _distanceToTopBeforeMoving + Environment.NewLine
                        + "_distanceToTopWhenMoving" + _distanceToTopWhenMoving +
                        Environment.NewLine;
#endif
                    targetOffsetY = 0f;
                }
            }

            targetOffsetY = targetOffsetY.Clamp(-(float)(item.ActualHeight), (float)item.ActualHeight);

            //The one are being animated should not be animated at this time.
            //Note that mass number of animations being triggered wil slow down the app, 
            //specifically in Mobile device.
            if (!_isItemsAnimated[itemIndex])
            {
                if (itemVisual.Offset.Y == targetOffsetY) return;

                var movingAnim = _compositor.CreateScalarKeyFrameAnimation();
                movingAnim.InsertKeyFrame(0f, itemVisual.Offset.Y);
                movingAnim.InsertKeyFrame(1f, targetOffsetY);
                movingAnim.Duration = TimeSpan.FromMilliseconds(200);
#if DEBUG
                debugInfo += $"OFFSET:{targetOffsetY} ,INDEX {itemIndex}" + Environment.NewLine;
                debugInfo += "[END]----------------------------" + Environment.NewLine;
                System.Diagnostics.Debug.WriteLine(debugInfo);
#endif
                //Create a batch to know when the animation would end.
                var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                itemVisual.StartAnimation("Offset.Y", movingAnim);
                _isItemsAnimated[itemIndex] = true;

                batch.Completed += (sender, e) =>
                {
                    _isItemsAnimated[itemIndex] = false;
                };
                batch.End();
            }
        }

        private void RestoreItemsStatus()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = (ItemsSource as IList)[i];

                var container = ContainerFromItem(item) as ListViewItem;
                var index = IndexFromContainer(container);
                var containerVisual = ElementCompositionPreview.GetElementVisual(container);

                var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeAnimation.InsertKeyFrame(1f, 1f);
                fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);

                containerVisual.StartAnimation("Opacity", fadeAnimation);
                containerVisual.Offset = new Vector3(0f, 0f, 0f);
            }
        }

        private void DisableTransition()
        {
            this.ItemContainerTransitions = null;
        }

        public static Visual GetVisual(UIElement element)
        {
            return ElementCompositionPreview.GetElementVisual(element);
        }
        #endregion
    }
}
