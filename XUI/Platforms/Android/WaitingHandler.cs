using Android.OS;
using Java.Lang;

namespace XUI.Platform.AndroidInternal {
    public class WaitingHandler : Handler {
        IRunnable Target;

        System.Threading.ManualResetEventSlim TargetHandled = new(false);

        public WaitingHandler(Looper looper) : base(looper) {
        }

        public WaitingHandler(Looper looper, ICallback callback) : base(looper, callback) {
        }

        public override void DispatchMessage(Message msg) {
            if (Target != null) {
                IRunnable callback = msg.Callback;
                if (callback != null && Target == callback) {
                    base.DispatchMessage(msg);
                    TargetHandled.Set();
                    return;
                }
            }
            base.DispatchMessage(msg);
        }

        public override void HandleMessage(Message msg) {
            if (Target != null) {
                IRunnable callback = msg.Callback;
                if (callback != null && Target == callback) {
                    base.HandleMessage(msg);
                    TargetHandled.Set();
                    return;
                }
            }
            base.HandleMessage(msg);
        }

        public void PostAndWait(System.Action action) => PostAndWait(new Runnable(action));

        public void PostAndWait(IRunnable runnable) {
            Target = runnable;
            Post(runnable);
            // The caller of this method blocks indefinitely until the current instance is set.
            //   The caller will return immediately if the event is currently in a set state.
            TargetHandled.Wait();
            TargetHandled.Reset();
            Target = null;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                TargetHandled.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}