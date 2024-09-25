
namespace Jigsawer.Images;

public static class EmbeddedImage {
    public static string GetPath(string imageName) {
        return "Jigsawer.Images." + imageName;
    }

    public const string MainMenuBackgroundTile = "MainMenuBackgroundTile.png";
}
