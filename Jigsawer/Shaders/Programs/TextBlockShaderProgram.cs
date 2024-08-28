
using Jigsawer.GLBuffers;
using Jigsawer.GLBuffers.Interfaces;
using Jigsawer.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public class TextBlockShaderProgram : ShaderProgram {
    private const string EntityName = "TextBlock";
    private const int FontInfoBindingPoint = 1;
    private readonly UBO<FontInfo> fontInfoUbo;

    public TextBlockShaderProgram(FontAtlas fontAtlas) {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        fontInfoUbo = new UBO<FontInfo>(FontInfoBindingPoint);

        FillFontInfoUbo(fontInfoUbo, fontAtlas);
    }

    private static void FillFontInfoUbo(UBO<FontInfo> ubo, FontAtlas fontAtlas) {
        ref var fontInfo = ref ubo.Map();

        fontInfo.fontHeight = fontAtlas.CharacterHeight;

        fontAtlas.CharacterWidths.CopyTo(fontInfo.CharacterWidths);

        ubo.Unmap();
    }

    public void SetProjectionMatrix(ref Matrix3 mat) {
        SetMatrix(UniformLocations.ProjectionMatrix, ref mat);
    }

    public void SetTextureUnit(int unit) {
        SetInt(UniformLocations.Texture, unit);
    }

    public override void Use() {
        base.Use();

        fontInfoUbo.Bind();
    }

    public static class AttributePositions {
        public const int Position = 0;
        public const int CharacterId = 1;
    }

    private static class UniformLocations {
        public const int ProjectionMatrix = 0;
        public const int Texture = 1;
    }
}
