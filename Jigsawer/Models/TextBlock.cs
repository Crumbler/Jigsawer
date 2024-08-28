

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
    private readonly int displayedCharacters;
    private const int PosSize = sizeof(float) * 2,
        IndexSize = sizeof(int);

    public TextBlock(string text, Vector2 position, FontAtlas fontAtlas, ref Matrix3 projMat) {
        this.fontAtlas = fontAtlas;
        fontTexture = fontAtlas.Texture;

        dataVBO = VBO.Create(BufferUsageHint.StaticDraw);

        displayedCharacters = CalculateDisplayedCharacterCount(text);

        dataVBO.Reset(displayedCharacters * (PosSize + IndexSize));

        IntPtr ptr = dataVBO.Map();
        CalculateAndStoreCharacterPositions(text, position,
            fontAtlas, ptr, displayedCharacters);

        ptr += displayedCharacters * PosSize;
        StoreCharacterIndices(text, ptr, displayedCharacters);

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
        vao.SetBindingPointToBuffer(1, dataVBO.Id, displayedCharacters * PosSize, IndexSize);
        vao.SetBindingPointDivisor(1, 1);
        vao.SetIntegerAttributeFormat(
            TextBlockShaderProgram.AttributePositions.CharacterId,
            1, VertexAttribIntegerType.UnsignedByte);

        shader = new TextBlockShaderProgram(fontAtlas);
        shader.SetProjectionMatrix(ref projMat);
        shader.SetTextureUnit(fontTexture.Unit);
    }

    private static int CalculateDisplayedCharacterCount(ReadOnlySpan<char> chars) {
        return chars.Length - chars.Count(' ') - chars.Count('\n');
    }

    private static unsafe void CalculateAndStoreCharacterPositions(ReadOnlySpan<char> chars,
        Vector2 basePos, FontAtlas fontAtlas, IntPtr positionsPtr,
        int displayedCharacters) {
        float x = basePos.X;
        float y = basePos.Y;

        ReadOnlySpan<float> characterWidths = fontAtlas.CharacterWidths;

        var positions = new Span<Vector2>(positionsPtr.ToPointer(), displayedCharacters);
        int positionInd = 0;

        for (int i = 0; i < chars.Length; ++i) {
            char c = chars[i];
            switch (c) {
                case ' ':
                    x += fontAtlas.SpaceWidth;
                    break;

                case '\n':
                    x = basePos.X;
                    y += fontAtlas.CharacterHeight;
                    break;

                default:
                    positions[positionInd] = new Vector2(x, y);
                    ++positionInd;

                    x += characterWidths[i];
                    break;
            }
        }
    }

    private static unsafe void StoreCharacterIndices(ReadOnlySpan<char> chars,
        IntPtr indicesPtr, int displayedCharacters) {
        var indices = new Span<int>(indicesPtr.ToPointer(), displayedCharacters);
        int indexInd = 0;

        for (int i = 0; i < chars.Length; ++i) {
            char c = chars[i];

            switch (c) {
                case ' ':
                case '\n':
                    break;
                    
                default:
                    indices[indexInd] = c - FontAtlas.MinChar;
                    ++indexInd;
                    break;
            }
        }
    }

    public void UpdateProjectionMatrix(ref Matrix3 mat) {
        shader.SetProjectionMatrix(ref mat);
    }

    public void Render() {
        vao.Bind();

        shader.Use();

        fontTexture.Use();

        GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, displayedCharacters);
    }

    public void Delete() {
        vao.Delete();
        dataVBO.Delete();
    }
}
