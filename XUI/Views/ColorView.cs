using XUI.Graphics;

namespace XUI.Views {
    public class ColorView : View {
        public Color4 Color { get; set; }

        public ColorView() {
            Color = Colors.AliceBlue;
        }

        protected override void OnDraw(Canvas canvas) {
            canvas.Clear(Color);
        }
    }
}