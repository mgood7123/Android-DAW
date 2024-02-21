using System;
using System.Collections.Generic;
using Vortice.Direct2D1;
using Vortice.DirectWrite;

namespace XUI.Platforms.Windows {
    public class WindowsCanvas : Graphics.Canvas {
        internal ID2D1RenderTarget renderTarget;
        internal ID2D1Factory factory;
        internal IDWriteFactory iDWriteFactory;
        internal IDWriteTextFormat iDWriteTextFormat;
        internal ID2D1SolidColorBrush iD2D1SolidColorBrush;
        internal float w;
        internal float h;
        internal float density;
        internal System.Numerics.Matrix3x2 transform = System.Numerics.Matrix3x2.Identity;
        internal System.Numerics.Vector2 translation = System.Numerics.Vector2.Zero;

        public override bool IsHardwareAccelerated => true;

        protected override float getDensity() => density;

        internal class State {
            internal ID2D1DrawingStateBlock Block { get; set; }
            internal int Clip { get; set; }
        }

        internal List<State> stack = [];

        public override void OnCreateResources() {
            iDWriteFactory = DWrite.DWriteCreateFactory<IDWriteFactory>(Vortice.DirectWrite.FactoryType.Shared);
            iDWriteTextFormat = iDWriteFactory.CreateTextFormat("Arial", 20);
            iD2D1SolidColorBrush = renderTarget.CreateSolidColorBrush(Graphics.Colors.Black.ToPlatformColor());
        }

        public override void OnDisposeResources() {
            for (int s = stack.Count - 1; s >= 0; s--) {
                State state = stack[s];
                while (clipCount != state.Clip) {
                    renderTarget.PopAxisAlignedClip();
                    clipCount--;
                }
                state.Block.Dispose();
                stack.RemoveAt(s);
            }

            if (clipCount != 0) {
                throw new InvalidOperationException("OnDisposeResources : clip count failed to reduce to zero");
            }

            // clear internal list memory, lists often allocate memory but do not release when .Clear()
            // often for perf reasons however this means if we allocate 10 MB via the list (internal list memory)
            // then Clear() it, the 10 MB will not be erased unless it is garbage collected
            //
            stack = [];

            iD2D1SolidColorBrush?.Dispose();
            iDWriteTextFormat?.Dispose();
            iDWriteFactory?.Dispose();
        }

        public override void OnResize(float w, float h) {
            this.w = w;
            this.h = h;
        }

        public override void Translate(float dx, float dy) {
            translation.X += dx;
            translation.Y += dy;
            transform.Translation = translation;
            renderTarget.Transform = transform;
        }

        public override void Clear() {
            renderTarget.Clear(iD2D1SolidColorBrush.Color);
        }

        public override void Clear(Graphics.Color4 color) {
            renderTarget.Clear(color.ToPlatformColor());
        }

        public override void DrawText(string text, int x, int y, int w, int h) {
            renderTarget.DrawText(text, iDWriteTextFormat, new(x, y, w, h), iD2D1SolidColorBrush);
        }

        public override void SetColor(Graphics.Color4 color) {
            iD2D1SolidColorBrush.Color = color.ToPlatformColor();
        }

        public override void DrawRect(float x, float y, float w, float h) {
            renderTarget.FillRectangle(new(x, y, w, h), iD2D1SolidColorBrush);
        }

        int clipCount = 0;

        public override void ClipRelativeRect(float x, float y, float w, float h) {
            renderTarget.PushAxisAlignedClip(new(x, y, w, h), AntialiasMode.PerPrimitive);
            clipCount++;
        }

        public override int Save() {
            int save = stack.Count;
            ID2D1DrawingStateBlock iD2D1DrawingStateBlock = factory.CreateDrawingStateBlock();
            stack.Add(new() { Block = iD2D1DrawingStateBlock, Clip = clipCount });
            renderTarget.SaveDrawingState(iD2D1DrawingStateBlock);
            return save;
        }

        public override void Restore(int save) {
            for (int s = stack.Count-1; s >= save; s--) {
                State state = stack[s];
                while (clipCount != state.Clip) {
                    renderTarget.PopAxisAlignedClip();
                    clipCount--;
                }
                if (s == save) {
                    renderTarget.RestoreDrawingState(state.Block);
                    transform = renderTarget.Transform;
                    translation = transform.Translation;
                }
                state.Block.Dispose();
                stack.RemoveAt(s);
            }
        }

        protected override float getWidth() => w;
        protected override float getHeight() => h;
    }
}
