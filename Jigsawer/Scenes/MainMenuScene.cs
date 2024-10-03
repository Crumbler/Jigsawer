
using Jigsawer.Entities;
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
    private readonly Buttons buttons;

    public MainMenuScene() : base() {
        backgroundImage = new ImageModel(Images.EmbeddedImage.MainMenuBackgroundTile,
            ImageSizeMode.Normal,
            0.5f,
            Texture.repeatingParameters) {
            Rect = new Box2(Vector2.Zero, FramebufferSize)
        };

        backgroundPuzzles = new MainMenuPuzzlesModel(FramebufferSize);

        var buttonColor = Color4.Gray.WithAlpha(0.8f);
        var buttonHoverColor = Color4.Black.WithAlpha(0.8f);
        var textColor = Color4.White;

        const float padding = 20f;
        const float fontSize = 50f;

        ButtonTextInfo buttonSingleplayer = new(
            new ButtonInfo(new Box2(200, 200, 500, 280),
                buttonColor, buttonHoverColor),
            new TextInfo("Singleplayer", new Vector2(200), textColor, padding, fontSize),
            OnSingleplayer);

        ButtonTextInfo buttonExit = new(
            new ButtonInfo(new Box2(200, 350, 500, 430),
                buttonColor, buttonHoverColor),
            new TextInfo("Exit", new Vector2(200, 350), textColor, padding, fontSize),
            OnExit);

        buttons = new Buttons(buttonSingleplayer, buttonExit);
    }

    public override void Close() {
        backgroundImage.Delete();
        backgroundPuzzles.Delete();

        base.Close();
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
