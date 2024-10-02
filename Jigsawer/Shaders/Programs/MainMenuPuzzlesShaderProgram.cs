
using Jigsawer.Scenes;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public sealed class MainMenuPuzzlesShaderProgram : ShaderProgram {
    private const string EntityName = "MainMenuPuzzles";

    public MainMenuPuzzlesShaderProgram() {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        ConnectUniformBlockToBuffer(UniformBlockNames.SharedInfo, Globals.SharedInfoBindingPoint);
    }

    public void SetDrawSize(Vector2 drawSize) => SetVector2(UniformLocations.DrawSize, drawSize);

    private static class UniformLocations {
        public const int DrawSize = 0;
    }

    private static class UniformBlockNames {
        public const string SharedInfo = "SharedInfo";
    }
}
