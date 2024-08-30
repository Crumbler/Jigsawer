﻿
using Jigsawer.GLBuffers;
using Jigsawer.GLObjects;
using Jigsawer.Shaders.Programs;

using OpenTK.Mathematics;

namespace Jigsawer.Scenes;

public abstract class Scene {
    private double secondsAccumulator;
    protected Matrix3 projMat;

    protected Scene() {
        FramebufferSize = Viewport.Size;
        CalculateProjectionMatrix(FramebufferSize);
    }

    protected int TotalMilliseconds { get; private set; }
    protected Vector2i FramebufferSize { get; private set; }
    public Action<SceneType>? OnTransfer { get; set; }

    private void CalculateProjectionMatrix(Vector2i size) {
        var mat = Matrix3.CreateScale(size.X / 2f, -size.Y / 2f, 0f);

        mat.Row0.Z = size.X / 2f;
        mat.Row1.Z = size.Y / 2f;
        mat.Row2.Z = 1f;

        projMat = mat.Inverted();
    }

    protected void TransferToScene(SceneType sceneType) => OnTransfer?.Invoke(sceneType);

    /// <summary>
    /// WHen overriding make sure to call the base method.
    /// </summary>
    protected virtual void Close() {
        ShaderProgram.StopUsing();
        VAO.Unbind();
        VBO.Unbind();
        UBO.UnbindAll();
        Texture.Unbind();
    }

    public abstract void Render();

    /// <summary>
    /// When overriding make sure to call the base method.
    /// </summary>
    public virtual void Update(double secondsPassed) {
        secondsAccumulator += secondsPassed;

        double millis = Math.Floor(secondsAccumulator * 1000.0);

        TotalMilliseconds += (int)millis;

        secondsAccumulator -= millis / 1000.0;
    }

    /// <summary>
    /// Called when the framebuffer is resized.
    /// When overriding, make sure to call the base method.
    /// </summary>
    public virtual void OnFramebufferResize(Vector2i newSize) {
        CalculateProjectionMatrix(newSize);

        FramebufferSize = newSize;
    }
}
