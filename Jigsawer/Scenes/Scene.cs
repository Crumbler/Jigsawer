﻿
using Jigsawer.Main;

using OpenTK.Mathematics;

namespace Jigsawer.Scenes;

public abstract class Scene {
    protected Matrix3 projMat;

    protected Scene() {
        var viewport = Viewport.Get();
        FramebufferSize = viewport.Max;
        CalculateProjectionMatrix(FramebufferSize);
    }

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
    protected abstract void Close();

    public abstract void Render();
    public abstract void Update();

    /// <summary>
    /// Called when the framebuffer is resized.
    /// When overriding, 
    /// </summary>
    public virtual void OnFramebufferResize(Vector2i newSize) {
        CalculateProjectionMatrix(newSize);

        FramebufferSize = newSize;
    }
}
