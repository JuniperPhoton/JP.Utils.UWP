using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace JP.Utils.CompositionAPI
{
    public static class EasingFuncFactory
    {
        public static CubicBezierEasingFunction CreateCubibeEasingFunc(Compositor compositor)
        {
            var func = compositor.CreateCubicBezierEasingFunction(
                 new Vector2(), new Vector2(1f,1f));
            return func;
        }
    }
}
