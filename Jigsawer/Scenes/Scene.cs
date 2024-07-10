
namespace Jigsawer.Scenes;

public abstract class Scene {
    public required Action<SceneType> OnTransfer { get; init; }
    protected void TransferToScene(SceneType sceneType) => OnTransfer(sceneType);
    public abstract void Render();
    public abstract void Update();
    public abstract void Close();
}
