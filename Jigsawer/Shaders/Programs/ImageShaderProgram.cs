
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public sealed class ImageShaderProgram : ShaderProgram {
    private const string EntityName = "Image";

    private int projectionMatrixUniform,
        textureSizeUniform,
        textureUniform;

    private ImageShaderProgram() { }

    protected override void BindAttributes() {
        BindAttribute(AttributePositions.Position, AttributeNames.Position);
    }

    protected override void GetUniformLocations() {
        projectionMatrixUniform = GetUniformLocation(UniformNames.ProjectionMatrix);

        textureSizeUniform = GetUniformLocation(UniformNames.TextureSize);
        textureUniform = GetUniformLocation(UniformNames.Texture);
    }

    public static ImageShaderProgram Create() {
        var program = new ImageShaderProgram();

        program.Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        return program;
    }

    public void SetProjectionMatrix(ref Matrix3 mat) {
        SetMatrix(projectionMatrixUniform, ref mat);
    }
    public void SetTextureSize(Vector2 size) {
        SetVector2(textureSizeUniform, size);
    }
    public void SetTextureUnit(int unit) {
        SetInt(textureUniform, unit);
    }

    public static class AttributePositions {
        public const int Position = 0;
    }

    private static class AttributeNames {
        public const string Position = "box";
    }

    private static class UniformNames {
        public const string ProjectionMatrix = "projMat";
        public const string TextureSize = "textureSize";
        public const string Texture = "texture0";
    }
}
