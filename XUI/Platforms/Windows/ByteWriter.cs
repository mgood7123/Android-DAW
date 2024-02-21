using System;
using System.Collections.Generic;
using XUI.Graphics;

namespace XUI.Platforms.Windows {
    internal class ByteWriter {
        internal List<byte> byteArray = [];
        internal List<object> objectArray = [];

        // 8 bits   (1 byte)
        // 16 bits  (2 bytes)
        // 32 bits  (4 bytes)
        // 64 bits  (8 bytes)
        // 128 bits (16 bytes)

        internal void writeObject(object obj) => objectArray.Add(obj);
        internal void writeByte(byte value) => byteArray.Add(value);

        internal void writeShort(short value) {
            byte[] bytes = BitConverter.GetBytes(value);
            for (int b = 0; b < bytes.Length; b++) {
                byteArray.Add(bytes[b]);
            }
        }

        internal void writeInt(int value) {
            byte[] bytes = BitConverter.GetBytes(value);
            for (int b = 0; b < bytes.Length; b++) {
                byteArray.Add(bytes[b]);
            }
        }

        internal void writeFloat(float value) {
            byte[] bytes = BitConverter.GetBytes(value);
            for (int b = 0; b < bytes.Length; b++) {
                byteArray.Add(bytes[b]);
            }
        }

        internal void writeLong(long value) {
            byte[] bytes = BitConverter.GetBytes(value);
            for (int b = 0; b < bytes.Length; b++) {
                byteArray.Add(bytes[b]);
            }
        }

        internal void writeDouble(double value) {
            byte[] bytes = BitConverter.GetBytes(value);
            for (int b = 0; b < bytes.Length; b++) {
                byteArray.Add(bytes[b]);
            }
        }

        internal void writeColor4(Color4 color) => writeInt((int)color.ToRgba());
    }
}
