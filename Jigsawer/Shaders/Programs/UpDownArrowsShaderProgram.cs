
using Jigsawer.Scenes;

using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Shaders.Programs;

public sealed class UpDownArrowsShaderProgram : ShaderProgram {
    private const string EntityName = "UpDownArrows";

    public UpDownArrowsShaderProgram() {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        ConnectUniformBlockToBuffer(UniformBlockNames.SharedInfo, Globals.SharedInfoBindingPoint);
    }

    public static class AttributePositions {
        public const int Rectangle = 0,
            IsDownArrow = 1,
            Color = 2;
    }

    private static class UniformBlockNames {
        public const string SharedInfo = "SharedInfo";
    }
}
