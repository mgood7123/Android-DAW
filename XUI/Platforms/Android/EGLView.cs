using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Java.Nio;
using Microsoft.Maui.Platform;
using XUI.Platforms.Android;

namespace XUI.Platform.AndroidInternal {
    using static LogError;

    //
    // WINDOWS:
    //
    // [ DirectX 11 ] renders 3D
    // [ Direct2D ] renders 2D
    //
    // [ Direct2D ] works on top of [ DirectX 11 ]
    //
    //
    // ANDROID:
    //
    // [ TextureView + EGL Context ] renders 3D (as a normal View)
    // [ GL texture + SurfaceTexture + Surface + Canvas ] renders 2D
    // [ GL texture + SurfaceTexture + Surface + Canvas ] works on top of [ TextureView + EGL Context ]
    //
    public class EGLView : FrameLayout {
        TextureView textureView;
        EGLContextManager eglContextManager = new();
        Surface surface;

        AndroidCanvas canvas;
        AndroidCanvas hardware_canvas;
        RenderThread renderThread = new();
        RenderThread renderThread2 = new();

        long frameCount = 0;
        long frameCountPer60 = 0;
        long frameTimeNanos = 0;
        bool CanvasIsHardwareAccelerated;

        public EGLView(Context context) : base(context) {
            Microsoft.Maui.ApplicationModel.Platform.ActivityStateChanged += Platform_ActivityStateChanged;
            textureView = new TextureView(context);
            textureView.SurfaceTextureDestroyed += TextureView_SurfaceTextureDestroyed;
            textureView.SurfaceTextureUpdated += TextureView_SurfaceTextureUpdated;
            textureView.SurfaceTextureSizeChanged += TextureView_SurfaceTextureSizeChanged;
            textureView.SurfaceTextureAvailable += TextureView_SurfaceTextureAvailable;
            ViewAttachedToWindow += EGLView_ViewAttachedToWindow;
            ViewDetachedFromWindow += EGLView_ViewDetachedFromWindow;
            SetWillNotDraw(false);
            renderThread.OnDoFrame = EGLView_Render;
            AddView(textureView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            renderThread2.SetRenderMode(false);
        }

        private void Platform_ActivityStateChanged(object sender, Microsoft.Maui.ApplicationModel.ActivityStateChangedEventArgs e) {
            if (e.State == Microsoft.Maui.ApplicationModel.ActivityState.Paused) {
                renderThread.SetRenderMode(false);
            } else if (e.State == Microsoft.Maui.ApplicationModel.ActivityState.Resumed) {
                renderThread.SetRenderMode(true);
            }
        }

        void RenderGL() {
            Error("creating gl resources");
            int FLOAT_SIZE_BYTES = 4;
            int TRIANGLE_VERTICES_DATA_STRIDE_BYTES = 5 * FLOAT_SIZE_BYTES;
            int TRIANGLE_VERTICES_DATA_POS_OFFSET = 0;
            int TRIANGLE_VERTICES_DATA_UV_OFFSET = 3;
            float[] mTriangleVerticesData = {
                // X,     Y,     Z,     U,     V
                -1.0f, -1.0f,  0.0f,  0.0f,  0.0f,
                 1.0f, -1.0f,  0.0f,  1.0f,  0.0f,
                -1.0f,  1.0f,  0.0f,  0.0f,  1.0f,
                 1.0f,  1.0f,  0.0f,  1.0f,  1.0f
            };

            FloatBuffer mTriangleVertices;

            string mVertexShader =
                    "uniform mat4 uMVPMatrix;\n" +
                    "uniform mat4 uSTMatrix;\n" +
                    "attribute vec4 aPosition;\n" +
                    "attribute vec4 aTextureCoord;\n" +
                    "varying vec2 vTextureCoord;\n" +
                    "void main() {\n" +
                    "  gl_Position = uMVPMatrix * aPosition;\n" +
                    "  vTextureCoord = (uSTMatrix * aTextureCoord).xy;\n" +
                    "}\n";

            string mFragmentShader =
                    "#extension GL_OES_EGL_image_external : require\n" +
                    "precision mediump float;\n" +
                    "varying vec2 vTextureCoord;\n" +
                    "uniform samplerExternalOES sTexture;\n" +
                    "void main() {\n" +
                    "  gl_FragColor = texture2D(sTexture, vTextureCoord);\n" +
                    "}\n";

            float[] mMVPMatrix = new float[16];
            float[] mSTMatrix = new float[16];
            Android.Opengl.Matrix.SetIdentityM(mMVPMatrix, 0);
            Android.Opengl.Matrix.SetIdentityM(mSTMatrix, 0);

            int mProgram;
            int mTextureID;
            int muMVPMatrixHandle;
            int muSTMatrixHandle;
            int maPositionHandle;
            int maTextureHandle;

            mTriangleVertices = ByteBuffer.AllocateDirect(
                    mTriangleVerticesData.Length * FLOAT_SIZE_BYTES)
                    .Order(ByteOrder.NativeOrder()).AsFloatBuffer();
            mTriangleVertices.Put(mTriangleVerticesData).Position(0);

            int mWidth = 0, mHeight = 0;

            mProgram = createProgram(mVertexShader, mFragmentShader);
            if (mProgram == 0) {
                return;
            }

            maPositionHandle = Android.Opengl.GLES20.GlGetAttribLocation(mProgram, "aPosition");
            checkGlError("glGetAttribLocation aPosition");
            if (maPositionHandle == -1) {
                throw new System.Exception("Could not get attrib location for aPosition");
            }
            maTextureHandle = Android.Opengl.GLES20.GlGetAttribLocation(mProgram, "aTextureCoord");
            checkGlError("glGetAttribLocation aTextureCoord");
            if (maTextureHandle == -1) {
                throw new System.Exception("Could not get attrib location for aTextureCoord");
            }

            muMVPMatrixHandle = Android.Opengl.GLES20.GlGetUniformLocation(mProgram, "uMVPMatrix");
            checkGlError("glGetUniformLocation uMVPMatrix");
            if (muMVPMatrixHandle == -1) {
                throw new System.Exception("Could not get attrib location for uMVPMatrix");
            }

            muSTMatrixHandle = Android.Opengl.GLES20.GlGetUniformLocation(mProgram, "uSTMatrix");
            checkGlError("glGetUniformLocation uSTMatrix");
            if (muSTMatrixHandle == -1) {
                throw new System.Exception("Could not get attrib location for uSTMatrix");
            }

            int[] textures = new int[1];
            Android.Opengl.GLES20.GlGenTextures(1, textures, 0);

            mTextureID = textures[0];
            Android.Opengl.GLES20.GlBindTexture(Android.Opengl.GLES11Ext.GlTextureExternalOes, mTextureID);
            checkGlError("glBindTexture mTextureID");

            Android.Opengl.GLES20.GlTexParameterf(Android.Opengl.GLES11Ext.GlTextureExternalOes, Android.Opengl.GLES20.GlTextureMinFilter,
                    Android.Opengl.GLES20.GlNearest);
            Android.Opengl.GLES20.GlTexParameterf(Android.Opengl.GLES11Ext.GlTextureExternalOes, Android.Opengl.GLES20.GlTextureMagFilter,
            Android.Opengl.GLES20.GlLinear);

            // SurfaceTexture is a combination of a surface and an OpenGL ES (GLES) texture.
            // SurfaceTexture instances are used to provide surfaces that output to GLES textures.
            //
            // SurfaceTexture contains an instance of BufferQueue for which apps are the consumer.
            //
            // The onFrameAvailable() callback notifies apps when the producer queues a new buffer.
            //
            // Then, apps call updateTexImage(),
            //  which releases the previously held buffer,
            //  acquires the new buffer from the queue,
            //  and makes EGL calls to make the buffer available to GLES as an external texture.
            //

            Error("creating surface texture");
            SurfaceTexture surfaceTexture = new(false);
            surfaceTexture.SetDefaultBufferSize((int)canvas.Width, (int)canvas.Height);
            Error("attaching to texture");
            surfaceTexture.AttachToGLContext(mTextureID);
            surfaceTexture.GetTransformMatrix(mSTMatrix);
            Error("create surface");
            Surface surface = new(surfaceTexture);
            Error("lock hardware canvas");
            Canvas glCanvas = surface.LockHardwareCanvas();
            if (glCanvas != null) {
                Error("draw A 255 R 255 G 255 B 0");
                glCanvas.DrawARGB(255, 255, 255, 0);
                //CanvasIsHardwareAccelerated = glCanvas.IsHardwareAccelerated;
                //hardware_canvas.hostCanvas = glCanvas;
                //hardware_canvas.canvas = glCanvas;
                //hardware_canvas.OnResize(glCanvas.Width, glCanvas.Height);
                //hardware_canvas.SetColor(XUI.Graphics.Colors.Silver);
                //if (render != null) {
                //    int s = hardware_canvas.Save();
                //    render(hardware_canvas);
                //    hardware_canvas.Restore(s);
                //}
                //hardware_canvas.canvas = null;
                //hardware_canvas.hostCanvas = null;
                Error("unlock canvas and post");
                surface.UnlockCanvasAndPost(glCanvas);
            }
            Error("update tex image");
            surfaceTexture.UpdateTexImage();
            Error("drawing texture");
            //Android.Opengl.GLES20.GlActiveTexture(Android.Opengl.GLES20.GlTexture0);
            //Android.Opengl.GLES20.GlBindTexture(Android.Opengl.GLES11Ext.GlTextureExternalOes, 0);
            Android.Opengl.GLES20.GlClearColor(0.0f, 1.0f, 0.0f, 1.0f);
            Android.Opengl.GLES20.GlClear(Android.Opengl.GLES20.GlDepthBufferBit | Android.Opengl.GLES20.GlColorBufferBit);
            if (true) {
                Android.Opengl.GLES20.GlUseProgram(mProgram);
                checkGlError("glUseProgram");
                Android.Opengl.GLES20.GlActiveTexture(Android.Opengl.GLES20.GlTexture0);
                Android.Opengl.GLES20.GlBindTexture(Android.Opengl.GLES11Ext.GlTextureExternalOes, mTextureID);
                mTriangleVertices.Position(TRIANGLE_VERTICES_DATA_POS_OFFSET);
                Android.Opengl.GLES20.GlVertexAttribPointer(maPositionHandle, 3, Android.Opengl.GLES20.GlFloat, false,
                TRIANGLE_VERTICES_DATA_STRIDE_BYTES, mTriangleVertices);
                checkGlError("glVertexAttribPointer maPosition");
                Android.Opengl.GLES20.GlEnableVertexAttribArray(maPositionHandle);
                checkGlError("glEnableVertexAttribArray maPositionHandle");
                mTriangleVertices.Position(TRIANGLE_VERTICES_DATA_UV_OFFSET);
                Android.Opengl.GLES20.GlVertexAttribPointer(maTextureHandle, 3, Android.Opengl.GLES20.GlFloat, false,
                TRIANGLE_VERTICES_DATA_STRIDE_BYTES, mTriangleVertices);
                checkGlError("glVertexAttribPointer maTextureHandle");
                Android.Opengl.GLES20.GlEnableVertexAttribArray(maTextureHandle);
                checkGlError("glEnableVertexAttribArray maTextureHandle");
                Android.Opengl.Matrix.SetIdentityM(mMVPMatrix, 0);
                Android.Opengl.GLES20.GlUniformMatrix4fv(muMVPMatrixHandle, 1, false, mMVPMatrix, 0);
                Android.Opengl.GLES20.GlUniformMatrix4fv(muSTMatrixHandle, 1, false, mSTMatrix, 0);
                Android.Opengl.GLES20.GlDrawArrays(Android.Opengl.GLES20.GlTriangleStrip, 0, 4);
                checkGlError("glDrawArrays");
            }
            Android.Opengl.GLES20.GlFlush();

            Error("disposing surface");
            surface.Release();
            surface.Dispose();
            surface = null;
            Error("disposing surface texture");
            surfaceTexture.DetachFromGLContext();
            surfaceTexture.Release();
            surfaceTexture.Dispose();
            surfaceTexture = null;
            Error("disposing gl resources");
            Android.Opengl.GLES20.GlDeleteProgram(mProgram);
            Android.Opengl.GLES10.GlDeleteTextures(1, textures, 0);
            Error("disposed");
        }

        private int loadShader(int shaderType, string source) {
            int shader = Android.Opengl.GLES20.GlCreateShader(shaderType);
            if (shader != 0) {
                Android.Opengl.GLES20.GlShaderSource(shader, source);
                Android.Opengl.GLES20.GlCompileShader(shader);
                int[] compiled = new int[1];
                Android.Opengl.GLES20.GlGetShaderiv(shader, Android.Opengl.GLES20.GlCompileStatus, compiled, 0);
                if (compiled[0] == 0) {
                    Error("Could not compile shader " + shaderType + ":");
                    Error(Android.Opengl.GLES20.GlGetShaderInfoLog(shader));
                    Android.Opengl.GLES20.GlDeleteShader(shader);
                    shader = 0;
                }
            }
            return shader;
        }

        private int createProgram(string vertexSource, string fragmentSource) {
            int vertexShader = loadShader(Android.Opengl.GLES20.GlVertexShader, vertexSource);
            if (vertexShader == 0) {
                return 0;
            }
            int pixelShader = loadShader(Android.Opengl.GLES20.GlFragmentShader, fragmentSource);
            if (pixelShader == 0) {
                return 0;
            }

            int program = Android.Opengl.GLES20.GlCreateProgram();
            if (program != 0) {
                Android.Opengl.GLES20.GlAttachShader(program, vertexShader);
                checkGlError("glAttachShader");
                Android.Opengl.GLES20.GlAttachShader(program, pixelShader);
                checkGlError("glAttachShader");
                Android.Opengl.GLES20.GlLinkProgram(program);
                int[] linkStatus = new int[1];
                Android.Opengl.GLES20.GlGetProgramiv(program, Android.Opengl.GLES20.GlLinkStatus, linkStatus, 0);
                if (linkStatus[0] != Android.Opengl.GLES20.GlTrue) {
                    Error("Could not link program: ");
                    Error(Android.Opengl.GLES20.GlGetProgramInfoLog(program));
                    Android.Opengl.GLES20.GlDeleteProgram(program);
                    program = 0;
                }
            }
            return program;
        }

        private void checkGlError(string op) {
            int error;
            while ((error = Android.Opengl.GLES20.GlGetError()) != Android.Opengl.GLES20.GlNoError) {
                Error(op + ": glError " + error);
                throw new System.Exception(op + ": glError " + error);
            }
        }

        private void EGLView_Render(long frameTimeNanos) {
            frameCount++;
            if (frameCount == 60) {
                frameCountPer60++;
                frameCount = 0;
            }
            this.frameTimeNanos = frameTimeNanos;

            if (!eglContextManager.HasSurface) {
                return;
            }
            if (!eglContextManager.MakeCurrent()) {
                return;
            }

            //Android.Opengl.GLES20.GlClearColor(0.0f, 1.0f, 0.0f, 1.0f);
            //Android.Opengl.GLES20.GlClear(Android.Opengl.GLES20.GlDepthBufferBit | Android.Opengl.GLES20.GlColorBufferBit);
            RenderGL();
            eglContextManager.SwapBuffers();

            // draws to TextureView surface
            //if (eglContextManager.Major == 1) {
            //    Android.Opengl.GLES10.GlClearColor(1.0f, 0.0f, 0.0f, 1.0f);
            //    Android.Opengl.GLES10.GlClear(Android.Opengl.GLES10.GlColorBufferBit);
            //} else if (eglContextManager.Major == 2) {
            //    Android.Opengl.GLES20.GlClearColor(1.0f, 0.0f, 0.0f, 1.0f);
            //    Android.Opengl.GLES20.GlClear(Android.Opengl.GLES20.GlColorBufferBit);
            //} else if (eglContextManager.Major == 3) {
            //    Android.Opengl.GLES30.GlClearColor(1.0f, 0.0f, 0.0f, 1.0f);
            //    Android.Opengl.GLES30.GlClear(Android.Opengl.GLES30.GlColorBufferBit);
            //}

            //
            // WINDOWS:
            //
            // [ DirectX 11 ] renders 3D
            // [ Direct2D ] renders 2D
            //
            // [ Direct2D ] works on top of [ DirectX 11 ]
            //
            //
            // ANDROID:
            //
            // [ TextureView + EGL Context ] renders 3D (as a normal View)
            // [ GL texture + SurfaceTexture + Surface + Canvas ] renders 2D
            // [ GL texture + SurfaceTexture + Surface + Canvas ] works on top of [ TextureView + EGL Context ]
            //
            //
            //
            //  create a GL texture
            //  assign it to a SurfaceTexture
            //  assign it to a Surface
            //  obtain a hardware canvas
            //  draw via canvas
            //  unlock and post hardware canvas
            //  draw GL texture
            // 
            // int gl_texture = create_texture(200, 200);
            //
            // SurfaceTexture surfaceTexture = new();
            //
            // surfaceTexture.AttachToGLContext(gl_texture);
            //
            // Surface textureSurface = new(surfaceTexture);
            //
            // Canvas glCanvas = surface.LockHardwareCanvas();
            //
            // if (glCanvas != null) {
            //     draw(glCanvas);
            //     surface.UnlockCanvasAndPost(glCanvas);
            // }
            //
            // surface.Release();
            // surfaceTexture.ReleaseTexImage();
            // surfaceTexture.DetachFromGLContext();
            //
            // draw_texture(gl_texture, 0, 0, 200, 200);
            //
            //
            //
            //
            //
            //Canvas glCanvas = surface.LockHardwareCanvas();
            //canvas.hostCanvas = glCanvas;
            //canvas.canvas = glCanvas;
            //canvas.OnResize(glCanvas.Width, glCanvas.Height);
            //canvas.Clear(XUI.Graphics.Colors.Red);
            //if (render != null) {
            //    int s = canvas.Save();
            //    render(canvas);
            //    canvas.Restore(s);
            //}
            //surface.UnlockCanvasAndPost(glCanvas);
            //eglContextManager.SwapBuffers();
        }

        private void EGLView_ViewAttachedToWindow(object sender, ViewAttachedToWindowEventArgs e) {
            canvas = new AndroidCanvas();
            canvas.context = Context;
            canvas.OnCreateResources();
            canvas.density = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.GetDisplayDensity();
            hardware_canvas = new AndroidCanvas();
            hardware_canvas.context = Context;
            hardware_canvas.OnCreateResources();
            hardware_canvas.density = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.GetDisplayDensity();
            renderThread2.Start();
            renderThread.Start();
            renderThread.Post(() => {
                if (!eglContextManager.TryCreate(8, 8, 8, 0, 16, 0, EGLContextManager.EGL_VERSION.EGL_VERSION_3_0) || !eglContextManager.TryCreate(8, 8, 8, 0, 16, 0, EGLContextManager.EGL_VERSION.EGL_VERSION_2_0)) {
                    // do nothing
                    return;
                }
                if (!eglContextManager.HasContext) {
                    // do nothing
                    return;
                }
            });
            renderThread.SetRenderMode(true);
        }
        private void EGLView_ViewDetachedFromWindow(object sender, ViewDetachedFromWindowEventArgs e) {
            renderThread.SetRenderMode(false);
            renderThread.PostAndWait(() => {
                if (!eglContextManager.TryDetachFromSurfaceTexture()) {
                    // ignore
                }
                if (!eglContextManager.TryDestroy()) {
                    // do nothing
                    return;
                }
            });
            renderThread.Stop();
            renderThread2.Stop();
            surface.Release();
            surface = null;
            hardware_canvas.density = 0;
            hardware_canvas.OnDisposeResources();
            hardware_canvas.context = null;
            hardware_canvas = null;
            canvas.density = 0;
            canvas.OnDisposeResources();
            canvas.context = null;
            canvas = null;
        }

        private void TextureView_SurfaceTextureAvailable(object sender, TextureView.SurfaceTextureAvailableEventArgs e) {
            surface = new Surface(e.Surface);
            renderThread.Post(() => {
                if (!eglContextManager.HasContext) {
                    // do nothing
                    return;
                }
                if (!eglContextManager.TryAttachToSurfaceTexture(e.Surface)) {
                    // do nothing
                    return;
                }
            });
            renderThread.SetRenderMode(true);
        }

        private void TextureView_SurfaceTextureSizeChanged(object sender, TextureView.SurfaceTextureSizeChangedEventArgs e) {

        }

        private void TextureView_SurfaceTextureUpdated(object sender, TextureView.SurfaceTextureUpdatedEventArgs e) {
        }

        private void TextureView_SurfaceTextureDestroyed(object sender, TextureView.SurfaceTextureDestroyedEventArgs e) {
            renderThread.SetRenderMode(false);
            renderThread.PostAndWait(() => {
                if (!eglContextManager.TryDetachFromSurfaceTexture()) {
                    // do nothing
                    return;
                }
            });
            surface.Release();
            surface = null;
        }

        long EGLHolderFrameCount = 0;
        long EGLHolderFrameCountPer60 = 0;

        public override void OnDrawForeground(Canvas android_canvas) {
            EGLHolderFrameCount++;
            if (EGLHolderFrameCount == 60) {
                EGLHolderFrameCountPer60++;
                EGLHolderFrameCount = 0;
            }
            canvas.hostCanvas = android_canvas;
            canvas.canvas = android_canvas;
            canvas.OnResize(android_canvas.Width, android_canvas.Height);
            canvas.SetColor(XUI.Graphics.Colors.Silver);
            System.TimeSpan renderTime = renderThread.RenderTime;
            canvas.DrawText(
                "EGL Major: " + eglContextManager.Major
                + "\nEGL Minor: " + eglContextManager.Minor
                + "\nEGL Has Display: " + eglContextManager.HasDisplay
                + "\nEGL Has Context: " + eglContextManager.HasContext
                + "\nEGL Has Surface: " + eglContextManager.HasSurface
                + "\nEGL Context Is Current: " + eglContextManager.IsCurrent
                + "\nEGL Frame: " + frameCount
                + "\nEGL Frame per 60: " + frameCountPer60
                + "\nEGL Frame time nanoseconds: " + frameTimeNanos
                + "\nEGL Render time: " + renderTime.Milliseconds + "." + renderTime.Microseconds + " (milli.micro)"
                + "\nEGL Hardware Accelerated Window: " + textureView.IsHardwareAccelerated
                + "\nEGL Hardware Accelerated Canvas: " + CanvasIsHardwareAccelerated
                + "\nEGLHolder Frame: " + EGLHolderFrameCount
                + "\nEGLHolder Frame per 60: " + EGLHolderFrameCountPer60
                , 0, 0, (int)canvas.Width, (int)canvas.Height
            );
            Invalidate();
        }

        internal System.Action<AndroidCanvas> render;
    }
}