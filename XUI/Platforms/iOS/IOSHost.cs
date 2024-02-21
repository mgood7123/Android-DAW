using System;
using XUI.Graphics;

namespace XUI.Platforms.IOS {
    public class IOSHost : Platform.PlatformView<UIKit.UILabel> {
        public IOSHost() : base(() => new()) {
            //d2d = new();
            //d2d.BindToControl(NativeView, this);
            //d2d.Render = OnRender;
            Loaded += Host_Loaded;
        }

        private void Host_Loaded(object sender, EventArgs e) {
            NativeView.Text = "XUI Host does not support your platform.";
        }

        //private void OnRender(ID2D1RenderTarget renderTarget) {
        //    if (render != null) {
        //        renderTarget.BeginDraw();
        //        render(new Windows.Canvas(renderTarget));
        //        renderTarget.EndDraw();
        //    }
        //}

        protected Action<Canvas> render;
    }
}
