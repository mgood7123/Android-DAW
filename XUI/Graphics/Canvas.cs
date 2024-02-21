namespace XUI.Graphics {
    public abstract class Canvas {
        public float Width => getWidth();
        public float Height => getHeight();
        public float Density => getDensity();

        public abstract bool IsHardwareAccelerated { get; }

        protected abstract float getWidth();
        protected abstract float getHeight();
        protected abstract float getDensity();

        public abstract void OnResize(float width, float height);
        public abstract void OnCreateResources();
        public abstract void OnDisposeResources();

        public abstract void Clear();
        public abstract void Clear(Color4 color);
        public abstract void SetColor(Color4 color);
        public abstract void DrawText(string text, int x, int y, int w, int h);
        public abstract void Translate(float x, float y);
        public abstract void ClipRelativeRect(float x, float y, float w, float h);
        public abstract void DrawRect(float x, float y, float w, float h);
        public abstract int Save();
        public abstract void Restore(int save);
    }
}