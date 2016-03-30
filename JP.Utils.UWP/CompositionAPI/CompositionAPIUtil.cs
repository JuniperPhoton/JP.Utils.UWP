using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace JP.Utils.CompositionAPI
{
    public static class CompositionAPIUtil
    {
        public static void MoveByValue(this UIElement element, Vector3 changedValue)
        {
            var rootVisual = GetElementVisual(element);
            var originalOffset = rootVisual.Offset;
            rootVisual.Offset = new Vector3(originalOffset.X + changedValue.X,
                originalOffset.Y + changedValue.Y,
                originalOffset.Z + changedValue.Z);
        }

        public static void RotateToNewAngle(this UIElement element, float angleInDegrees)
        {
            var rootVisual = element.GetElementVisual();
            rootVisual.RotationAxis = new Vector3(0f, 0f, 1f);
            rootVisual.RotationAngleInDegrees = angleInDegrees;
        }

        public static void RotateByAngle(this UIElement element, float anglesToAdd)
        {
            var rootVisual = element.GetElementVisual();
            rootVisual.RotationAxis = new Vector3(0f, 0f, 1f);
            rootVisual.RotationAngleInDegrees += anglesToAdd;
        }

        public static void ScaleToValue(this UIElement element, Vector3 scale)
        {
            var rootVisual = element.GetElementVisual();
            rootVisual.CenterPoint = new Vector3(0.5f,0.5f, 1f);
            element.GetElementVisual().Scale = scale;
        }

        public static Visual GetElementVisual(this UIElement element)
        {
            return ElementCompositionPreview.GetElementVisual(element);
        }
    }
}
