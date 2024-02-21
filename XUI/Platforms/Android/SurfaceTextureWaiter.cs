using Android.Graphics;

namespace XUI.Platform.AndroidInternal {
    using static LogError;
    internal class SurfaceTextureWaiter : Java.Lang.Object, Android.Graphics.SurfaceTexture.IOnFrameAvailableListener {
        System.Threading.ManualResetEventSlim TargetHandled = new(false);
        public void OnFrameAvailable(SurfaceTexture surfaceTexture) {
            Error("FRAME AVAILABLE");
            TargetHandled.Set();
        }

        public void WaitForFrame() {
            TargetHandled.Wait();
            TargetHandled.Reset();
        }
    }
}