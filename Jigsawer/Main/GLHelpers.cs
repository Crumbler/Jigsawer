using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Runtime.CompilerServices;

namespace Jigsawer.Main;
public static class GLHelpers {
    [SkipLocalsInit]
    public static unsafe Box2i GetViewport() {
        Box2i viewportBox;

        GL.GetInteger(GetPName.Viewport, (int*)&viewportBox);

        return viewportBox;
    }
}
