
using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Shaders.Programs;

public sealed class ImageShaderProgram : ShaderProgram {
    private const string EntityName = "Image";

    public ImageShaderProgram(int sharedInfoUboBindingPoint) {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        ConnectUniformBlockToBuffer(UniformBlockNames.SharedInfo, sharedInfoUboBindingPoint);
    }

    public void SetScaleFactor(float x) {
        SetFloat(UniformLocations.ScaleFactor, x);
    }
    public void SetTextureUnit(int unit) {
        SetInt(UniformLocations.Texture, unit);
    }

    public static class AttributePositions {
        public const int Position = 0;
    }

    private static class UniformLocations {
        public const int ScaleFactor = 0;
        public const int Texture = 1;
    }

    private static class UniformBlockNames {
        public const string SharedInfo = "SharedInfo";
    }
}
