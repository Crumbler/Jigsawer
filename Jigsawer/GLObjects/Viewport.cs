using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Runtime.CompilerServices;

namespace Jigsawer.GLObjects;

public static class Viewport {
    public static Box2i Bounds {
        [SkipLocalsInit]
        get {
            Box2i viewportBox;

            unsafe {
                GL.GetInteger(GetPName.Viewport, (int*)&viewportBox);
            }

            return viewportBox;
        }

        set => GL.Viewport(value);
    }

    public static Vector2i Size {
        get {
            var bounds = Bounds;
            return bounds.Max - bounds.Min;
        }

        set => Bounds = new Box2i(Vector2i.Zero, value);
    }
}
