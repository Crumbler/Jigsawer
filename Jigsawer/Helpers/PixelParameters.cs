
using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Helpers;

public static class PixelParameters {
    public static int UnpackAlignment {
        get => GL.GetInteger(GetPName.UnpackAlignment);
        set => GL.PixelStore(PixelStoreParameter.UnpackAlignment, value);
    }
}
