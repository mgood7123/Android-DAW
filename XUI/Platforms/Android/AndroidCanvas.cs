using Android.Content;
using Android.Widget;
using ACanvas = Android.Graphics.Canvas;
using APaint = Android.Graphics.Paint;
using ALayoutParams = Android.Views.ViewGroup.LayoutParams;
using Android.Graphics;

namespace XUI.Platforms.Android {
    public class AndroidCanvas : Graphics.Canvas {
        internal ACanvas hostCanvas;
        internal ACanvas canvas;
        internal Picture pic;
        internal APaint paint;
        internal TextView textView;
        internal Context context;
        internal float w;
        internal float h;
        internal float density;

        public override bool IsHardwareAccelerated => hostCanvas.IsHardwareAccelerated;

        protected override float getDensity() => density;

        public override void OnCreateResources() {
            paint = new();
            textView = new(context);
        }

        public override void OnDisposeResources() {
            textView?.Dispose();
            paint?.Dispose();
        }

        public override void OnResize(float w, float h) {
            this.w = w;
            this.h = h;
        }

        public override void Translate(float dx, float dy) {
            canvas.Translate(dx, dy);
        }

        protected override float getHeight() => h;
        protected override float getWidth() => w;

        // canvas.DrawPaint/DrawColor/DrawARGB does not respect the current clip
        public override void Clear() => DrawRect(0, 0, Width, Height);

        public override void Clear(Graphics.Color4 color) {
            var old = paint.Color;
            paint.Color = color.ToPlatformColor();
            Clear();
            paint.Color = old;
        }

        public override void DrawText(string text, int x, int y, int w, int h) {
            textView.LayoutParameters = new ALayoutParams(w, h);
            textView.Text = text;
            textView.Invalidate();
            textView.Measure(w, h);
            textView.Layout(0, 0, w, h);
            int saveCount = canvas.Save();
            canvas.Translate(x, y);
            textView.Draw(canvas);
            canvas.RestoreToCount(saveCount);
        }

        public override void SetColor(Graphics.Color4 color) {
            paint.Color = color.ToPlatformColor();
        }

        public override void ClipRelativeRect(float x, float y, float w, float h) {
            canvas.ClipRect(x, y, w, h);
        }

        public override void DrawRect(float x, float y, float w, float h) {
            canvas.DrawRect(x, y, w, h, paint);
        }

        public override int Save() {
            return canvas.Save();
        }

        public override void Restore(int save) {
            canvas.RestoreToCount(save);
        }
    }
}
