using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace JP.Utils.Animation
{
    public abstract class AbstractScalarAnimator : IAnimator
    {
        protected float _toValue;
        protected float _fromValue;

        public UIElement UIElement { get; set; }

        public TimeSpan DurationTime { get; set; }

        public Visual RootVisual { get; set; }

        public Compositor Compositor { get; set; }

        public IAnimator AnimateWith(UIElement uiElement)
        {
            UIElement = uiElement;
            return this;
        }

        public IAnimator For(TimeSpan time)
        {
            DurationTime = time;
            return this;
        }

        public IAnimator From(float fromValue)
        {
            _fromValue = fromValue;
            return this;
        }

        public IAnimator To(float toValue)
        {
            _toValue = toValue;
            return this;
        }

        public virtual void Now()
        {
            if (UIElement == null) throw new ArgumentNullException("Must specify an UIElement.");
            var root = ElementCompositionPreview.GetElementVisual(UIElement);
            var compositor = root.Compositor;
            this.RootVisual = root;
            this.Compositor = compositor;
        }

        public IAnimator From(Vector3 fromValue)
        {
            throw new NotImplementedException();
        }

        public IAnimator To(Vector3 toValue)
        {
            throw new NotImplementedException();
        }
    }
}
