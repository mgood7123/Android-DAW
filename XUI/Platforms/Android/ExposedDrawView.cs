using Android.Content;
using Android.Runtime;
using Android.Util;
using System;
using ACanvas = Android.Graphics.Canvas;
using AView = Android.Views.View;
using ASurfaceView = Android.Views.SurfaceView;

namespace XUI {
    namespace Platforms {
        namespace Android {
            public class ExposedDrawView : AView {
                public Action<ACanvas, float, float> Render;

                public ExposedDrawView(Context context) : base(context) { }
                public ExposedDrawView(Context context, IAttributeSet attrs) : base(context, attrs) { }
                public ExposedDrawView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }
                public ExposedDrawView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) { }
                protected ExposedDrawView(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

                public sealed override void Draw(ACanvas canvas) {
                    if (Render != null) {
                        Render(canvas, canvas.Width, canvas.Height);
                    }
                }
            }
        }
    }
}