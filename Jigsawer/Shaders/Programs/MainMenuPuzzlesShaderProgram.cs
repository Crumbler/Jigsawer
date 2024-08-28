
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public sealed class MainMenuPuzzlesShaderProgram : ShaderProgram {
    private const string EntityName = "MainMenuPuzzles";

    public MainMenuPuzzlesShaderProgram() {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));
    }

    public void SetTime(int time) => SetInt(UniformLocations.Time, time);
    public void SetDrawSize(Vector2 drawSize) => SetVector2(UniformLocations.DrawSize, drawSize);

    private static class UniformLocations {
        public const int Time = 0;
        public const int DrawSize = 1;
    }
}
