using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using SharpGen.Runtime;
using Windows.Foundation;
using XUI.Utils;
using System;

using static Vortice.Direct3D11.D3D11;
using static Vortice.DXGI.DXGI;

using ISwapChainBackgroundPanelNative = Vortice.WinUI.ISwapChainBackgroundPanelNative;
using ISwapChainPanelNative = Vortice.WinUI.ISwapChainPanelNative;

namespace XUI.Platforms.Windows.DirectX {
    public class DX11 : IDisposable {
        internal ID3D11Device iD3D11Device;
        internal ID3D11DeviceContext iD3D11DeviceContext;
        internal ID3D11RenderTargetView iD3D11RenderTargetView;
        internal ID3D11DepthStencilView iD3D11DepthStencilView;
        internal ID3D11Texture2D iD3D11Texture2D;
        internal Size backBufferSize;
        internal IDXGIFactory2 iDXGIFactory2;
        internal IDXGIAdapter1 iDXGIAdapter1;

        /// <summary>
        /// Raised when the Direct3D device is recreated.
        /// </summary>
        public Action<ID3D11Device, ID3D11DeviceContext> DeviceReset;

        /// <summary>
        /// Raised when the control requests a new frame.
        /// </summary>
        public Action<ID3D11DeviceContext, float, float> Render;

        /// <summary>
        /// The Direct3D device.
        /// </summary>
        public ID3D11Device D3DDevice {
            get {
                ThrowIfDisposed();

                return iD3D11Device;
            }
            private set {
                iD3D11Device = value;
            }
        }

        /// <summary>
        /// The Direct3D device context.
        /// </summary>
        public ID3D11DeviceContext D3DContext {
            get {
                ThrowIfDisposed();

                return iD3D11DeviceContext;
            }
            private set {
                iD3D11DeviceContext = value;
            }
        }

        /// <summary>
        /// The render target view that holds the reference to the current back buffer.
        /// </summary>
        public ID3D11RenderTargetView BackBufferView {
            get {
                ThrowIfDisposed();

                return iD3D11RenderTargetView;
            }
            internal set {
                iD3D11RenderTargetView = value;
            }
        }

        /// <summary>
        /// The depth stencil target view that holds the reference to the current depth stencil buffer.
        /// </summary>
        public ID3D11DepthStencilView DepthStencilView {
            get {
                ThrowIfDisposed();

                return iD3D11DepthStencilView;
            }
            internal set {
                iD3D11DepthStencilView = value;
            }
        }

        /// <summary>
        /// The texture of the current back buffer.
        /// </summary>
        public ID3D11Texture2D BackBuffer {
            get {
                ThrowIfDisposed();

                return iD3D11Texture2D;
            }
            internal set {
                iD3D11Texture2D = value;
            }
        }

        /// <summary>
        /// The size in pixels of the current back buffer.
        /// </summary>
        public Size BackBufferSize {
            get {
                ThrowIfDisposed();

                return backBufferSize;
            }
            internal set {
                backBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets if the instance has been disposed.
        /// </summary>
        public bool IsDisposed {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets if the instance has been bound to a control.
        /// </summary>
        public bool IsBound {
            get;
            protected set;
        }

        /// <summary>
        /// Raises the DeviceReset event.
        /// </summary>
        /// <param name="newDevice">New Direct3D11 device.</param>
        /// <param name="newContext">New Direct3D11 device context.</param>
        internal void OnDeviceReset(ID3D11Device newDevice, ID3D11DeviceContext newContext) {
            D3DDevice = newDevice;
            D3DContext = newContext;

            if (DeviceReset != null) {
                DeviceReset(iD3D11Device, iD3D11DeviceContext);
            }
        }

        /// <summary>
        /// Raises the Render event.
        /// </summary>
        internal virtual void OnRender() {
            if (Render != null) {
                Render(iD3D11DeviceContext, Width, Height);
            }
        }

        /// <summary>
        /// Releases all resources used by this object.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Throws an exception if the object has been disposed.
        /// </summary>
        protected void ThrowIfDisposed() {
            if (IsDisposed) {
                throw new ObjectDisposedException("The object has already been disposed.");
            }
        }

        /// <summary>
        /// Throws an exception if the object has already been bound.
        /// </summary>
        protected void ThrowIfBound() {
            if (IsBound) {
                throw new InvalidOperationException("This instance has already been bound to a control.");
            }
        }

        /// <summary>
        /// Minimum back buffer width/height, because sometimes bound controls aren't measured yet and their values are 0.
        /// </summary>
        internal static readonly float MIN_BACKBUFFER_DIMENSION = 1.0f;

        internal IDXGISwapChain1 swapChain;
        internal SwapChainBackgroundPanel backgroundPanel;
        internal SwapChainPanel panel;
        internal ISwapChainBackgroundPanelNative nativeBackgroundPanel;
        internal ISwapChainPanelNative nativePanel;
        internal FeatureLevel FeatureLevel;
        float density;
        Microsoft.Maui.Controls.Window window;
        View view;
        public float Width { get; internal set; }
        public float Height { get; internal set; }
        public float Density { get => density; }

        /// <summary>
        /// Creates a new DX11 instance.
        /// </summary>
        public DX11() {
            IsDisposed = false;
            IsBound = false;
        }

        /// <summary>
        /// Binds the object to a SwapChainBackgroundPanel and initializes Direct3D11 resources.
        /// </summary>
        /// <param name="backgroundPanel">SwapChainBackgroundPanel control used for drawing.</param>
        public void BindToControl(SwapChainBackgroundPanel backgroundPanel, View view) {
            ThrowIfDisposed();
            ThrowIfBound();

            this.backgroundPanel = backgroundPanel;
            this.view = view;

            view.Loaded += View_LoadedBackgroundPanel;
            view.Unloaded += View_LoadedBackgroundPanel;

            if (view.IsLoaded) {
                View_LoadedBackgroundPanel(null, null);
            }

            IsBound = true;
        }

        /// <summary>
        /// Binds the object to a SwapChainPanel and initializes Direct3D11 resources.
        /// </summary>
        /// <param name="panel">SwapChainPanel control used for drawing.</param>
        public void BindToControl(SwapChainPanel panel, View view) {
            ThrowIfDisposed();
            ThrowIfBound();

            this.panel = panel;
            this.view = view;

            view.Loaded += View_LoadedPanel;
            view.Unloaded += View_UnloadedPanel;

            if (view.IsLoaded) {
                View_LoadedPanel(null, null);
            }

            IsBound = true;
        }

        private void View_LoadedBackgroundPanel(object sender, EventArgs e) {
            window = view.Window;

            density = window.DisplayDensity;

            nativeBackgroundPanel = ComObject.As<ISwapChainBackgroundPanelNative>(backgroundPanel);
            UpdateBackBufferSize();

            CreateDeviceDependentResources();
            CreateSizeDependentResources();

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            backgroundPanel.SizeChanged += HostControl_SizeChanged;
            window.DisplayDensityChanged += DensityChanged;
        }

        private void View_UnloadedBackgroundPanel(object sender, EventArgs e) {
            window.DisplayDensityChanged -= DensityChanged;
            backgroundPanel.SizeChanged -= HostControl_SizeChanged;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;

            ReleaseSizeDependentResources();
            ReleaseDeviceDependentResources();

            nativePanel = null;
            window = null;
        }

        private void View_LoadedPanel(object sender, EventArgs e) {
            window = view.Window;

            density = window.DisplayDensity;

            nativePanel = ComObject.As<ISwapChainPanelNative>(panel);
            UpdateBackBufferSize();

            CreateDeviceDependentResources();
            CreateSizeDependentResources();

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            panel.SizeChanged += HostControl_SizeChanged;
            window.DisplayDensityChanged += DensityChanged;
        }

        private void View_UnloadedPanel(object sender, EventArgs e) {
            window.DisplayDensityChanged -= DensityChanged;
            panel.SizeChanged -= HostControl_SizeChanged;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;

            ReleaseSizeDependentResources();
            ReleaseDeviceDependentResources();

            nativePanel = null;
            window = null;
        }

        private void BackgroundPanel_Loaded(object sender, RoutedEventArgs e) {
            CreateDeviceDependentResources();
            CreateSizeDependentResources();

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            backgroundPanel.SizeChanged += HostControl_SizeChanged;
            window.DisplayDensityChanged += DensityChanged;
        }

        private void BackgroundPanel_Unloaded(object sender, RoutedEventArgs e) {
            window.DisplayDensityChanged -= DensityChanged;
            backgroundPanel.SizeChanged -= HostControl_SizeChanged;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            ReleaseSizeDependentResources();
            ReleaseDeviceDependentResources();

        }

        /// <summary>
        /// Called when the composition target request a rendering operation.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Parameters.</param>
        private void CompositionTarget_Rendering(object sender, object e) {
            OnRender();

            PresentParameters parameters = new();

            Result result = swapChain.Present(1, PresentFlags.None, parameters);
            if (result.Failure) {
                if (result.Code == Vortice.DXGI.ResultCode.DeviceRemoved.Code || result.Code == Vortice.DXGI.ResultCode.DeviceReset.Code) {
                    CreateDeviceDependentResources();
                    CreateSizeDependentResources();
                } else {
                    throw new SharpGenException(result.Description);
                }
            }
        }

        internal static readonly FeatureLevel[] s_featureLevels = [
            FeatureLevel.Level_11_1,
            FeatureLevel.Level_11_0,
            FeatureLevel.Level_10_1,
            FeatureLevel.Level_10_0
        ];

        internal IDXGIAdapter1 GetHardwareAdapter() {
            if (iDXGIFactory2 != null) {
                IDXGIFactory6 iDXGIFactory6 = iDXGIFactory2.QueryInterface<IDXGIFactory6>();
                if (iDXGIFactory6 != null) {
                    for (int adapterIndex = 0;
                        iDXGIFactory6.EnumAdapterByGpuPreference(adapterIndex, GpuPreference.HighPerformance, out IDXGIAdapter1 adapter).Success;
                        adapterIndex++) {
                        if (adapter == null) {
                            continue;
                        }

                        AdapterDescription1 desc = adapter.Description1;

                        if ((desc.Flags & AdapterFlags.Software) != AdapterFlags.None) {
                            // Don't select the Basic Render Driver adapter.
                            adapter.Dispose();
                            continue;
                        }
                        iDXGIFactory6.Dispose();
                        return adapter;
                    }
                }
                iDXGIFactory6.Dispose();
                for (int adapterIndex = 0;
                    iDXGIFactory2.EnumAdapters1(adapterIndex, out IDXGIAdapter1 adapter).Success;
                    adapterIndex++) {
                    AdapterDescription1 desc = adapter.Description1;

                    if ((desc.Flags & AdapterFlags.Software) != AdapterFlags.None) {
                        // Don't select the Basic Render Driver adapter.
                        adapter.Dispose();

                        continue;
                    }

                    return adapter;
                }
                throw new InvalidOperationException("Cannot detect D3D11 adapter");
            }
            throw new InvalidOperationException("D3D11 adapter factory is null");
        }

        /// <summary>
        /// Creates resources that depend on the current device.
        /// </summary>
        protected virtual void CreateDeviceDependentResources() {
            ReleaseDeviceDependentResources();

            iDXGIFactory2 = CreateDXGIFactory2<IDXGIFactory2>(DebugHelper.IsDebugBuild);

            iDXGIAdapter1 = GetHardwareAdapter();
#if DEBUG
            DeviceCreationFlags creationFlags = DeviceCreationFlags.Debug;
#else
            DeviceCreationFlags creationFlags = DeviceCreationFlags.None;
#endif
            creationFlags |= DeviceCreationFlags.BgraSupport;
#if DEBUG
            if (SdkLayersAvailable()) {
                creationFlags |= DeviceCreationFlags.Debug;
            }
#endif

            if (D3D11CreateDevice(
                iDXGIAdapter1,
                DriverType.Unknown,
                creationFlags,
                s_featureLevels,
                out ID3D11Device newDevice, out FeatureLevel, out ID3D11DeviceContext newContext).Failure) {
                // If the initialization fails, fall back to the WARP device.
                // For more information on WARP, see:
                // http://go.microsoft.com/fwlink/?LinkId=286690
                D3D11CreateDevice(
                    nint.Zero,
                    DriverType.Warp,
                    creationFlags,
                    s_featureLevels,
                    out newDevice, out FeatureLevel, out newContext).CheckError();
            }

            OnDeviceReset(newDevice, newContext);
        }

        /// <summary>
        /// Releases resources that depend on the current device.
        /// </summary>
        protected virtual void ReleaseDeviceDependentResources() {
            iDXGIAdapter1?.Dispose();
            iD3D11Device?.Dispose();
            iD3D11DeviceContext?.Dispose();
            iDXGIFactory2?.Dispose();
        }

        /// <summary>
        /// Releases resources that depend on the current back buffer size.
        /// </summary>
        protected virtual void ReleaseSizeDependentResources() {
            iD3D11Texture2D?.Dispose();
            iD3D11RenderTargetView?.Dispose();
            iD3D11DepthStencilView?.Dispose();
        }

        /// <summary>
        /// Creates resources that depend on the current back buffer size.
        /// </summary>
        protected virtual void CreateSizeDependentResources() {
            ReleaseSizeDependentResources();

            int w = (int)Width;
            int h = (int)Height;

            if (swapChain != null) {
                swapChain.ResizeBuffers(2, w, h, Format.B8G8R8A8_UNorm, SwapChainFlags.None);
            } else {
                SwapChainDescription1 swapChainDescription = new() {
                    Width = w,
                    Height = h,
                    Format = Format.B8G8R8A8_UNorm,
                    Stereo = false,
                    SampleDescription = new SampleDescription(1, 0),
                    BufferUsage = Usage.Backbuffer | Usage.RenderTargetOutput,
                    BufferCount = 2,
                    Scaling = Scaling.Stretch,
                    SwapEffect = SwapEffect.FlipSequential,
                };

                using IDXGIDevice2 dxgiDevice2 = iD3D11Device.QueryInterface<IDXGIDevice2>();
                using IDXGIAdapter dxgiAdapter = dxgiDevice2.GetAdapter();
                using IDXGIFactory2 dxgiFactory2 = dxgiAdapter.GetParent<IDXGIFactory2>();
                swapChain = dxgiFactory2.CreateSwapChainForComposition(iD3D11Device, swapChainDescription);

                if (backgroundPanel != null) {
                    nativeBackgroundPanel.SetSwapChain(swapChain);
                } else if (panel != null) {
                    nativePanel.SetSwapChain(swapChain);
                }

                dxgiDevice2.MaximumFrameLatency = 1;
            }

            BackBuffer = swapChain.GetBuffer<ID3D11Texture2D>(0);
            BackBufferView = iD3D11Device.CreateRenderTargetView(BackBuffer);
            UpdateBackBufferSize();

            using (ID3D11Texture2D depthBuffer = iD3D11Device.CreateTexture2D(new Texture2DDescription() {
                Format = Format.D24_UNorm_S8_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = w,
                Height = h,
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.DepthStencil,
            })) {
                DepthStencilView = iD3D11Device.CreateDepthStencilView(depthBuffer, new DepthStencilViewDescription() {
                    ViewDimension = DepthStencilViewDimension.Texture2D
                });
            }

            iD3D11DeviceContext.RSSetViewport(0, 0, Width, Height, 0.0f, 1.0f);
        }

        /// <summary>
        /// Disposes this object's resources.
        /// </summary>
        /// <param name="disposing">true if releasing managed resources.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                swapChain?.Dispose();

                window.DisplayDensityChanged -= DensityChanged;

                CompositionTarget.Rendering -= CompositionTarget_Rendering;

                if (nativeBackgroundPanel != null) {
                    nativeBackgroundPanel?.Dispose();
                    backgroundPanel.SizeChanged -= HostControl_SizeChanged;
                }

                if (nativePanel != null) {
                    nativePanel?.Dispose();
                    panel.SizeChanged -= HostControl_SizeChanged;
                }

                backgroundPanel = null;
                panel = null;

                ReleaseDeviceDependentResources();
                ReleaseSizeDependentResources();
            }

            IsBound = false;
            IsDisposed = true;
        }

        /// <summary>
        /// Called when the size of the host control changes.
        /// </summary>
        /// <param name="sender">Sender control.</param>
        /// <param name="e">Event arguments.</param>
        private void HostControl_SizeChanged(object sender, SizeChangedEventArgs e) {
            UpdateBackBufferSize();
            CreateSizeDependentResources();
        }

        /// <summary>
        /// Called when the DPI of the current display change.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="args">Event arguments.</param>
        private void DensityChanged(object sender, DisplayDensityChangedEventArgs e) {
            density = e.DisplayDensity;
            UpdateBackBufferSize();
            CreateSizeDependentResources();
        }

        /// <summary>
        /// Calculates the correct back buffer size, taking into account display DPI and hosted control size.
        /// </summary>
        private void UpdateBackBufferSize() {
            if (backgroundPanel != null) {
                Width = (float)Math.Max(backgroundPanel.ActualWidth * density, MIN_BACKBUFFER_DIMENSION);
                Height = (float)Math.Max(backgroundPanel.ActualHeight * density, MIN_BACKBUFFER_DIMENSION);
                BackBufferSize = new Size(Width, Height);
            }

            if (panel != null) {
                Width = (float)Math.Max(panel.ActualWidth * density, MIN_BACKBUFFER_DIMENSION);
                Height = (float)Math.Max(panel.ActualHeight * density, MIN_BACKBUFFER_DIMENSION);
                BackBufferSize = new Size(Width, Height);
            }
        }
    }
}