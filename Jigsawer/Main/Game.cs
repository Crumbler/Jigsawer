using Jigsawer.Debug;
using Jigsawer.GLObjects;
using Jigsawer.Scenes;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Jigsawer.Main;

public sealed class Game : GameWindow {
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
            APIVersion = new Version(4, 5),

            Flags =
#if DEBUG
            ContextFlags.Debug
#else
            ContextFlags.ForwardCompatible
#endif
        }) { }

    protected override void OnLoad() {
        base.OnLoad();

        CenterWindow();

        InitOpenGL();

        Logger.LogDebug("Loading game");

        SwitchToScene(SceneType.MainMenu);

        Logger.LogDebug("Loaded game");
    }

    private static void InitOpenGL() {
        GL.ClearColor(Color4.Black);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        DebugHelper.InitDebugLogging();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        Viewport.Size = new Vector2i(e.Width, e.Height);

        currentScene?.OnFramebufferResize(e.Size);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
        switch (e.Key) {
            case Keys.F4 when e.Alt:
                Close();
                break;

            case Keys.F11:
            case Keys.Enter when e.Alt:
                ToggleFullscreen();
                break;
        }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e) {
        currentScene?.OnMouseDown(e);
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
        GL.Clear(ClearBufferMask.ColorBufferBit);

        currentScene?.Render();

        SwapBuffers();
    }

    private void SwitchToScene(SceneType sceneType) {
        Scene newScene;
        
        switch (sceneType) {
            case SceneType.MainMenu:
                newScene = new MainMenuScene();
                break;

            case SceneType.SingleplayerStart:
                newScene = new SingleplayerStartScene();
                break;
            default:
                throw new ArgumentException($"Scene type {sceneType} not found.", nameof(sceneType));
        }

        newScene.SceneTransferAction = SwitchToScene;
        newScene.ExitAction = Close;

        currentScene = newScene;
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        currentScene?.Update(args.Time, MousePosition);
    }

    private new void Close() {
        Logger.LogDebug("Closing game");

        base.Close();
    }
}
