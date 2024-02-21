using System;
using XUI.Platform.AndroidInternal;

namespace XUI.Platforms.Android {
    public class AndroidHost : Platform.PlatformView<EGLView> {
        public AndroidHost() : base(() => new EGLView(Context)) {
        }

        protected Action<AndroidCanvas> render { get => NativeView.render; set => NativeView.render = value; }
    }
}