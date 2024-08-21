using Jigsawer.Scenes;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System.Runtime.InteropServices;

namespace Jigsawer.Main;

public sealed class Game : GameWindow {
    private WindowState previousWindowState = WindowState.Normal;
    private Scene? currentScene;

#if DEBUG
    private static readonly DebugProc debugMessageDelegate = OnDebugMessage;
#endif

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

#if DEBUG
        GL.DebugMessageCallback(debugMessageDelegate, 0);
        GL.Enable(EnableCap.DebugOutput);
#endif
    }

#if DEBUG
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source">Source of the debugging message.</param>
    /// <param name="type">Type of the debugging message.</param>
    /// <param name="id">ID associated with the message.</param>
    /// <param name="severity">Severity of the message.</param>
    /// <param name="length">Length of the string in pMessage</param>
    /// <param name="pMessage">Pointer to message string</param>
    /// <param name="pUserParam">User specified parameter</param>
    private static void OnDebugMessage(
        DebugSource source,
        DebugType type,
        int id,
        DebugSeverity severity,
        int length,
        IntPtr pMessage,
        IntPtr pUserParam) {
        string message = Marshal.PtrToStringAnsi(pMessage, length);
        string sourceString = DebugHelper.OpenGLDebugSourceToString(source);
        string typeString = DebugHelper.OpenGLDebugTypeToString(type);
        string severityString = DebugHelper.OpenGLDebugSeverityToString(severity);

        Console.WriteLine($"[Severity = {severityString}, source = {sourceString}, type = {typeString}, id = {id}]\n{message}");
    }
#endif

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        Viewport.Size = new Vector2i(e.Width, e.Height);

        currentScene?.OnFramebufferResize(e.Size);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
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
            default:
                throw new ArgumentException($"Scene type {sceneType} not found.", nameof(sceneType));
        }

        newScene.OnTransfer = SwitchToScene;

        currentScene = newScene;
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        currentScene?.Update(args.Time);
    }
}
