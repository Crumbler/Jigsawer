
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
    private readonly PanelsModel imagaPanel;

    public SingleplayerStartScene() : base() {
        backgroundImage = new ImageModel(sharedInfo.BindingPoint, 0.5f) {
            Rect = new Box2(Vector2.Zero, FramebufferSize)
        };

        backgroundPuzzles = new MainMenuPuzzlesModel(FramebufferSize, sharedInfo.BindingPoint);

        ButtonInfo buttonStart = new(new Box2(200, 200, 500, 280),
            Color4.Gray.WithAlpha(0.8f), Color4.Black.WithAlpha(0.8f),
            Color4.White, 20, 50f,
            "Start", OnStart, false);

        ButtonInfo buttonBack = new(new Box2(200, 350, 500, 430),
            Color4.Gray.WithAlpha(0.8f), Color4.Black.WithAlpha(0.8f),
            Color4.White, 20, 50f,
            "Back", OnBack);

        buttons = new ButtonsModel(sharedInfo.BindingPoint, buttonStart, buttonBack);
        
        var imagePanelInfo = new PanelInfo(new Box2(600, 200, 1000, 800), Color4.Blue);

        imagaPanel = new PanelsModel(sharedInfo.BindingPoint, imagePanelInfo);
    }

    protected override void Close() {
        base.Close();

        backgroundImage.Delete();
        backgroundPuzzles.Delete();
        buttons.Delete();
        imagaPanel.Delete();
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
    }

    public override void Render() {
        backgroundImage.Render();

        backgroundPuzzles.Render();

        imagaPanel.Render();

        buttons.Render();
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
