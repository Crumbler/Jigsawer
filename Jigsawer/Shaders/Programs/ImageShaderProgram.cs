using OpenTK.Graphics.OpenGL;

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

    protected override void BindAttributes() {
        BindAttribute(AttributePositions.Position, AttributeNames.Position);
    }

    private static class AttributePositions {
        public const int Position = 0;
    }

    private static class AttributeNames {
        public const string Position = "position";
    }
}
