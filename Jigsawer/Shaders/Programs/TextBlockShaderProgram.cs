
using Jigsawer.GLBuffers;
using Jigsawer.GLBuffers.Interfaces;
using Jigsawer.Text;

using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Shaders.Programs;

public sealed class TextBlockShaderProgram : ShaderProgram {
    private const string EntityName = "TextBlock";
    private static readonly Dictionary<FontAtlas, TextBlockShaderProgram> instances = [];
    private readonly UBO<FontInfo> fontInfoUbo;
    private readonly FontAtlas fontAtlas;
    private int instanceCount = 1;

    private TextBlockShaderProgram(FontAtlas fontAtlas, int sharedInfoUboBindingPoint) {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        this.fontAtlas = fontAtlas;
        fontInfoUbo = new UBO<FontInfo>();

        ConnectUniformBlockToBuffer(UniformBlockNames.FontInfo, fontInfoUbo.BindingPoint);
        ConnectUniformBlockToBuffer(UniformBlockNames.SharedInfo, sharedInfoUboBindingPoint);

        FillFontInfoUBO(fontInfoUbo, fontAtlas);
    }

    private static void FillFontInfoUBO(UBO<FontInfo> ubo, FontAtlas fontAtlas) {
        ref var fontInfo = ref ubo.Map();

        fontInfo.fontHeight = fontAtlas.CharacterHeight;
        fontAtlas.CharacterSizes.CopyTo(fontInfo.CharacterSizes);

        ubo.Unmap();
    }

    public void SetTextureUnit(int unit) {
        SetInt(UniformLocations.Texture, unit);
    }

    public override void Use() {
        base.Use();

        fontInfoUbo.Bind();
    }

    public static TextBlockShaderProgram GetInstance(FontAtlas atlas, int sharedInfoUboBindingPoint) {
        if (instances.TryGetValue(atlas, out var existingProgram)) {
            ++existingProgram.instanceCount;
            return existingProgram;
        }

        var newProgram = new TextBlockShaderProgram(atlas, sharedInfoUboBindingPoint);
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
        public const int Position = 0,
            CharacterId = 1,
            Color = 2,
            SizeMult = 3;
    }

    private static class UniformLocations {
        public const int Texture = 0;
    }
    private static class UniformBlockNames {
        public const string FontInfo = "FontInfo";
        public const string SharedInfo = "SharedInfo";
    }
}
