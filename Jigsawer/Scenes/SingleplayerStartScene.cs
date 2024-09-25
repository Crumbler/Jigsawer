
using Jigsawer.GLObjects;
using Jigsawer.Helpers;
using Jigsawer.Models;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Jigsawer.Scenes;

public sealed class SingleplayerStartScene : Scene {
    private readonly ImageModel backgroundImage;
    private readonly MainMenuPuzzlesModel backgroundPuzzles;
    private readonly ButtonsModel buttons;
    private readonly PanelsModel imagePanel;

    public SingleplayerStartScene() : base() {
        backgroundImage = new ImageModel(sharedInfo.BindingPoint, 0.5f,
            Images.EmbeddedImage.MainMenuBackgroundTile, Texture.repeatingParameters) {
            Rect = new Box2(Vector2.Zero, FramebufferSize)
        };

        backgroundPuzzles = new MainMenuPuzzlesModel(FramebufferSize, sharedInfo.BindingPoint);

        var buttonColor = Color4.Gray.WithAlpha(0.8f);
        var buttonHoverColor = Color4.Black.WithAlpha(0.8f);
        var textColor = Color4.White;

        const float buttonWidth = 500f,
            buttonHeight = 60f,
            buttonX = 100f,
            buttonY = 200f,
            buttonGap = 30f,
            yDiff = buttonHeight + buttonGap;

        const float padding = 10f;

        ButtonInfo buttonStart = new(
            new Box2(buttonX, buttonY, buttonX + buttonWidth, buttonY + buttonHeight),
            buttonColor, buttonHoverColor, textColor,
            padding, 40f,
            "Start", OnStart, false);

        ButtonInfo buttonLoadFromClipboard = new(
            new Box2(buttonX, buttonY + yDiff,
            buttonX + buttonWidth, buttonY + yDiff + buttonHeight),
            buttonColor, buttonHoverColor, textColor,
            padding, 40f,
            "Load image from clipboard", OnLoadFromClipboard);

        ButtonInfo buttonLoadFromFile = new(
            new Box2(buttonX, buttonY + yDiff * 2f,
            buttonX + buttonWidth, buttonY + yDiff * 2f + buttonHeight),
            buttonColor, buttonHoverColor, textColor,
            padding, 40f,
            "Load image from file", OnLoadFromFile);

        ButtonInfo buttonBack = new(
            new Box2(buttonX, buttonY + yDiff * 3f,
                buttonX + buttonWidth, buttonY + yDiff * 3f + buttonHeight),
            buttonColor, buttonHoverColor, textColor,
            padding, 40f,
            "Back", OnBack);

        buttons = new ButtonsModel(sharedInfo.BindingPoint,
            buttonStart, buttonLoadFromClipboard, buttonLoadFromFile, buttonBack);
        
        var imagePanelInfo = new PanelInfo(CalculateImagePanelBox(),
            Color4.Cornsilk.WithAlpha(0.9f));

        imagePanel = new PanelsModel(sharedInfo.BindingPoint, imagePanelInfo);
    }

    private Box2 CalculateImagePanelBox() {
        var box = new Box2(650f, 50f, FramebufferSize.X - 50f, FramebufferSize.Y - 50f);
        
        return box;
    }

    public override void Close() {
        base.Close();

        backgroundImage.Delete();
        backgroundPuzzles.Delete();
        buttons.Delete();
        imagePanel.Delete();
    }

    private void OnStart() {

    }

    private void OnLoadFromClipboard() {

    }

    private void OnLoadFromFile() {

    }

    private void OnBack() {
        TransferToScene(SceneType.MainMenu);
    }

    public override void OnFramebufferResize(Vector2i newSize) {
        base.OnFramebufferResize(newSize);

        backgroundImage.Rect = new Box2(Vector2.Zero, FramebufferSize);
        
        backgroundPuzzles.UpdateDrawSize(newSize);

        imagePanel.SetPanelRect(0, CalculateImagePanelBox());
    }

    public override void Render() {
        backgroundImage.Render();

        backgroundPuzzles.Render();

        buttons.Render();

        imagePanel.Render();
    }

    public override void OnMouseDown(MouseButtonEventArgs e) {
        if (e.Button == MouseButton.Left) {
            buttons.TryClick(CursorPos);
        }
    }

    protected override void Update(int passedMs) {
        buttons.Update(CursorPos, passedMs);
    }
}
