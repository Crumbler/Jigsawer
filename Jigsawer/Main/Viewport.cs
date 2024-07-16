
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

namespace Jigsawer.Main;

public static class Viewport {
    [SkipLocalsInit]
    public static unsafe Box2i Get() {
        Box2i viewportBox;

        GL.GetInteger(GetPName.Viewport, (int*)&viewportBox);

        return viewportBox;
    }

    public static void Set(Box2i box) => GL.Viewport(box);
}
