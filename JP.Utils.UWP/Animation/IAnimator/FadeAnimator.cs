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

        public override async Task NowAsync()
        {
            await base.NowAsync();
            _animation = Compositor.CreateScalarKeyFrameAnimation();
            _animation.InsertKeyFrame(0f, _fromValue);
            _animation.InsertKeyFrame(1f, _toValue);
            _animation.Duration = TimeSpan.FromMilliseconds(DurationTimeInMiles);
            _animation.DelayTime = TimeSpan.FromMilliseconds(DelayTimeSpan);

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            var batch = Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            
            RootVisual.StartAnimation("Opacity", _animation);
            batch.Completed += (sender, e) =>
              {
                  tcs.SetResult(true);
              };
            batch.End();

            await tcs.Task;
        }

        public override void Now()
        {
            var task = this.NowAsync();
        }
    }
}
