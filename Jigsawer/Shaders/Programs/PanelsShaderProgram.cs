
using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Shaders.Programs;

public class PanelsShaderProgram : ShaderProgram {
    private const string EntityName = "Panel";

    public PanelsShaderProgram(int sharedInfoUboBindingPoint) {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        ConnectUniformBlockToBuffer(UniformBlockNames.SharedInfo, sharedInfoUboBindingPoint);
    }

    public static class AttributePositions {
        public const int Rectangle = 0,
            Color = 1;
    }

    private static class UniformBlockNames {
        public const string SharedInfo = "SharedInfo";
    }
}
