
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Jigsawer;

public sealed class Game : GameWindow {

    private WindowState previousWindowState = WindowState.Normal;

    public Game(int width, int height, string title) :
        base(new GameWindowSettings() {
            UpdateFrequency = 60
        }, new NativeWindowSettings() {
            ClientSize = (width, height),
            Title = title,
            Vsync = VSyncMode.Adaptive
        }) { }

    protected override void OnLoad() {
        base.OnLoad();

        CenterWindow();
        
        GL.ClearColor(Color4.Black);


    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        base.OnFramebufferResize(e);

        
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
        base.OnKeyDown(e);
        
        switch (e.Key) {
            case Keys.F4:
                if (e.Alt) {
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

        SwapBuffers();
    }
}
