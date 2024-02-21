using XUI.Graphics;

namespace XUI.Views {

    public class View {
        public View() {}

        internal void Draw(Canvas canvas) {
            OnDraw(canvas);
        }

        protected virtual void OnDraw(Canvas canvas) {}
    }
}