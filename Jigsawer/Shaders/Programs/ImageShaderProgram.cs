
using Jigsawer.Scenes;

using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Shaders.Programs;

public abstract class ImageShaderProgram : ShaderProgram {
    protected void InitializeImageShader(string entityName) {
        Initialize(
            ShaderInfo.Get(entityName, ShaderType.VertexShader),
            ShaderInfo.Get(entityName, ShaderType.FragmentShader));

        ConnectUniformBlockToBuffer(UniformBlockNames.SharedInfo, Globals.SharedInfoBindingPoint);
    }

    public abstract void SetScaleFactor(float x);
    public abstract void SetTextureUnit(int unit);

    public static class AttributePositions {
        public const int Position = 0;
    }

    private static class UniformBlockNames {
        public const string SharedInfo = "SharedInfo";
    }
}
