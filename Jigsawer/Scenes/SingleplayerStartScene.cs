
using Jigsawer.Helpers;
using Jigsawer.Models;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System.Drawing;

namespace Jigsawer.Scenes;

public class SingleplayerStartScene : Scene {
    private readonly ImageModel backgroundImage;
    private readonly MainMenuPuzzlesModel backgroundPuzzles;
    private readonly ButtonsModel buttons;

    public SingleplayerStartScene() : base() {
        backgroundImage = new ImageModel(sharedInfo.BindingPoint, 0.5f) {
            Rect = new Box2(Vector2.Zero, FramebufferSize)
        };

        backgroundPuzzles = new MainMenuPuzzlesModel(FramebufferSize, sharedInfo.BindingPoint);

        ButtonInfo buttonStart = new(new Box2(200, 200, 500, 280),
            Color.Gray.WithAlpha(0.8f), Color.Black.WithAlpha(0.8f),
            Color.White, 20, 50f,
            "Start", OnStart);

        ButtonInfo buttonBack = new(new Box2(200, 350, 500, 430),
            Color.Gray.WithAlpha(0.8f), Color.Black.WithAlpha(0.8f),
            Color.White, 20, 50f,
            "Back", OnBack);

        buttons = new ButtonsModel(sharedInfo.BindingPoint, buttonStart, buttonBack);
    }

    protected override void Close() {
        base.Close();

        backgroundImage.Delete();
        backgroundPuzzles.Delete();
        buttons.Delete();
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
