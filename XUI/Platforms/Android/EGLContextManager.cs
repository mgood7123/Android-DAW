using Android.Opengl;
using System.Threading;

namespace XUI.Platform.AndroidInternal {
    class EGLContextManager {
        int[] config = new int[15];

        private EGLDisplay eglDisplay;
        private EGLConfig eglConfig;
        private EGLContext eglContext;
        private EGLSurface eglSurface;
        int[] eglVersion = new int[2];
        int[] eglNumConfigs = new int[1];
        int[] tmp_value = new int[1];
        int[] attrib_list = new int[3];

        public EGLContextManager() {
            config[0] = EGL14.EglRedSize;
            config[2] = EGL14.EglGreenSize;
            config[4] = EGL14.EglBlueSize;
            config[6] = EGL14.EglAlphaSize;
            config[8] = EGL14.EglDepthSize;
            config[10] = EGL14.EglStencilSize;
            config[12] = EGL14.EglRenderableType;
            config[14] = EGL14.EglNone;

            int EGL_CONTEXT_CLIENT_VERSION = 0x3098;

            attrib_list[0] = EGL_CONTEXT_CLIENT_VERSION;
            attrib_list[2] = EGL14.EglNone;
        }

        // Android.Opengl.GLES10
        // Android.Opengl.GLES20
        // Android.Opengl.GLES30
        // Android.Opengl.GLES31
        // Android.Opengl.GLES32
        public enum EGL_VERSION {
            EGL_VERSION_1_0,
            EGL_VERSION_2_0,
            EGL_VERSION_3_0,
            EGL_VERSION_3_1,
            EGL_VERSION_3_2,
        };

        public bool TryCreate(int redSize, int greenSize, int blueSize, int alphaSize, int depthSize, int stencilSize, EGL_VERSION version) {
            switch (version) {
                case EGL_VERSION.EGL_VERSION_1_0:
                    eglVersion[0] = 1;
                    eglVersion[1] = 0;
                    config[13] = EGL14.EglOpenglEsBit;
                    break;
                case EGL_VERSION.EGL_VERSION_2_0:
                    eglVersion[0] = 2;
                    eglVersion[1] = 0;
                    config[13] = EGL14.EglOpenglEs2Bit;
                    break;
                case EGL_VERSION.EGL_VERSION_3_0:
                    eglVersion[0] = 3;
                    eglVersion[1] = 0;
                    config[13] = EGLExt.EglOpenglEs3BitKhr;
                    break;
                case EGL_VERSION.EGL_VERSION_3_1:
                    eglVersion[0] = 3;
                    eglVersion[1] = 1;
                    config[13] = EGLExt.EglOpenglEs3BitKhr;
                    break;
                case EGL_VERSION.EGL_VERSION_3_2:
                    eglVersion[0] = 3;
                    eglVersion[1] = 2;
                    config[13] = EGLExt.EglOpenglEs3BitKhr;
                    break;
                default:
                    return false;
            }
            config[1] = redSize;
            config[3] = greenSize;
            config[5] = blueSize;
            config[7] = alphaSize;
            config[9] = depthSize;
            config[11] = stencilSize;

            eglDisplay = EGL14.EglGetDisplay(EGL14.EglDefaultDisplay);

            if (eglDisplay == EGL14.EglNoDisplay) {
                return false;
            }

            if (!EGL14.EglInitialize(eglDisplay, eglVersion, 0, eglVersion, 1)) {
                return false;
            }

            if (!EGL14.EglChooseConfig(eglDisplay, config, 0, null, 0, 0, eglNumConfigs, 0)) {
                return false;
            }

            if (eglNumConfigs[0] < 0) {
                return false;
            }

            //EGL14.EglBindAPI(EGL14.EglOpenglEsApi);

            EGLConfig[] configs = new EGLConfig[eglNumConfigs[0]];
            int[] numConfigs = new int[eglNumConfigs[0]];

            // EglChooseConfig(
            //  EGLDisplay? dpy,
            //  int[]? attrib_list,
            //  int attrib_listOffset,
            //  EGLConfig[]? configs,
            //  int configsOffset,
            //  int config_size,
            //  int[]? num_config,
            //  int num_configOffset
            // );
            // Java.Lang.IllegalArgumentException: 'length - configsOffset < config_size < needed'
            if (!EGL14.EglChooseConfig(eglDisplay, config, 0, configs, 0, eglNumConfigs[0], numConfigs, 0)) {
                return false;
            }

            bool config_found = false;
            for (int i = 0; i < configs.Length; i++) {
                if (!EGL14.EglGetConfigAttrib(eglDisplay, configs[i], EGL14.EglRedSize, tmp_value, 0) || tmp_value[0] != redSize) {
                    continue;
                }
                if (!EGL14.EglGetConfigAttrib(eglDisplay, configs[i], EGL14.EglGreenSize, tmp_value, 0) || tmp_value[0] != greenSize) {
                    continue;
                }
                if (!EGL14.EglGetConfigAttrib(eglDisplay, configs[i], EGL14.EglBlueSize, tmp_value, 0) || tmp_value[0] != blueSize) {
                    continue;
                }
                if (!EGL14.EglGetConfigAttrib(eglDisplay, configs[i], EGL14.EglAlphaSize, tmp_value, 0) || tmp_value[0] != alphaSize) {
                    continue;
                }
                if (!EGL14.EglGetConfigAttrib(eglDisplay, configs[i], EGL14.EglDepthSize, tmp_value, 0) || tmp_value[0] != depthSize) {
                    continue;
                }
                if (!EGL14.EglGetConfigAttrib(eglDisplay, configs[i], EGL14.EglStencilSize, tmp_value, 0) || tmp_value[0] != stencilSize) {
                    continue;
                }
                config_found = true;
                eglConfig = configs[i];
                break;
            }

            if (!config_found) {
                return false;
            }

            attrib_list[1] = eglVersion[0];

            eglContext = EGL14.EglCreateContext(eglDisplay, eglConfig, EGL14.EglNoContext, attrib_list, 0);

            if (eglContext == null || eglContext == EGL14.EglNoContext) {
                return false;
            }

            return true;
        }

        bool is_current = false;
        int current_thread = 0;

        public bool MakeCurrent() {
            if (!EGL14.EglMakeCurrent(eglDisplay, eglSurface, eglSurface, eglContext)) {
                return false;
            }
            current_thread = Thread.CurrentThread.ManagedThreadId;
            is_current = true;
            return true;
        }

        public bool IsCurrent => is_current && current_thread == Thread.CurrentThread.ManagedThreadId;

        public bool SwapBuffers() => EGL14.EglSwapBuffers(eglDisplay, eglSurface);

        public bool HasDisplay => eglDisplay != null && eglDisplay != EGL14.EglNoDisplay;
        public bool HasContext => eglContext != null && eglContext != EGL14.EglNoContext;
        public bool HasSurface => eglSurface != null && eglSurface != EGL14.EglNoSurface;
        public int Major => eglVersion[0];
        public int Minor => eglVersion[1];

        public bool TryDestroy() {
            if (HasContext) {
                if (HasSurface) {
                    if (!TryDetachFromSurfaceTexture()) {
                        return false;
                    }
                }
                if (!EGL14.EglDestroyContext(eglDisplay, eglContext)) {
                    return false;
                }
                eglContext = null;
            }
            if (HasDisplay) {
                if (!EGL14.EglTerminate(eglDisplay)) {
                    return false;
                }
                eglDisplay = null;
            }
            return true;
        }

        public bool TryAttachToSurfaceTexture(Android.Graphics.SurfaceTexture? surfaceTexture) {
            if (!HasContext) {
                return false;
            }
            // attrib list cannot be null
            eglSurface = EGL14.EglCreateWindowSurface(eglDisplay, eglConfig, surfaceTexture, new int[] { EGL14.EglNone }, 0);
            if (eglSurface == null || eglSurface == EGL14.EglNoSurface) {
                return false;
            }
            return true;
        }

        public bool TryDetachFromSurfaceTexture() {
            if (!HasContext) {
                return false;
            }
            if (HasSurface) {
                if (!EGL14.EglDestroySurface(eglDisplay, eglSurface)) {
                    return false;
                }
            }
            eglSurface = null;
            return true;
        }
    }
}
