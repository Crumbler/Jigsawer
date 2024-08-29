
using Jigsawer.GLBuffers;
using Jigsawer.GLBuffers.Interfaces;
using Jigsawer.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public sealed class TextBlockShaderProgram : ShaderProgram {
    private const string EntityName = "TextBlock";
    private const int FontInfoBindingPoint = 1;
    private static readonly Dictionary<FontAtlas, TextBlockShaderProgram> instances = [];
    private readonly UBO<FontInfo> fontInfoUbo;
    private int instanceCount = 1;
    private readonly FontAtlas fontAtlas;

    private TextBlockShaderProgram(FontAtlas fontAtlas) {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        this.fontAtlas = fontAtlas;
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

    public static TextBlockShaderProgram GetInstance(FontAtlas atlas) {
        if (instances.TryGetValue(atlas, out var existingProgram)) {
            ++existingProgram.instanceCount;
            return existingProgram;
        }

        var newProgram = new TextBlockShaderProgram(atlas);
        instances.Add(atlas, newProgram);
        return newProgram;
    }

    public override void Delete() {
        --instanceCount;
        if (instanceCount == 0) {
            instances.Remove(fontAtlas);
            base.Delete();
        }
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
