using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace JP.Utils.Animation
{
    public class FadeAnimator : AbstractScalarAnimator
    {
        protected ScalarKeyFrameAnimation _animation;

        public override void Now()
        {
            base.Now();
            _animation = Compositor.CreateScalarKeyFrameAnimation();
            _animation.InsertKeyFrame(0f, _fromValue);
            _animation.InsertKeyFrame(1f, _toValue);
            _animation.Duration = TimeSpan.FromMilliseconds(DurationTimeInMiles);
            RootVisual.StartAnimation("Opacity", _animation);
        }
    }
}
