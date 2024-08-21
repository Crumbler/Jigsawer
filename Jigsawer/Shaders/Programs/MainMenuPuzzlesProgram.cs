
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public sealed class MainMenuPuzzlesProgram : ShaderProgram {
    private const string EntityName = "MainMenuPuzzles";

    private int drawSizeUniform,
        timeUniform;

    public static MainMenuPuzzlesProgram Create() {
        var program = new MainMenuPuzzlesProgram();
        
        program.Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        return program;
    }

    protected override void BindAttributes() { }

    protected override void GetUniformLocations() {
        drawSizeUniform = GetUniformLocation(UniformNames.DrawSize);
        timeUniform = GetUniformLocation(UniformNames.Time);
    }

    public void SetTime(int time) => SetInt(timeUniform, time);
    public void SetDrawSize(Vector2 drawSize) => SetVector2(drawSizeUniform, drawSize);

    private static class UniformNames {
        public const string DrawSize = "drawSize";
        public const string Time = "time";
    }
}
