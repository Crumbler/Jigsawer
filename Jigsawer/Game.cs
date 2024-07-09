
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Jigsawer;

public class Game : GameWindow {
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

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        SwapBuffers();
    }
}
