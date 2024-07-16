
using Jigsawer.Models;
using Jigsawer.Shaders.Programs;

using OpenTK.Mathematics;

namespace Jigsawer.Scenes;

public sealed class MainMenuScene : Scene {
    private readonly ImageModel backgroundImage;
    private readonly ImageShaderProgram imageShader;

    public MainMenuScene() : base() {
        backgroundImage = new ImageModel {
            Rect = new Box2(Vector2.Zero, FramebufferSize)
        };

        imageShader = ImageShaderProgram.Create();

        imageShader.Use();
        imageShader.SetProjectionMatrix(ref projMat);
    }

    protected override void Close() {
        backgroundImage.Delete();
        imageShader.Delete();
    }

    public override void OnFramebufferResize(Vector2i newSize) {
        base.OnFramebufferResize(newSize);

        imageShader.Use();
        imageShader.SetProjectionMatrix(ref projMat);

        backgroundImage.Rect = new Box2(Vector2.Zero, FramebufferSize);
    }

    public override void Render() {
        imageShader.Use();
        backgroundImage.Render();
    }

    public override void Update() {
        
    }
}
