using Windows.UI.Xaml;

namespace JP.Utils.Animation
{
    public class FlowsAnimator
    {
        public static FadeAnimator CreateFadeAnimation()
        {
            return new FadeAnimator();
        }

        public static MoveAnimator CreateMoveAnimation()
        {
            return new MoveAnimator();
        }
    }
}
