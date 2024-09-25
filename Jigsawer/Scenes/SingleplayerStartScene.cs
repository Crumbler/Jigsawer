
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

        const float buttonWidth = 300f,
            buttonHeight = 80f,
            buttonX = 200f,
            buttonY = 200f,
            buttonGap = 50f;

        ButtonInfo buttonStart = new(
            new Box2(buttonX, buttonY, buttonX + buttonWidth, buttonY + buttonHeight),
            buttonColor, buttonHoverColor, textColor,
            20, 50f,
            "Start", OnStart, false);

        ButtonInfo buttonBack = new(
            new Box2(buttonX, buttonY + buttonHeight + buttonGap,
                buttonX + buttonWidth, buttonY + buttonHeight + buttonGap + buttonHeight),
            buttonColor, buttonHoverColor, textColor,
            20, 50f,
            "Back", OnBack);

        buttons = new ButtonsModel(sharedInfo.BindingPoint, buttonStart, buttonBack);
        
        var imagePanelInfo = new PanelInfo(CalculateImagePanelBox(),
            Color4.Cornsilk.WithAlpha(0.9f));

        imagePanel = new PanelsModel(sharedInfo.BindingPoint, imagePanelInfo);
    }

    private Box2 CalculateImagePanelBox() {
        var box = new Box2(550f, 50f, FramebufferSize.X - 50f, FramebufferSize.Y - 50f);
        
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
