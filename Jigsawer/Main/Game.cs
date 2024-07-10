using Jigsawer.Scenes;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Jigsawer.Main;

public sealed partial class Game : GameWindow {
    private WindowState previousWindowState = WindowState.Normal;
    private Scene? currentScene;

    public Game(int width, int height, string title) :
        base(new GameWindowSettings() {
            UpdateFrequency = 60
        }, new NativeWindowSettings() {
            ClientSize = (width, height),
            Title = title,
            Vsync = VSyncMode.Adaptive,
            API = ContextAPI.OpenGL,
            APIVersion = new Version(3, 3)
        }) { }

    protected override void OnLoad() {
        base.OnLoad();

        CenterWindow();

        GL.ClearColor(Color4.Black);

        SwitchToScene(SceneType.MainMenu);

        Logger.LogDebug("Loaded game");
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        base.OnFramebufferResize(e);


    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
        base.OnKeyDown(e);

        switch (e.Key) {
            case Keys.F4:
                if (e.Alt) {
                    Logger.LogDebug("Closing game");
                    Close();
                }
                break;

            case Keys.Enter:
                if (e.Alt) {
                    ToggleFullscreen();
                }
                break;
        }
    }

    private void ToggleFullscreen() {
        Logger.LogDebug("Toggled fullscreen");

        if (WindowState == WindowState.Fullscreen) {
            WindowState = previousWindowState;
        } else {
            previousWindowState = WindowState;
            WindowState = WindowState.Fullscreen;
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        currentScene?.Render();

        SwapBuffers();
    }

    private void SwitchToScene(SceneType sceneType) {
        switch (sceneType) {
            case SceneType.MainMenu:
                currentScene = new MainMenuScene() {
                    OnTransfer = SwitchToScene
                };
                break;
        }
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);

        currentScene?.Update();
    }
}
