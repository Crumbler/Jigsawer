
using Jigsawer.Debug;
using Jigsawer.Entities;
using Jigsawer.GLObjects;
using Jigsawer.Helpers;
using Jigsawer.Models;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System.Drawing;
using System.Windows.Forms;

namespace Jigsawer.Scenes;

public sealed class SingleplayerStartScene : Scene {
    private readonly ImageModel backgroundImage;
    private readonly MainMenuPuzzlesModel backgroundPuzzles;
    private readonly Buttons buttons;
    private readonly PanelsModel imagePanel;
    private ImageModel? puzzleImage;
    private Bitmap? puzzleBitmap;

    public SingleplayerStartScene() : base() {
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

        const float buttonWidth = 500f,
            buttonHeight = 60f,
            buttonX = 100f,
            buttonY = 200f,
            buttonGap = 30f,
            yDiff = buttonHeight + buttonGap;

        const float padding = 10f;

        const float fontSize = 40f;

        ButtonTextInfo buttonStart = new(
            new ButtonInfo(new Box2(buttonX, buttonY,
                buttonX + buttonWidth, buttonY + buttonHeight),
                buttonColor, buttonHoverColor, false),
            new TextInfo("Start", new Vector2(buttonX, buttonY), textColor, padding, fontSize),
            OnStart);

        ButtonTextInfo buttonLoadFromClipboard = new(
            new ButtonInfo(new Box2(buttonX, buttonY + yDiff,
                buttonX + buttonWidth, buttonY + yDiff + buttonHeight),
                buttonColor, buttonHoverColor),
            new TextInfo("Load image from clipboard", new Vector2(buttonX, buttonY + yDiff),
                textColor, padding, fontSize),
            OnLoadFromClipboard);

        ButtonTextInfo buttonLoadFromFile = new(
            new ButtonInfo(new Box2(buttonX, buttonY + yDiff * 2f,
                buttonX + buttonWidth, buttonY + yDiff * 2f + buttonHeight),
                buttonColor, buttonHoverColor),
            new TextInfo("Load image from file", new Vector2(buttonX, buttonY + yDiff * 2f),
                textColor, padding, fontSize),
            OnLoadFromFile);

        ButtonTextInfo buttonBack = new(
            new ButtonInfo(new Box2(buttonX, buttonY + yDiff * 3f,
                buttonX + buttonWidth, buttonY + yDiff * 3f + buttonHeight),
                buttonColor, buttonHoverColor),
            new TextInfo("Back", new Vector2(buttonX, buttonY + yDiff * 3f),
                textColor, padding, fontSize),
            OnBack);

        buttons = new Buttons(buttonStart, buttonLoadFromClipboard,
            buttonLoadFromFile, buttonBack);

        var imagePanelInfo = new PanelInfo(CalculateImagePanelBox(),
            Color4.Cornsilk.WithAlpha(0.9f));

        imagePanel = new PanelsModel(imagePanelInfo);
    }

    private Box2 CalculateImagePanelBox() {
        var box = new Box2(650f, 50f, FramebufferSize.X - 50f, FramebufferSize.Y - 50f);
        
        return box;
    }

    private Box2 CalculateImageBox() {
        var box = new Box2(675f, 75f, FramebufferSize.X - 75f, FramebufferSize.Y - 100f);

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

    private void OnStart(MouseButtonEventArgs eventArgs) {

    }

    private void OnLoadFromClipboard(MouseButtonEventArgs eventArgs) {
        var bmp = ClipboardHelper.GetBitmap();
        if (bmp == null) {
            Logger.LogDebug("Failed to get bitmap from clipboard");
            return;
        }

        DisplayImage(bmp);
    }

    private void OnLoadFromFile(MouseButtonEventArgs eventArgs) {
        using var openFileDialog = new OpenFileDialog();

        openFileDialog.ShowPreview = true;
        openFileDialog.Title = "Select an image file";
        openFileDialog.CheckFileExists = true;

        var res = openFileDialog.ShowDialog();
        if (res != DialogResult.OK) {
            return;
        }

        Bitmap bmp;

        try {
#pragma warning disable CA2000 // No object to dispose if an exception is caught
            bmp = new Bitmap(openFileDialog.FileName);
#pragma warning restore CA2000
        }
        catch (FileNotFoundException) {
            Logger.LogDebug("File not found");
            return;
        }
        catch {
            Logger.LogDebug("Incorrect file format");
            return;
        }

        DisplayImage(bmp);
    }

    private void DisplayImage(Bitmap bmp) {
        puzzleBitmap?.Dispose();

        puzzleBitmap = bmp;

        puzzleImage?.Delete();

        puzzleImage = new ImageModel(bmp, ImageSizeMode.Zoom) {
            Rect = CalculateImageBox()
        };
    }

    private void OnBack(MouseButtonEventArgs eventArgs) {
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
            buttons.TryClick(CursorPos, e);
        }
    }

    protected override void Update(int passedMs) {
        buttons.Update(CursorPos, passedMs);
    }
}
