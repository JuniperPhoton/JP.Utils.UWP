using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace JP.Utils.Animation
{
    public interface IAnimator
    {
        UIElement UIElement { get; set; }

        Visual RootVisual { get; set; }

        Compositor Compositor { get; set; }

        IAnimator AnimateWith(UIElement uiElement);

        IAnimator For(int timeInMiles);

        IAnimator From(float fromValue);

        IAnimator From(Vector3 fromValue);

        IAnimator To(float toValue);

        IAnimator To(Vector3 toValue);

        IAnimator Delay(int value);

        Task NowAsync();

        void Now();
    }
}
