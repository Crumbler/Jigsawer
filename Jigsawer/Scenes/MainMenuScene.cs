
using Jigsawer.Helpers;
using Jigsawer.Models;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System.Drawing;

namespace Jigsawer.Scenes;

public sealed class MainMenuScene : Scene {
    private readonly ImageModel backgroundImage;
    private readonly MainMenuPuzzlesModel backgroundPuzzles;
    private readonly ButtonsModel buttons;

    public MainMenuScene() : base() {
        backgroundImage = new ImageModel(sharedInfo.BindingPoint, 0.5f) {
            Rect = new Box2(Vector2.Zero, FramebufferSize)
        };

        backgroundPuzzles = new MainMenuPuzzlesModel(FramebufferSize, sharedInfo.BindingPoint);
        const string buttonText = "Some text";

        ButtonInfo button = new(new Box2(200, 400, 400, 500),
            Color.Gray.WithAlpha(0.8f), Color.Black.WithAlpha(0.8f),
            Color.White, 10, 40f,
            buttonText, static () => Console.WriteLine("Click"));

        buttons = new ButtonsModel(sharedInfo.BindingPoint, button);
    }

    protected override void Close() {
        base.Close();

        backgroundImage.Delete();
        backgroundPuzzles.Delete();
    }

    public override void OnFramebufferResize(Vector2i newSize) {
        base.OnFramebufferResize(newSize);

        backgroundImage.Rect = new Box2(Vector2.Zero, FramebufferSize);

        backgroundPuzzles.UpdateDrawSize(newSize);
    }

    public override void OnMouseDown(MouseButtonEventArgs e) {
        if (e.Button == MouseButton.Left) {
            buttons.TryClick(CursorPos);
        }
    }

    public override void Render() {
        backgroundImage.Render();

        //backgroundPuzzles.Render();

        buttons.Render();
    }

    protected override void Update(int passedMs) {
        buttons.Update(CursorPos, passedMs);
    }
}
