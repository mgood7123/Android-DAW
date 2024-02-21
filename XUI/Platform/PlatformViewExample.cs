namespace XUI.Platform {
#if WINDOWS
    public class PlatformViewExample : PlatformView<Microsoft.UI.Xaml.Controls.TextBlock> {
        public PlatformViewExample() : base(() => new Microsoft.UI.Xaml.Controls.TextBlock()) { }
#elif ANDROID
    public class PlatformViewExample : PlatformView<Android.Widget.TextView> {
        public PlatformViewExample() : base(() => new Android.Widget.TextView(Context)) {}
#elif IOS || MACCATALYST
    public class PlatformViewExample : PlatformView<UIKit.UILabel> {
        public PlatformViewExample() : base(() => new UIKit.UILabel()) {}
#endif
        public string Text {
            get => NativeView.Text;
            set {
                NativeView.Text = value ?? "";
            }
        }
    }
}
