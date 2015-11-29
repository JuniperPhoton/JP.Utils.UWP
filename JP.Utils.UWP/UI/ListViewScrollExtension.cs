using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace JP.Utils.UI
{
    public static class ListViewScrollExtension
    {
        public async static Task ScrollToIndex(this ListViewBase listViewBase, int index)
        {
            bool isVerticalScrolling, isVirtualizing = default(bool);
            double previousOffset = default(double);

            // if it's a ListView, then we assume the scrolling is vertical
            if (listViewBase is ListView)
            {
                isVerticalScrolling = true;
            }
            // if it's a GridView, then we assume the scrolling is horizontal
            else if (listViewBase is GridView)
            {
                isVerticalScrolling = false;
            }
            else
            {
                throw new ArgumentException("The control needs to inherit from ListViewBase!");
            }

            // get the ScrollViewer withtin the ListView/GridView
            var scrollViewer = listViewBase.GetScrollViewer();
            // get the SelectorItem to scroll to
            var selectorItem = listViewBase.ContainerFromIndex(index) as SelectorItem;

            // when it's null, means virtualization is on and the item hasn't been realized yet
            if (selectorItem == null)
            {
                isVirtualizing = true;
                previousOffset = isVerticalScrolling ? scrollViewer.VerticalOffset : scrollViewer.HorizontalOffset;

                // call ScrollIntoView to realize the item
                listViewBase.ScrollIntoView(listViewBase.Items[index], ScrollIntoViewAlignment.Leading);
                await Task.Delay(1);
                selectorItem = (SelectorItem)listViewBase.ContainerFromIndex(index);
            }

            // calculate the position object in order to know how much to scroll to
            var transform = selectorItem.TransformToVisual((UIElement)scrollViewer.Content);
            var position = transform.TransformPoint(new Point(0, 0));

            // when virtualized, scroll backward/forward a little bit (listViewBase.ActualHeight * 2) to allow animation to be at least visible
            if (isVirtualizing)
            {
                if (isVerticalScrolling)
                {
                    scrollViewer.ChangeView(null, position.Y + (position.Y > previousOffset ? -1 : 1) * listViewBase.ActualHeight * 2, null, true);
                }
                else
                {
                    scrollViewer.ChangeView(position.X + (position.X > previousOffset ? -1 : 1) * listViewBase.ActualWidth * 2, null, null, true);
                }
            }

            // do the scrolling
            if (isVerticalScrolling)
            {
                scrollViewer.ChangeView(null, position.Y, null);
            }
            else
            {
                scrollViewer.ChangeView(position.X, null, null);
            }
        }
    }
}
