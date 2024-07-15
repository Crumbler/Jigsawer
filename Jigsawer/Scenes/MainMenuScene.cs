
using Jigsawer.Models;
using Jigsawer.Shaders.Programs;

using OpenTK.Mathematics;

namespace Jigsawer.Scenes;

public sealed class MainMenuScene : Scene {
    private readonly ImageModel backgroundImage;
    private readonly ImageShaderProgram imageShader;

    public MainMenuScene() {
        backgroundImage = new ImageModel {
            Rect = new Box2(0, 0, 0.5f, 0.5f)
        };

        imageShader = ImageShaderProgram.Create();
    }

    protected override void Close() {
        backgroundImage.Delete();
        imageShader.Delete();
    }

    public override void Render() {
        imageShader.Use();
        backgroundImage.Render();
    }

    public override void Update() {
        
    }
}
