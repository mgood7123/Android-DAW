using System;
using Vortice.Direct2D1;
using Vortice.DXGI;
using XUI.Utils;

namespace XUI.Platforms.Windows.DirectX {
    public class Direct2D : DX11 {
        internal IDXGISurface iDXGISurface;
        internal ID2D1Factory1 iD2D1Factory1;
        internal ID2D1RenderTarget iD2D1RenderTarget;

        /// <summary>
        /// Raised when the control requests a new frame.
        /// </summary>
        new public Action<ID2D1RenderTarget> Render;

        public Action<ID2D1RenderTarget> RenderTargetCreate;

        /// <summary>
        /// Raises the RenderTargetCreate event.
        /// </summary>
        /// <param name="newRenderTarget">New Direct2D render target.</param>
        internal void OnRenderTargetCreate(ID2D1RenderTarget newRenderTarget) {
            iD2D1RenderTarget = newRenderTarget;

            if (RenderTargetCreate != null) {
                RenderTargetCreate(iD2D1RenderTarget);
            }
        }

        public Action BeforeRenderTargetDispose;

        /// <summary>
        /// Raises the RenderTargetCreate event.
        /// </summary>
        internal void OnBeforeRenderTargetDispose() {
            if (BeforeRenderTargetDispose != null) {
                BeforeRenderTargetDispose();
            }
        }

        /// <summary>
        /// Raises the Render event.
        /// </summary>
        internal override void OnRender() {
            if (Render != null) {
                Render(iD2D1RenderTarget);
            }
        }

        protected override void CreateDeviceDependentResources() {
            base.CreateDeviceDependentResources();
            iD2D1Factory1 = D2D1.D2D1CreateFactory<ID2D1Factory1>(FactoryType.SingleThreaded, DebugHelper.IsDebugBuild ? DebugLevel.Information : DebugLevel.None);
        }

        protected override void ReleaseDeviceDependentResources() {
            iD2D1Factory1?.Dispose();
            base.ReleaseDeviceDependentResources();
        }

        protected override void CreateSizeDependentResources() {
            base.CreateSizeDependentResources();
            iDXGISurface = BackBuffer.QueryInterface<IDXGISurface>();
            iD2D1RenderTarget = iD2D1Factory1.CreateDxgiSurfaceRenderTarget(iDXGISurface, new RenderTargetProperties {
                PixelFormat = new Vortice.DCommon.PixelFormat(iDXGISurface.Description.Format, Vortice.DCommon.AlphaMode.Premultiplied)
            });
            OnRenderTargetCreate(iD2D1RenderTarget);
        }

        protected override void ReleaseSizeDependentResources() {
            OnBeforeRenderTargetDispose();
            iD2D1RenderTarget?.Dispose();
            iDXGISurface?.Dispose();
            base.ReleaseSizeDependentResources();
        }
    }
}
