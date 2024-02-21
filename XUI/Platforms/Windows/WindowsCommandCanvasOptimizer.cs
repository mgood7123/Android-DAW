//using System.Collections.Generic;

//namespace XUI.Platforms.Windows {
//    public class WindowsCommandCanvasOptimizer {
//        internal ByteReader reader;
//        internal ByteWriter writer;
//        public void From(WindowsCommandCanvas windowsCommandCanvas) {
//            reader = new(windowsCommandCanvas.writer);
//            writer = new();
//            optimize();
//        }

//        private void optimize() {
//            copy();
//        }

//        private void copy() {
//            // copy
//            writer.byteArray = new(reader.byteArray);
//            writer.objectArray = new(reader.objectArray);
//        }
//    }
//}