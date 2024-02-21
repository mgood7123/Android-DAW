namespace XUI.Utils {

    public static class DebugHelper {
#if DEBUG
        public static bool IsDebugBuild = true;
#else
        public static bool IsDebugBuild = false;
#endif
    }
}
