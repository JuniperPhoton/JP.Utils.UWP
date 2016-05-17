using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace JP.UWP.CustomControl
{
    public enum LayoutEnum
    {
        Stretch,
        Center,
        Bottom
    }
    public class ContentPopupEx : ContentControl
    {
        private TaskCompletionSource<int> _tcs;

        private Grid _rootGrid;
        private Grid _contentGrid;
        private FrameworkElement _rootFramework;
        private Border _maskBorder;

        private Popup _currentPopup;
        private LayoutEnum _layout;

        public bool PlayPopupAnim = true;

        private Storyboard _inStory;
        private Storyboard _outStory;

        private Page _currentPage;
        public Page CurrentPage
        {
            get
            {
                if (_currentPage != null) return _currentPage;
                else return ((Window.Current.Content as Frame).Content) as Page;
            }
            set
            {
                _currentPage = value;
            }
        }

        private bool _isOpen = false;

        private ContentPopupEx()
        {
            DefaultStyleKey = typeof(ContentPopupEx);

            _tcs = new TaskCompletionSource<int>();

            if (_currentPopup == null)
            {
                _currentPopup = new Popup();
                _currentPopup.VerticalAlignment = VerticalAlignment.Stretch;
                this.Height = (Window.Current.Content as Frame).Height;
                this.Width = (Window.Current.Content as Frame).Width;
                _currentPopup.Child = this;
                _currentPopup.IsOpen = true;
            }

            CurrentPage.SizeChanged -= Page_SizeChanged;
            CurrentPage.SizeChanged += Page_SizeChanged;
        }

        public ContentPopupEx(FrameworkElement element, LayoutEnum layout = LayoutEnum.Center) : this()
        {
            this._rootFramework = element;
            this._layout = layout;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _rootGrid = GetTemplateChild("RootGrid") as Grid;
            _contentGrid = GetTemplateChild("ContentGrid") as Grid;
            if (_layout == LayoutEnum.Stretch)
            {
                _contentGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                _contentGrid.VerticalAlignment = VerticalAlignment.Stretch;
            }
            else if (_layout == LayoutEnum.Bottom)
            {
                _contentGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                _contentGrid.VerticalAlignment = VerticalAlignment.Bottom;
            }
            _contentGrid.Children.Add(_rootFramework);
            _inStory = _rootGrid.Resources["InStory"] as Storyboard;
            _outStory = _rootGrid.Resources["OutStory"] as Storyboard;
            _maskBorder = GetTemplateChild("MaskBorder") as Border;
            _outStory.Completed += ((sender, e) =>
            {
                _currentPopup.IsOpen = false;
            });
            _maskBorder.Tapped += ((sendert, et) =>
            {
                if (!_isOpen)
                {
                    return;
                }
                Hide();
            });
            _tcs.TrySetResult(0);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCurrentLayout();
        }

        private void UpdateCurrentLayout()
        {
            _rootGrid.Width = this.Width = Window.Current.Bounds.Width;
            _rootGrid.Height = this.Height = Window.Current.Bounds.Height;
        }

        public async Task ShowAsync()
        {
            await _tcs.Task;
            PopupService.CurrentShownCPEX = this;
            UpdateCurrentLayout();
            _maskBorder.Visibility = Visibility.Visible;
            _isOpen = true;
            if (PlayPopupAnim)
                _inStory.Begin();
        }

        public void Hide()
        {
            _isOpen = false;
            _outStory.Begin();
            _maskBorder.Visibility = Visibility.Collapsed;
            PopupService.CurrentShownCPEX = null;
        }
    }
}
