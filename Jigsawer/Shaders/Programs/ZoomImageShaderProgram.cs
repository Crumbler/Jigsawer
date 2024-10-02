
namespace Jigsawer.Shaders.Programs;

public sealed class ZoomImageShaderProgram : ImageShaderProgram {
    private const string EntityName = "Image.Zoom";

    public ZoomImageShaderProgram() {
        InitializeImageShader(EntityName);
    }

    public override void SetScaleFactor(float x) { }

    public override void SetTextureUnit(int unit) {
        SetInt(UniformLocations.Texture, unit);
    }

    private static class UniformLocations {
        public const int Texture = 0;
    }
}
