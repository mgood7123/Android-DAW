using Android.Util;

namespace XUI.Platform.AndroidInternal {
    internal static class LogError {
        internal static void Error(string op) => Log.Error("GL", op);
    }
}