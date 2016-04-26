using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.DirectX;
using Windows.Foundation;
using Windows.Graphics.Display;

namespace JP.UWP.CustomControl
{
    [ContentProperty(Name = nameof(Content))]
    public class Shadow :  Windows.UI.Xaml.Controls.Control
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Color), typeof(Shadow), new PropertyMetadata(Colors.Black));

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(FrameworkElement), typeof(Shadow), new PropertyMetadata(null, ContentChanged));

        public static readonly DependencyProperty DepthProperty = DependencyProperty.Register(nameof(Depth), typeof(double), typeof(Shadow), new PropertyMetadata(2.0d, DepthChanged));

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(nameof(Direction), typeof(double), typeof(Shadow), new PropertyMetadata(270.0d));

        private CanvasControl _canvas;

        private int _pixelHeight;

        private byte[] _pixels;

        private int _pixelWidth;

        public Shadow()
        {
            this.DefaultStyleKey = typeof(Shadow);
            this.Unloaded += this.OnUnloaded;
        }

        public Color Color
        {
            get
            {
                return (Color)this.GetValue(ColorProperty);
            }
            set
            {
                this.SetValue(ColorProperty, value);
            }
        }

        public FrameworkElement Content
        {
            get
            {
                return (FrameworkElement)this.GetValue(ContentProperty);
            }
            set
            {
                this.SetValue(ContentProperty, value);
            }
        }

        public double Depth
        {
            get
            {
                return (double)this.GetValue(DepthProperty);
            }
            set
            {
                this.SetValue(DepthProperty, value);
            }
        }

        public double Direction
        {
            get
            {
                return (double)this.GetValue(DirectionProperty);
            }
            set
            {
                this.SetValue(DirectionProperty, value);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._canvas = (CanvasControl)this.GetTemplateChild("PART_Canvas");
            this._canvas.Draw += this.Canvas_Draw;
            this.ExpendCanvas();
        }

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Shadow obj = (Shadow)d;

            FrameworkElement oldValue = (FrameworkElement)e.OldValue;
            if (oldValue != null)
            {
                oldValue.LayoutUpdated -= obj.Content_LayoutUpdated;
            }

            FrameworkElement newValue = (FrameworkElement)e.NewValue;
            if (newValue != null)
            {
                try
                {
                    newValue.LayoutUpdated += obj.Content_LayoutUpdated;
                }
                catch (Exception)
                {

                }
            }
        }

        private static void DepthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Shadow obj = (Shadow)d;
            obj.ExpendCanvas();
        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (this.Content == null || this._pixels == null || this._pixelWidth <= 0 || this._pixelHeight <= 0)
            {
                // 不满足绘制条件，清除 Canvas。
                args.DrawingSession.Clear(sender.ClearColor);
            }
            else
            {
                // 计算内容控件相对于 Canvas 的位置。
                GeneralTransform transform = this.Content.TransformToVisual(sender);
                Vector2 location = transform.TransformPoint(new Point()).ToVector2();

                using (CanvasCommandList cl = new CanvasCommandList(sender))
                {
                    using (CanvasDrawingSession clds = cl.CreateDrawingSession())
                    {
                        using (CanvasBitmap bitmap = CanvasBitmap.CreateFromBytes(sender, this._pixels, this._pixelWidth, this._pixelHeight, DirectXPixelFormat.B8G8R8A8UIntNormalized, DisplayInformation.GetForCurrentView().LogicalDpi))
                        {
                            // 在 Canvas 对应的位置中绘制内容控件的外观。
                            clds.DrawImage(bitmap, location);
                        }
                    }

                    float translateX = (float)(Math.Cos(Math.PI / 180.0d * this.Direction) * this.Depth);
                    float translateY = 0 - (float)(Math.Sin(Math.PI / 180.0d * this.Direction) * this.Depth);

                    Transform2DEffect finalEffect = new Transform2DEffect()
                    {
                        Source = new ShadowEffect()
                        {
                            Source = cl,
                            BlurAmount = 2,// 阴影模糊参数，越大越发散，感觉 2 足够了。
                            ShadowColor = this.GetShadowColor()
                        },
                        TransformMatrix = Matrix3x2.CreateTranslation(translateX, translateY)
                    };

                    args.DrawingSession.DrawImage(finalEffect);
                }
            }
        }

        private async void Content_LayoutUpdated(object sender, object e)
        {
            if (DesignMode.DesignModeEnabled || this.Visibility == Visibility.Collapsed || this.Content.Visibility == Visibility.Collapsed)
            {
                // DesignMode 不能调用 RenderAsync 方法。
                // 控件自身隐藏或者内容隐藏时也不能调用 RenderAsync 方法。
                this._pixels = null;
                this._pixelWidth = 0;
                this._pixelHeight = 0;
            }
            else
            {
                try
                {
                    RenderTargetBitmap bitmap = new RenderTargetBitmap();
                    await bitmap.RenderAsync(this.Content);

                    int pixelWidth = bitmap.PixelWidth;
                    int pixelHeight = bitmap.PixelHeight;
                    if (bitmap.PixelWidth > 0 && bitmap.PixelHeight > 0)
                    {
                        this._pixels = (await bitmap.GetPixelsAsync()).ToArray();
                        this._pixelWidth = pixelWidth;
                        this._pixelHeight = pixelHeight;
                    }
                    else
                    {
                        // 内容宽或高为 0 时不能调用 GetPixelAsync 方法。
                        this._pixels = null;
                        this._pixelWidth = pixelWidth;
                        this._pixelHeight = pixelHeight;
                    }
                }
                catch(Exception)
                {

                }
                
            }

            if (this._canvas != null)
            {
                // 请求重绘。
                this._canvas.Invalidate();
            }
        }

        private void ExpendCanvas()
        {
            if (this._canvas != null)
            {
                // 扩展 Canvas 以确保阴影能够显示。
                this._canvas.Margin = new Thickness(0 - (this.Depth + 10));
            }
        }

        private Color GetShadowColor()
        {
            if (this.Content.Visibility == Visibility.Collapsed)
            {
                return Colors.Transparent;
            }
            // 阴影透明度应该受内容的 Opacity 属性影响。
            double alphaProportion = Math.Max(0, Math.Min(1, this.Content.Opacity));
            return Color.FromArgb((byte)(Color.A * alphaProportion), Color.R, Color.G, Color.B);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (this._canvas != null)
            {
                this._canvas.RemoveFromVisualTree();
                this._canvas = null;
            }
        }
    }
}
