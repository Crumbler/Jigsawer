
namespace Jigsawer.Shaders.Programs;

public sealed class NormalImageShaderProgram : ImageShaderProgram {
    private const string EntityName = "Image.Normal";

    public NormalImageShaderProgram(int sharedInfoUboBindingPoint) {
        InitializeImageShader(EntityName, sharedInfoUboBindingPoint);
    }

    public override void SetScaleFactor(float x) {
        SetFloat(UniformLocations.ScaleFactor, x);
    }

    public override void SetTextureUnit(int unit) {
        SetInt(UniformLocations.Texture, unit);
    }

    private static class UniformLocations {
        public const int ScaleFactor = 0;
        public const int Texture = 1;
    }
}
