namespace XUI.Platform {
    public class Host
#if WINDOWS
        : Platforms.Windows.WindowsHost
#elif ANDROID
        : Platforms.Android.AndroidHost
#elif IOS
        : Platforms.IOS.IOSHost
#elif MACCATALYST
        : Platforms.MacOS.MacOSHost
#endif
    {
        public Views.View Content { get; set; }
        public Host() {
            render = drawView;
        }

        private void drawView(Graphics.Canvas canvas) => Content?.Draw(canvas);
    }
}
