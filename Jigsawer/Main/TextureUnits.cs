﻿
using OpenTK.Graphics.OpenGL4;
using Jigsawer.Helpers;

namespace Jigsawer.Main;

public static class TextureUnits {
    private static readonly int unitCount = GetMaxTextureUnitCount();
    private static readonly byte[] unitUses = new byte[unitCount];
    private static readonly int[] unitToTexture = new int[unitCount];

    private static int GetMaxTextureUnitCount() {
        GL.GetInteger(GetPName.MaxTextureImageUnits, out int maxUnitCount);
        Logger.LogDebug("Max texture unit count: " + maxUnitCount);
        return maxUnitCount;
    }

    public static int GrabUnit() {
        var (index, element) = unitUses.GetMinElement();

        ++unitUses[index];

        return element;
    }

    public static void ReturnUnit(int unit) {
        --unitUses[unit];
    }

    public static void Bind(int unit, int texture) {
        if (unitToTexture[unit] != texture) {
            unitToTexture[unit] = texture;
            GL.BindTextureUnit(unit, texture);
        }
    }

    public static void UnbindAll() {
        Span<int> span = unitToTexture;

        for (int i = 0; i < span.Length; ++i) {
            if (span[i] != 0) {
                GL.BindTextureUnit(i, 0);
            }
        }

        span.Clear();
    }
}
