
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public sealed class ImageShaderProgram : ShaderProgram {
    private const string EntityName = "Image";

    private int projectionMatrixUniform;

    private ImageShaderProgram() { }

    public static ImageShaderProgram Create() {
        var program = new ImageShaderProgram();

        program.Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        return program;
    }

    protected override void BindAttributes() {
        BindAttribute(AttributePositions.Position, AttributeNames.Position);
    }

    public void SetProjectionMatrix(ref Matrix3 mat) {
        SetMatrix(projectionMatrixUniform, ref mat);
    }

    protected override void GetUniformLocations() {
        projectionMatrixUniform = GetUniformLocation(UniformNames.ProjectionMatrix);
    }

    public static class AttributePositions {
        public const int Position = 0;
    }

    private static class AttributeNames {
        public const string Position = "box";
    }

    private static class UniformNames {
        public const string ProjectionMatrix = "projMat";
    }
}
