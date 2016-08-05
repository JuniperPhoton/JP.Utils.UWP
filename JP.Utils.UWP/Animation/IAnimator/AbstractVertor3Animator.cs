using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace JP.Utils.Animation
{
    public abstract class AbstractVertor3Animator : IAnimator
    {
        protected Vector3 fromValue;
        protected Vector3 toValue;
        protected int _delayTime;

        public UIElement UIElement { get; set; }

        public int DurationTimeInMiles { get; set; }

        public Visual RootVisual { get; set; }

        public Compositor Compositor { get; set; }

        protected Vector3 FromValue
        {
            get
            {
                return fromValue;
            }

            set
            {
                fromValue = value;
            }
        }

        protected Vector3 ToValue
        {
            get
            {
                return toValue;
            }

            set
            {
                toValue = value;
            }
        }

        public IAnimator AnimateWith(UIElement uiElement)
        {
            UIElement = uiElement;
            return this;
        }

        public IAnimator For(int timeInMiles)
        {
            DurationTimeInMiles = timeInMiles;
            return this;
        }

        public IAnimator From(Vector3 fromValue)
        {
            FromValue = fromValue;
            return this;
        }

        public IAnimator To(Vector3 toValue)
        {
            ToValue = toValue;
            return this;
        }

        public virtual async Task NowAsync()
        {
            if (UIElement == null) throw new ArgumentNullException("Must specify an UIElement.");
            var root = ElementCompositionPreview.GetElementVisual(UIElement);
            var compositor = root.Compositor;
            this.RootVisual = root;
            this.Compositor = compositor;
        }

        public IAnimator From(float fromValue)
        {
            throw new NotImplementedException();
        }

        public IAnimator To(float toValue)
        {
            throw new NotImplementedException();
        }

        public IAnimator Delay(int timeSpan)
        {
            _delayTime = timeSpan;
            return this;
        }

        public virtual void Now()
        {
            this.NowAsync();
        }
    }
}
