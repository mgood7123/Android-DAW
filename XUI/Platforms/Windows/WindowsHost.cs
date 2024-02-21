using System;
using Vortice.Direct2D1;

namespace XUI {
    namespace Platforms.Windows {
        public class WindowsHost : Platform.PlatformView<Microsoft.UI.Xaml.Controls.SwapChainPanel> {
            DirectX.Direct2D d2d;
            WindowsCanvas canvas;
            public WindowsHost() : base(() => new Microsoft.UI.Xaml.Controls.SwapChainPanel()) {
                d2d = new();
                canvas = new();
                d2d.BindToControl(NativeView, this);
                d2d.Render = OnRender;
                d2d.RenderTargetCreate = OnRenderTargetCreate;
                d2d.BeforeRenderTargetDispose = OnBeforeRenderTargetDispose;
            }

            private void OnRenderTargetCreate(ID2D1RenderTarget renderTarget) {
                canvas.renderTarget = renderTarget;
                canvas.factory = d2d.iD2D1Factory1;
                canvas.density = d2d.Density;
                canvas.OnResize(d2d.Width, d2d.Height);
                canvas.OnCreateResources();
            }

            private void OnBeforeRenderTargetDispose() {
                canvas.OnDisposeResources();
                canvas.density = 0;
                canvas.factory = null;
                canvas.renderTarget = null;
            }

            private void OnRender(ID2D1RenderTarget renderTarget) {
                renderTarget.BeginDraw();
                canvas.Clear(XUI.Graphics.Colors.Transparent);
                if (render != null) {
                    int s = canvas.Save();
                    render(canvas);
                    canvas.Restore(s);
                }
                renderTarget.EndDraw();
            }

            protected Action<Graphics.Canvas> render;
        }
    }
}