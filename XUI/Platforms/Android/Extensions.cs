using XUI._Maths;
using XUI.Graphics;
using AColor = Android.Graphics.Color;

namespace XUI.Platforms.Android {
    public static class Extensions {
        public static AColor ToPlatformColor(this Color color) => new(color.R, color.G, color.B, (byte)255);
        public static AColor ToPlatformColor(this Color3 color) => new(PackHelpers.ToByte(color.R), PackHelpers.ToByte(color.G), PackHelpers.ToByte(color.B), (byte)255);
        public static AColor ToPlatformColor(this Color4 color) => new(PackHelpers.ToByte(color.R), PackHelpers.ToByte(color.G), PackHelpers.ToByte(color.B), PackHelpers.ToByte(color.A));
    }
}
