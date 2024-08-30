
using Jigsawer.Shaders.Programs;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Models;

public sealed class MainMenuPuzzlesModel {
    private const int PuzzlePieceCount = 4;
    private readonly MainMenuPuzzlesShaderProgram shader;

    public MainMenuPuzzlesModel(int time, Vector2 drawSize) {
        shader = new MainMenuPuzzlesShaderProgram();
        shader.SetTime(time);
        shader.SetDrawSize(drawSize);
    }

    public void Render() {
        shader.Use();

        GL.DrawArrays(PrimitiveType.Triangles, 0, PuzzlePieceCount * 3);
    }

    public void UpdateTime(int time) {
        shader.SetTime(time);
    }

    public void UpdateDrawSize(Vector2 drawSize) {
        shader.SetDrawSize(drawSize);
    }

    public void Delete() {
        shader.Delete();
    }
}
