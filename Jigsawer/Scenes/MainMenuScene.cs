
using Jigsawer.GLObjects;
using Jigsawer.Helpers;
using Jigsawer.Models;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Jigsawer.Scenes;

public sealed class MainMenuScene : Scene {
    private readonly ImageModel backgroundImage;
    private readonly MainMenuPuzzlesModel backgroundPuzzles;
    private readonly ButtonsModel buttons;

    public MainMenuScene() : base() {
        backgroundImage = new ImageModel(sharedInfo.BindingPoint, 0.5f,
            Images.EmbeddedImage.MainMenuBackgroundTile, Texture.repeatingParameters) {
            Rect = new Box2(Vector2.Zero, FramebufferSize)
        };

        backgroundPuzzles = new MainMenuPuzzlesModel(FramebufferSize, sharedInfo.BindingPoint);

        ButtonInfo buttonSingleplayer = new(new Box2(200, 200, 500, 280),
            Color4.Gray.WithAlpha(0.8f), Color4.Black.WithAlpha(0.8f),
            Color4.White, 20, 50f,
            "Singleplayer", OnSingleplayer);

        ButtonInfo buttonExit = new(new Box2(200, 350, 500, 430),
            Color4.Gray.WithAlpha(0.8f), Color4.Black.WithAlpha(0.8f),
            Color4.White, 20, 50f,
            "Exit", OnExit);

        buttons = new ButtonsModel(sharedInfo.BindingPoint, buttonSingleplayer, buttonExit);
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

    private void OnSingleplayer() {
        TransferToScene(SceneType.SingleplayerStart);
    }

    private void OnExit() {
        ExitAction.Invoke();
    }

    public override void Render() {
        backgroundImage.Render();

        backgroundPuzzles.Render();

        buttons.Render();
    }

    protected override void Update(int passedMs) {
        buttons.Update(CursorPos, passedMs);
    }
}
