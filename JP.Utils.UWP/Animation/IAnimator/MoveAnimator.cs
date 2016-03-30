using System.Numerics;

namespace JP.Utils.Animation
{
    public class MoveAnimator : AbstractVertor3Animator
    {
        public override void Now()
        {
            base.Now();
            var easing = Compositor.CreateCubicBezierEasingFunction(new Vector2(0, 1), new Vector2(1, 0));
            var animation = Compositor.CreateVector3KeyFrameAnimation();
            animation.Duration = DurationTime;
            animation.InsertKeyFrame(0, FromValue);
            animation.InsertKeyFrame(1f, ToValue);
            RootVisual.StartAnimation("Offset", animation);
        }
    }
}
