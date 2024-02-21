using XUI.Graphics;

namespace XUI.Views {
    public class RepeatDrawView : View {
        public View Content;
        public int Repeat;

        public RepeatDrawView() {
            Repeat = 1;;
        }

        protected override void OnDraw(Canvas canvas) {
            if (Content != null) {
                for (int i = 0; i < Repeat; i++) {
                    Content.Draw(canvas);
                }
            }
        }
    }
}