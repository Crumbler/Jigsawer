
using Jigsawer.Shaders.Programs;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Models;

public sealed class MainMenuPuzzlesModel {
    private readonly MainMenuPuzzlesProgram shader;
    private const int PuzzlePieceCount = 4;

    public MainMenuPuzzlesModel(int time, Vector2 drawSize) {
        shader = MainMenuPuzzlesProgram.Create();
        shader.Use();
        MainMenuPuzzlesProgram.SetTime(time);
        MainMenuPuzzlesProgram.SetDrawSize(drawSize);
    }

    public void Render() {
        shader.Use();

        GL.DrawArrays(PrimitiveType.Triangles, 0, PuzzlePieceCount * 3);
    }

    public void SetTime(int time) {
        shader.Use();
        MainMenuPuzzlesProgram.SetTime(time);
    }

    public void SetDrawSize(Vector2 drawSize) {
        shader.Use();
        MainMenuPuzzlesProgram.SetDrawSize(drawSize);
    }

    public void Delete() {
        shader.Delete();
    }
}
