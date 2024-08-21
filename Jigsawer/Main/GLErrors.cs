

using OpenTK.Graphics.OpenGL4;

using System.Diagnostics;

namespace Jigsawer.Main;

public static class GLErrors {
    [Conditional("DEBUG")]
    public static void Check() {
        for (ErrorCode code = GL.GetError(); code != ErrorCode.NoError; code = GL.GetError()) {
            Logger.LogError("Error with code: " + code);
        }
    }
}
