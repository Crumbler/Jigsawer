
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public sealed class ImageShaderProgram : ShaderProgram {
    private const string EntityName = "Image";

    private ImageShaderProgram() { }

    protected override void BindAttributes() {
        BindAttribute(AttributePositions.Position, AttributeNames.Position);
    }

    public static ImageShaderProgram Create() {
        var program = new ImageShaderProgram();

        program.Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        return program;
    }

    public void SetProjectionMatrix(ref Matrix3 mat) {
        SetMatrix(UniformLocations.ProjectionMatrix, ref mat);
    }
    public void SetTextureSize(Vector2 size) {
        SetVector2(UniformLocations.TextureSize, size);
    }
    public void SetTextureUnit(int unit) {
        SetInt(UniformLocations.Texture, unit);
    }

    public static class AttributePositions {
        public const int Position = 0;
    }

    private static class AttributeNames {
        public const string Position = "box";
    }

    private static class UniformLocations {
        public const int ProjectionMatrix = 0;
        public const int TextureSize = 1;
        public const int Texture = 2;
    }
}
