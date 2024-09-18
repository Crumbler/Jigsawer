
using Jigsawer.GLBuffers;
using Jigsawer.GLBuffers.Interfaces;
using Jigsawer.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public sealed class TextBlockShaderProgram : ShaderProgram {
    private const string EntityName = "TextBlock";
    private static readonly Dictionary<FontAtlas, TextBlockShaderProgram> instances = [];
    private readonly UBO<FontInfo> fontInfoUbo;
    private readonly FontAtlas fontAtlas;
    private int instanceCount = 1;

    private TextBlockShaderProgram(FontAtlas fontAtlas) {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        this.fontAtlas = fontAtlas;
        fontInfoUbo = new UBO<FontInfo>();

        ConnectUniformBlockToBuffer(UniformBlockNames.FontData, fontInfoUbo.BindingPoint);

        FillFontInfoUBO(fontInfoUbo, fontAtlas);
    }

    private static void FillFontInfoUBO(UBO<FontInfo> ubo, FontAtlas fontAtlas) {
        ref var fontInfo = ref ubo.Map();

        fontInfo.fontHeight = fontAtlas.CharacterHeight;
        fontAtlas.CharacterSizes.CopyTo(fontInfo.CharacterSizes);

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
        public const int Position = 0,
            CharacterId = 1,
            Color = 2,
            SizeMult = 3;
    }

    private static class UniformLocations {
        public const int ProjectionMatrix = 0,
            Texture = 1;
    }

    private static class UniformBlockNames {
        public const string FontData = "FontData";
    }
}
