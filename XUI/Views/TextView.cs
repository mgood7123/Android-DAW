using Microsoft.Maui.Platform;
using XUI.Graphics;

namespace XUI.Views {
    public class TextView : View {
        public string Text { get; set; }
        public Color4 TextColor { get; set; }

        public TextView() {
            TextColor = Colors.Black;
        }

        // clip-rect and translations are complicated in android
        // we can clip by 50 but we cannot clip by -50 unless we
        // restore to a previous clip/matrix point
        // clip is relative to the current translation as well
        //
        // we must handle these on D2D side as well

        protected override void OnDraw(Canvas canvas) {
            canvas.SetColor(TextColor);
            canvas.DrawText(Text, 0, 0, (int)canvas.Width, (int)canvas.Height);
            //canvas.Clear(Colors.AliceBlue);
            //int a = canvas.Save();
            //canvas.Translate(50, 50);
            //canvas.ClipRelativeRect(50, 50, 100, 100);
            //canvas.Clear(Colors.Purple);
            //canvas.Restore(a);
            //int b = canvas.Save();
            //canvas.ClipRelativeRect(0, 0, 50, 50);
            //canvas.SetColor(TextColor);
            //canvas.DrawText(Text, 0, 0, (int)canvas.Width, (int)canvas.Height);
            //canvas.Restore(b);
            //canvas.Translate(0, 200);
            //canvas.DrawText("canvas.Density : " + canvas.Density, 0, 0, (int)canvas.Width, (int)canvas.Height);
            //canvas.Translate(0, 100);
            //canvas.DrawText("canvas.IsHardwareAccelerated : " + canvas.IsHardwareAccelerated, 0, 0, (int)canvas.Width, (int)canvas.Height);
        }
    }
}