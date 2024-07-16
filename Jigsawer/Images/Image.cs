
namespace Jigsawer.Images;

public static class Image {
    public static string GetPath(string imageName) {
        return "Jigsawer.Images." + imageName;
    }

    public const string MainMenuBackgroundTile = "MainMenuBackgroundTile.png";
}
