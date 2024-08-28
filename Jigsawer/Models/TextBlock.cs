

using Jigsawer.Main;
using Jigsawer.Shaders.Programs;
using Jigsawer.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Models;

public class TextBlock {
    private readonly VAO vao;
    private readonly VBO dataVBO;
    private readonly Texture fontTexture;
    private readonly TextBlockShaderProgram shader;
    private readonly FontAtlas fontAtlas;
    private readonly int characterCount;
    private const int PosSize = sizeof(float) * 2,
        IndexSize = sizeof(int);

    public TextBlock(string text, Vector2 position, FontAtlas fontAtlas, ref Matrix3 projMat) {
        this.fontAtlas = fontAtlas;
        characterCount = text.Length;
        fontTexture = fontAtlas.Texture;

        dataVBO = VBO.Create(BufferUsageHint.StaticDraw);

        dataVBO.Reset(text.Length * (PosSize + IndexSize));

        IntPtr ptr = dataVBO.Map();
        CalculateAndStoreCharacterPositions(text, position, fontAtlas.CharacterWidths, ptr);

        ptr += text.Length * PosSize;
        StoreCharacterIndices(text, ptr);

        dataVBO.Unmap();

        vao = VAO.Create();
        vao.EnableVertexAttributeArray(TextBlockShaderProgram.AttributePositions.Position);
        vao.BindAttributeToPoint(TextBlockShaderProgram.AttributePositions.Position, 0);
        vao.SetBindingPointToBuffer(0, dataVBO.Id, 0, PosSize);
        vao.SetBindingPointDivisor(0, 1);
        vao.SetAttributeFormat(
            TextBlockShaderProgram.AttributePositions.Position,
            2, VertexAttribType.Float);

        vao.EnableVertexAttributeArray(TextBlockShaderProgram.AttributePositions.CharacterId);
        vao.BindAttributeToPoint(TextBlockShaderProgram.AttributePositions.CharacterId, 1);
        vao.SetBindingPointToBuffer(1, dataVBO.Id, text.Length * PosSize, IndexSize);
        vao.SetBindingPointDivisor(1, 1);
        vao.SetIntegerAttributeFormat(
            TextBlockShaderProgram.AttributePositions.CharacterId,
            1, VertexAttribIntegerType.UnsignedByte);

        shader = new TextBlockShaderProgram(fontAtlas);
        shader.SetProjectionMatrix(ref projMat);
        shader.SetTextureUnit(fontTexture.Unit);
    }

    private static unsafe void CalculateAndStoreCharacterPositions(ReadOnlySpan<char> chars,
        Vector2 basePos, ReadOnlySpan<float> charWidths, IntPtr positionsPtr) {
        float x = basePos.X;
        float y = basePos.Y;

        var positions = new Span<Vector2>(positionsPtr.ToPointer(), chars.Length * PosSize);

        for (int i = 0; i < chars.Length; ++i) {
            positions[i] = new Vector2(x, y);

            x += charWidths[i];
        }
    }

    private static unsafe void StoreCharacterIndices(ReadOnlySpan<char> chars,
        IntPtr indicesPtr) {
        var indices = new Span<int>(indicesPtr.ToPointer(), chars.Length * IndexSize);

        for (int i = 0; i < chars.Length; ++i) {
            indices[i] = chars[i] - FontAtlas.MinChar;
        }
    }

    public void UpdateProjectionMatrix(ref Matrix3 mat) {
        shader.SetProjectionMatrix(ref mat);
    }

    public void Render() {
        vao.Bind();

        shader.Use();

        fontTexture.Use();

        GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, characterCount);
    }

    public void Delete() {
        vao.Delete();
        dataVBO.Delete();
    }
}
