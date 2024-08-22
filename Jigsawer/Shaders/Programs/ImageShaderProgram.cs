
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public sealed class ImageShaderProgram : ShaderProgram {
    private const string EntityName = "Image";

    private ImageShaderProgram() { }

    public static ImageShaderProgram Create() {
        var program = new ImageShaderProgram();

        program.Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        return program;
    }

    public static void SetProjectionMatrix(ref Matrix3 mat) {
        SetMatrix(UniformLocations.ProjectionMatrix, ref mat);
    }
    public static void SetTextureSize(Vector2 size) {
        SetVector2(UniformLocations.TextureSize, size);
    }
    public static void SetTextureUnit(int unit) {
        SetInt(UniformLocations.Texture, unit);
    }

    public static class AttributePositions {
        public const int Position = 0;
    }

    private static class UniformLocations {
        public const int ProjectionMatrix = 0;
        public const int TextureSize = 1;
        public const int Texture = 2;
    }
}
