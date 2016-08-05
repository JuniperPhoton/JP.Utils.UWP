using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace JP.Utils.Animation
{
    public class MoveAnimator : AbstractVertor3Animator
    {
        public override async Task NowAsync()
        {
            await base.NowAsync();
            var easing = Compositor.CreateCubicBezierEasingFunction(new Vector2(0, 1), new Vector2(1, 0));
            var animation = Compositor.CreateVector3KeyFrameAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(DurationTimeInMiles);
            animation.InsertKeyFrame(0, FromValue);
            animation.InsertKeyFrame(1f, ToValue);

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            var batch = Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            RootVisual.StartAnimation("Offset", animation);
            batch.Completed += (sender, e) =>
              {
                  tcs.SetResult(true);
              };
            batch.End();

            await tcs.Task;
        }
    }
}
