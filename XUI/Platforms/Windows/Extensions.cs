using XUI.Graphics;

namespace XUI.Platforms.Windows {
    public static class Extensions {
        public static Vortice.Mathematics.Color4 ToPlatformColor(this Color color) => new Vortice.Mathematics.Color4(color.R, color.G, color.B, color.A);
        public static Vortice.Mathematics.Color4 ToPlatformColor(this Color3 color) => new Color(color.R, color.G, color.B).ToPlatformColor();
        public static Vortice.Mathematics.Color4 ToPlatformColor(this Color4 color) => new Color(color.R, color.G, color.B, color.A).ToPlatformColor();
    }
}
