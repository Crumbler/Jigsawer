﻿
using Jigsawer.GLObjects;
using Jigsawer.Helpers;
using Jigsawer.Models;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System.Drawing;

namespace Jigsawer.Scenes;

public sealed class SingleplayerStartScene : Scene {
    private readonly ImageModel backgroundImage;
    private readonly MainMenuPuzzlesModel backgroundPuzzles;
    private readonly ButtonsModel buttons;
    private readonly PanelsModel imagePanel;
    private ImageModel? puzzleImage;
    private Bitmap? puzzleBitmap;

    public SingleplayerStartScene() : base() {
        backgroundImage = new ImageModel(sharedInfo.BindingPoint,
            Images.EmbeddedImage.MainMenuBackgroundTile,
            ImageSizeMode.Normal,
            0.5f,
            Texture.repeatingParameters) {
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

        const float fontSize = 40f;

        ButtonInfo buttonStart = new(
            new Box2(buttonX, buttonY, buttonX + buttonWidth, buttonY + buttonHeight),
            buttonColor, buttonHoverColor, textColor,
            padding, fontSize,
            "Start", OnStart, false);

        ButtonInfo buttonLoadFromClipboard = new(
            new Box2(buttonX, buttonY + yDiff,
            buttonX + buttonWidth, buttonY + yDiff + buttonHeight),
            buttonColor, buttonHoverColor, textColor,
            padding, fontSize,
            "Load image from clipboard", OnLoadFromClipboard);

        ButtonInfo buttonLoadFromFile = new(
            new Box2(buttonX, buttonY + yDiff * 2f,
            buttonX + buttonWidth, buttonY + yDiff * 2f + buttonHeight),
            buttonColor, buttonHoverColor, textColor,
            padding, fontSize,
            "Load image from file", OnLoadFromFile);

        ButtonInfo buttonBack = new(
            new Box2(buttonX, buttonY + yDiff * 3f,
                buttonX + buttonWidth, buttonY + yDiff * 3f + buttonHeight),
            buttonColor, buttonHoverColor, textColor,
            padding, fontSize,
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

    private Box2 CalculateImageBox() {
        var box = new Box2(675f, 75f, FramebufferSize.X - 75f, FramebufferSize.Y - 75f);

        return box;
    }

    public override void Close() {
        backgroundImage.Delete();
        backgroundPuzzles.Delete();
        buttons.Delete();
        imagePanel.Delete();
        puzzleImage?.Delete();

        base.Close();
    }

    private void OnStart() {

    }

    private void OnLoadFromClipboard() {
        var bmp = ClipboardHelper.GetBitmap();
        if (bmp == null) {
            Console.WriteLine("Failed to get bitmap");
            return;
        }

        puzzleBitmap?.Dispose();

        puzzleBitmap = bmp;

        puzzleImage?.Delete();

        puzzleImage = new ImageModel(sharedInfo.BindingPoint, bmp, ImageSizeMode.Zoom) {
            Rect = CalculateImageBox()
        };
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

        if (puzzleImage != null) {
            puzzleImage.Rect = CalculateImageBox();
        }
    }

    public override void Render() {
        backgroundImage.Render();

        backgroundPuzzles.Render();

        buttons.Render();

        imagePanel.Render();
        puzzleImage?.Render();
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
