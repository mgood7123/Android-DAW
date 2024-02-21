using Android.OS;
using Android.Views;
using Java.Lang;

namespace XUI.Platform.AndroidInternal {
    // https://github.com/mousebird-consulting-inc/WhirlyGlobe/blob/07a710bf6deffda8222a7a7864eb855e95aa73b3/android/library/maply/src/main/java/com/mousebird/maply/MetroThread.java
    internal class RenderThread {
        internal class RenderThreadActual : HandlerThread {
            internal class RenderPost : Object, Choreographer.IFrameCallback {
                internal RenderThreadActual renderThread;
                internal bool continuous;
                internal System.DateTime start;
                internal System.TimeSpan render_time;
                public void DoFrame(long frameTimeNanos) {
                    if (renderThread.OnDoFrame != null) {
                        start = System.DateTime.Now;
                        renderThread.OnDoFrame(frameTimeNanos);
                        render_time = System.DateTime.Now - start;
                    }
                    if (continuous) {
                        renderThread.postFrameCallback();
                    }
                }
            }

            internal RenderPost renderPost = new();

            internal WaitingHandler handler;
            public RenderThreadActual(string name) : base(name) {
                renderPost.renderThread = this;
            }

            public System.Action<long> OnDoFrame;

            internal bool isContinuous;

            internal void SetRenderMode(bool continuous) {
                isContinuous = continuous;
                if (renderPost.continuous != isContinuous) {
                    renderPost.continuous = isContinuous;
                    if (isContinuous) {
                        handler?.Post(postFrameCallback);
                    } else {
                        handler?.PostAndWait(removeFrameCallback);
                    }
                }
            }

            internal void postFrameCallback() {
                removeFrameCallback();
                Choreographer.Instance.PostFrameCallback(renderPost);
            }

            internal void postFrameCallbackInvalidate() {
                if (!isContinuous) {
                    postFrameCallback();
                }
            }

            internal void removeFrameCallback() {
                Choreographer.Instance.RemoveFrameCallback(renderPost);
            }
        }

        readonly RenderThreadActual renderThread = new("RenderThread");

        public System.Action<long> OnDoFrame { get => renderThread.OnDoFrame; set => renderThread.OnDoFrame = value; }

        public void Start() {
            renderThread.Start();
            renderThread.handler = new(renderThread.Looper);
            if (renderThread.isContinuous) {
                renderThread.handler.Post(renderThread.postFrameCallback);
            } else {
                renderThread.handler.PostAndWait(renderThread.removeFrameCallback);
            }
        }

        public void SetRenderMode(bool continuous) => renderThread.SetRenderMode(continuous);

        public void Invalidate() => renderThread.postFrameCallbackInvalidate();
        public void CancelInvalidate() => renderThread.postFrameCallbackInvalidate();
        public System.TimeSpan RenderTime => renderThread.renderPost.render_time;

        public void Stop() {
            renderThread.QuitSafely();
            renderThread.Join();
        }

        public WaitingHandler Handler => renderThread.handler;
        public void Post(Runnable runnable) => Handler.Post(runnable);
        public void Post(System.Action action) => Handler.Post(action);
        public void PostAndWait(Runnable runnable) => Handler.PostAndWait(runnable);
        public void PostAndWait(System.Action action) => Handler.PostAndWait(action);
    }
}