
using Jigsawer.GLBuffers;
using Jigsawer.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public class TextBlockShaderProgram : ShaderProgram {
    private const string EntityName = "TextBlock";
    // +1 for font height
    private const int FontInfoInterfaceSize = (FontAtlas.TotalChars + 1) * sizeof(float);
    private const int FontInfoBindingPoint = 1;
    private readonly UBO fontInfoUbo;

    public TextBlockShaderProgram(FontAtlas fontAtlas) {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        fontInfoUbo = new UBO(FontInfoBindingPoint, FontInfoInterfaceSize);

        FillFontInfoUbo(fontInfoUbo, fontAtlas);
    }

    private static void FillFontInfoUbo(UBO ubo, FontAtlas fontAtlas) {
        IntPtr ptr = ubo.Map();

        FillFontHeight(ptr, fontAtlas.CharacterHeight);

        ptr += sizeof(float);

        FillCharacterWidths(ptr, fontAtlas.CharacterWidths);

        ubo.Unmap();
    }

    private static unsafe void FillFontHeight(IntPtr ptr, int height) {
        float* fPtr = (float*)ptr.ToPointer();
        *fPtr = height;
    }

    private static unsafe void FillCharacterWidths(IntPtr ptr, ReadOnlySpan<float> widths) {
        var span = new Span<float>(ptr.ToPointer(), widths.Length);

        widths.CopyTo(span);
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
