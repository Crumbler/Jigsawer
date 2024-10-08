﻿
using Jigsawer.Debug;
using Jigsawer.GLBuffers;
using Jigsawer.GLBuffers.Interfaces;
using Jigsawer.GLObjects;
using Jigsawer.Shaders.Programs;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Jigsawer.Scenes;

public abstract class Scene {
    private double secondsAccumulator;
    protected Matrix3 projMat;
    protected UBO<SharedInfo> sharedInfo;

    protected Scene() {
        FramebufferSize = Viewport.Size;
        CalculateProjectionMatrix(FramebufferSize);

        sharedInfo = new UBO<SharedInfo>();
        sharedInfo.Bind();

        ref var info = ref sharedInfo.Map();
        info.time = TotalMilliseconds;
        info.SetProjectionMatrix(in projMat);
        sharedInfo.Unmap();
    }

    protected int TotalMilliseconds { get; private set; }
    protected Vector2i FramebufferSize { get; private set; }
    protected Vector2 CursorPos { get; private set; }
    public Action<SceneType> SceneTransferAction { get; set; }
    public Action ExitAction { get; set; }

    private void CalculateProjectionMatrix(Vector2i size) {
        var mat = Matrix3.CreateScale(size.X / 2f, -size.Y / 2f, 0f);

        mat.Row0.Z = size.X / 2f;
        mat.Row1.Z = size.Y / 2f;
        mat.Row2.Z = 1f;

        projMat = mat.Inverted();
    }

    protected void TransferToScene(SceneType sceneType) {
        Logger.LogDebug("Transfering to scene " + sceneType);
        SceneTransferAction.Invoke(sceneType);
    }

    /// <summary>
    /// WHen overriding make sure to call the base method.
    /// </summary>
    protected virtual void Close() {
        ShaderProgram.StopUsing();
        VAO.Unbind();
        UBO.UnbindAll();
        Texture.Unbind();
    }

    public abstract void Render();

    public void Update(double secondsPassed, Vector2 cursorPos) {
        secondsAccumulator += secondsPassed;

        double millis = Math.Floor(secondsAccumulator * 1000.0);
        int passedMs = (int)millis;

        TotalMilliseconds += passedMs;
        ref var info = ref sharedInfo.MapRange(SharedInfo.TimeOffset, SharedInfo.TimeSize);
        info.time = TotalMilliseconds;
        sharedInfo.Unmap();

        secondsAccumulator -= millis / 1000.0;

        CursorPos = cursorPos;

        Update(passedMs);
    }

    public virtual void OnMouseDown(MouseButtonEventArgs e) {

    }

    protected abstract void Update(int passedMs);

    /// <summary>
    /// Called when the framebuffer is resized.
    /// When overriding, make sure to call the base method.
    /// </summary>
    public virtual void OnFramebufferResize(Vector2i newSize) {
        CalculateProjectionMatrix(newSize);

        ref var info = ref sharedInfo.MapRange(SharedInfo.ProjectionMatrixOffset,
            SharedInfo.ProjectionMatrixSize);

        info.SetProjectionMatrix(in projMat);

        sharedInfo.Unmap();
        
        FramebufferSize = newSize;
    }
}
