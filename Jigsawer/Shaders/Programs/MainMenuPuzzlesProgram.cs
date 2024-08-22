
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public sealed class MainMenuPuzzlesProgram : ShaderProgram {
    private const string EntityName = "MainMenuPuzzles";

    public static MainMenuPuzzlesProgram Create() {
        var program = new MainMenuPuzzlesProgram();
        
        program.Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        return program;
    }

    protected override void BindAttributes() { }

    public static void SetTime(int time) => SetInt(UniformLocations.Time, time);
    public static void SetDrawSize(Vector2 drawSize) => SetVector2(UniformLocations.DrawSize, drawSize);

    private static class UniformLocations {
        public const int Time = 0;
        public const int DrawSize = 1;
    }
}
