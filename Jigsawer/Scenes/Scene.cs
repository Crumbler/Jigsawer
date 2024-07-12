
namespace Jigsawer.Scenes;

public abstract class Scene {
    public Action<SceneType>? OnTransfer { get; set; }
    protected void TransferToScene(SceneType sceneType) => OnTransfer?.Invoke(sceneType);
    public abstract void Render();
    public abstract void Update();
    protected abstract void Close();
}
