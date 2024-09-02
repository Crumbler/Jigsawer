
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public class ButtonsShaderProgram : ShaderProgram {
    private const string EntityName = "Buttons";

    public ButtonsShaderProgram() {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));
    }

    public void SetProjectionMatrix(ref Matrix3 mat) {
        SetMatrix(UniformLocations.ProjectionMatrix, ref mat);
    }

    public static class AttributePositions {
        public const int Rectangle = 0,
            Color = 1,
            HoverColor = 2,
            HoverFactor = 3;
    }

    private static class UniformLocations {
        public const int ProjectionMatrix = 0;
    }
}
