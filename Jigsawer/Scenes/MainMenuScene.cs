
using Jigsawer.Models;

using OpenTK.Mathematics;

namespace Jigsawer.Scenes;

public sealed class MainMenuScene : Scene {
    private readonly ImageModel backgroundImage;

    public MainMenuScene() : base() {
        backgroundImage = new ImageModel(ref projMat, 0.5f) {
            Rect = new Box2(Vector2.Zero, FramebufferSize)
        };
    }

    protected override void Close() {
        base.Close();

        backgroundImage.Delete();
    }

    public override void OnFramebufferResize(Vector2i newSize) {
        base.OnFramebufferResize(newSize);

        backgroundImage.Rect = new Box2(Vector2.Zero, FramebufferSize);
        backgroundImage.UpdateProjectionMatrix(ref projMat);
    }

    public override void Render() {
        backgroundImage.Render();
    }

    public override void Update() {
        
    }
}
