using System;
using System.Collections.Generic;
using System.Numerics;
using XUI.Graphics;

namespace XUI.Platforms.Windows {
    internal class ByteReader {
        internal byte[] byteArray;
        internal object[] objectArray;
        int byteIdx = 0;
        int objectIdx = 0;

        List<Vector<int>> idxState = [];

        internal bool empty => byteArray.Length == 0 || byteIdx == byteArray.Length;

        public ByteReader(ByteWriter writer) {
            byteArray = writer.byteArray.ToArray();
            objectArray = writer.objectArray.ToArray();
        }

        internal int Save() {
            int save = idxState.Count;
            idxState.Add(new([byteIdx, objectIdx]));
            return save;
        }

        internal void Restore(int save) {
            for (int s = idxState.Count - 1; s >= save; s--) {
                if (s == save) {
                    Vector<int> vector = idxState[s];
                    byteIdx = vector[0];
                    objectIdx = vector[1];
                }
                idxState.RemoveAt(s);
            }
        }

        internal object readObject() => objectArray[objectIdx++];
        internal byte readByte() => byteArray[byteIdx++];
        internal short readShort() {
            short value = BitConverter.ToInt16(byteArray, byteIdx);
            byteIdx += 2;
            return value;
        }
        internal int readInt() {
            int value = BitConverter.ToInt32(byteArray, byteIdx);
            byteIdx += 4;
            return value;
        }
        internal long readLong() {
            long value = BitConverter.ToInt64(byteArray, byteIdx);
            byteIdx += 8;
            return value;
        }
        internal float readFloat() {
            float value = BitConverter.ToSingle(byteArray, byteIdx);
            byteIdx += 4;
            return value;
        }
        internal double readDouble() {
            double value = BitConverter.ToDouble(byteArray, byteIdx);
            byteIdx += 8;
            return value;
        }

        internal Color4 readColor4() => new(readInt());

        internal void skipObject() => objectIdx++;
        internal void skipByte() => byteIdx++;
        internal void skipShort() => byteIdx += 2;
        internal void skipInt() => byteIdx += 4;
        internal void skipLong() => byteIdx += 8;
        internal void skipFloat() => byteIdx += 4;
        internal void skipDouble() => byteIdx += 8;
        internal void skipColor4() => skipInt();
    }
}
