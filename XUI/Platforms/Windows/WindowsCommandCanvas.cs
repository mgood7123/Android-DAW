//using System.Collections.Generic;
//using XUI.Graphics;

//namespace XUI.Platforms.Windows {
//    public class WindowsCommandCanvas : Canvas {
//        internal float w;
//        internal float h;

//        internal ByteWriter writer = new();

//        protected override float getDensity() => 1f;


//        internal enum CMD {
//            CMD_RESIZE = 0,
//            CMD_CLEAR = 1,
//            CMD_CLEAR_COLOR = 2,
//            CMD_CLIP_RELATIVE_RECTANGLE = 3,
//            CMD_FILL_RECTANGLE = 4,
//            CMD_DRAW_TEXT = 5,
//            CMD_SAVE = 6,
//            CMD_RESTORE = 7,
//            CMD_SET_COLOR = 8,
//            CMD_TRANSLATE = 9
//        };

//        public override void OnCreateResources() {
//            reset();
//        }

//        public override void OnDisposeResources() {
//            reset();
//        }

//        public override void OnResize(float w, float h) {
//            writer.writeByte((byte)CMD.CMD_RESIZE);
//            writer.writeFloat(w);
//            writer.writeFloat(h);
//            this.w = w;
//            this.h = h;
//        }

//        public override void Clear() {
//            writer.writeByte((byte)CMD.CMD_CLEAR);
//            writer.writeFloat(w);
//            writer.writeFloat(h);
//        }

//        public override void Clear(Color4 color) {
//            writer.writeByte((byte)CMD.CMD_CLEAR_COLOR);
//            writer.writeColor4(color);
//            writer.writeFloat(w);
//            writer.writeFloat(h);
//        }

//        public override void ClipRelativeRect(float x, float y, float w, float h) {
//            writer.writeByte((byte)CMD.CMD_CLIP_RELATIVE_RECTANGLE);
//            writer.writeFloat(x);
//            writer.writeFloat(y);
//            writer.writeFloat(w);
//            writer.writeFloat(h);
//        }

//        public override void DrawRect(float x, float y, float w, float h) {
//            writer.writeByte((byte)CMD.CMD_FILL_RECTANGLE);
//            writer.writeFloat(x);
//            writer.writeFloat(y);
//            writer.writeFloat(w);
//            writer.writeFloat(h);
//        }

//        public override void DrawText(string text, int x, int y, int w, int h) {
//            writer.writeByte((byte)CMD.CMD_DRAW_TEXT);
//            writer.writeObject(text);
//            writer.writeFloat(x);
//            writer.writeFloat(y);
//            writer.writeFloat(w);
//            writer.writeFloat(h);
//        }

//        internal System.Numerics.Matrix3x2 transform;
//        internal System.Numerics.Vector2 translation;
//        internal List<System.Numerics.Matrix3x2> transformStack;

//        public override void Restore(int save) {
//            writer.writeByte((byte)CMD.CMD_RESTORE);
//            writer.writeInt(save);
//            for (int s = transformStack.Count - 1; s >= save; s--) {
//                if (s == save) {
//                    transform = transformStack[s];
//                    translation = transform.Translation;
//                }
//                transformStack.RemoveAt(s);
//            }
//        }

//        public override int Save() {
//            writer.writeByte((byte)CMD.CMD_SAVE);
//            int save = transformStack.Count;
//            transformStack.Add(transform);
//            writer.writeInt(save);
//            return save++;
//        }

//        public override void SetColor(Color4 color) {
//            writer.writeByte((byte)CMD.CMD_SET_COLOR);
//            writer.writeColor4(color);
//        }

//        public override void Translate(float dx, float dy) {
//            writer.writeByte((byte)CMD.CMD_TRANSLATE);
//            writer.writeFloat(dx);
//            writer.writeFloat(dy);
//            translation.X += dy;
//            translation.Y += dy;
//            transform.Translation = translation;
//        }

//        protected override float getHeight() => h;
//        protected override float getWidth() => w;

//        internal void reset() {
//            transform = System.Numerics.Matrix3x2.Identity;
//            translation = System.Numerics.Vector2.Zero;
//            transformStack = [];
//            writer = new();
//            OnResize(w, h);
//        }
//    }
//}
