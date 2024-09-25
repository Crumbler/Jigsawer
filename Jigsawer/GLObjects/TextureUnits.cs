
using OpenTK.Graphics.OpenGL4;
using Jigsawer.Helpers;
using Jigsawer.Debug;

namespace Jigsawer.GLObjects;

public static class TextureUnits {
    private static readonly int unitCount = GetMaxTextureUnitCount();
    private static readonly byte[] unitUseCounts = new byte[unitCount];
    private static readonly int[] unitToTexture = new int[unitCount];

    private static int GetMaxTextureUnitCount() {
        GL.GetInteger(GetPName.MaxTextureImageUnits, out int maxUnitCount);
        Logger.LogDebug("Max texture unit count: " + maxUnitCount);
        return maxUnitCount;
    }

    public static int GrabOne() {
        var (index, _) = unitUseCounts.GetMinElement();

        ++unitUseCounts[index];

        return index;
    }

    public static void ReturnOne(int unit) {
        --unitUseCounts[unit];
    }

    public static void Bind(int unit, int texture) {
        if (unitToTexture[unit] != texture) {
            unitToTexture[unit] = texture;
            GL.BindTextureUnit(unit, texture);
        }
    }

    public static void UnbindAll() {
        Array.Clear(unitUseCounts);
        Array.Clear(unitToTexture);
    }
}
