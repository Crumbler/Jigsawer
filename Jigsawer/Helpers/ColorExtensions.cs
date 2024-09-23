
using System.Drawing;
using System.Numerics;
using System.Buffers.Binary;
using OpenTK.Mathematics;

namespace Jigsawer.Helpers;

public static class ColorExtensions {
    public static int ToInt(this Color color) {
        int colVal = BinaryPrimitives.ReverseEndianness(color.ToArgb());
        colVal = (int)BitOperations.RotateRight((uint)colVal, 8);
        return colVal;
    }

    public static Color WithAlpha(this Color color, byte alpha) {
        return Color.FromArgb(alpha, color);
    }

    public static Color WithAlpha(this Color color, float alpha) {
        return Color.FromArgb((int)MathF.Round(255 * alpha), color);
    }

    public static int ToInt(this Color4 color) {
        int colVal = BinaryPrimitives.ReverseEndianness(color.ToArgb());
        colVal = (int)BitOperations.RotateRight((uint)colVal, 8);
        return colVal;
    }

    public static Color4 WithAlpha(this Color4 color, byte alpha) {
        return new Color4(color.R, color.G, color.B, alpha / 255f);
    }

    public static Color4 WithAlpha(this Color4 color, float alpha) {
        return new Color4(color.R, color.G, color.B, alpha);
    }
}
