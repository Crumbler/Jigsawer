
using Jigsawer.Models;

using OpenTK.Mathematics;

namespace Jigsawer.Scenes;

public sealed class MainMenuScene : Scene {
    private readonly ImageModel backgroundImage;
    private readonly MainMenuPuzzlesModel backgroundPuzzles;

    public MainMenuScene() : base() {
        backgroundImage = new ImageModel(ref projMat, 0.5f) {
            Rect = new Box2(Vector2.Zero, FramebufferSize)
        };

        backgroundPuzzles = new MainMenuPuzzlesModel(TotalMilliseconds, FramebufferSize);
    }

    protected override void Close() {
        base.Close();

        backgroundImage.Delete();
        backgroundPuzzles.Delete();
    }

    public override void OnFramebufferResize(Vector2i newSize) {
        base.OnFramebufferResize(newSize);

        backgroundImage.Rect = new Box2(Vector2.Zero, FramebufferSize);
        backgroundImage.UpdateProjectionMatrix(ref projMat);

        backgroundPuzzles.UpdateDrawSize(newSize);
    }

    public override void Render() {
        backgroundImage.Render();

        backgroundPuzzles.Render();
    }

    public override void Update(double secondsPassed) {
        base.Update(secondsPassed);

        backgroundPuzzles.UpdateTime(TotalMilliseconds);
    }
}
