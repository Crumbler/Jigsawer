
using Jigsawer.Shaders.Programs;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Models;

public sealed class MainMenuPuzzlesModel {
    private readonly MainMenuPuzzlesProgram shader;

    public MainMenuPuzzlesModel(int time, Vector2 drawSize) {
        shader = MainMenuPuzzlesProgram.Create();
        shader.Use();
        shader.SetTime(time);
        shader.SetDrawSize(drawSize);
    }

    public void Render() {
        shader.Use();

        GL.DrawArrays(PrimitiveType.Triangles, 0, 12);
    }

    public void SetTime(int time) {
        shader.Use();
        shader.SetTime(time);
    }

    public void SetDrawSize(Vector2 drawSize) {
        shader.Use();
        shader.SetDrawSize(drawSize);
    }

    public void Delete() {
        shader.Delete();
    }
}
