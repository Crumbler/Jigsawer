

using Jigsawer.GLBuffers;
using Jigsawer.GLObjects;
using Jigsawer.Helpers;
using Jigsawer.Shaders.Programs;
using Jigsawer.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Drawing;

namespace Jigsawer.Models;

public class TextBlock : IRenderableModel {
    private readonly VAO vao;
    private readonly VBO dataVBO;
    private readonly Texture fontTexture;
    private readonly TextBlockShaderProgram shader;
    private readonly int displayedCharacters;
    private const int PosSize = sizeof(uint),
        IndexSize = sizeof(int),
        ColorSize = sizeof(byte) * 4,
        SizeMultSize = sizeof(float);

    public TextBlock(ReadOnlySpan<char> text, Vector2 position, Color color,
        float padding, float size, ref Matrix3 projMat) {
        var fontAtlas = FontAtlas.GetFontAtlas((int)size);

        fontTexture = fontAtlas.Texture;

        displayedCharacters = CalculateDisplayedCharacterCount(text);

        int bufferSize = displayedCharacters * (PosSize + IndexSize) + ColorSize + SizeMultSize;

        dataVBO = new VBO(bufferSize);

        float sizeMult = size / fontAtlas.EmSize;

        IntPtr ptr = dataVBO.Map();
        CalculateAndStoreCharacterPositions(text, position, sizeMult,
            fontAtlas, ptr, padding, displayedCharacters);

        ptr += displayedCharacters * PosSize;
        StoreCharacterIndices(text, ptr, displayedCharacters);

        ptr += displayedCharacters * IndexSize;
        StoreColor(color, ptr);

        ptr += ColorSize;
        StoreSizeMult(sizeMult, ptr);

        dataVBO.Unmap();

        vao = new VAO();

        int offset = 0;
        vao.SetBindingPointToBuffer(0, dataVBO.Id, offset, PosSize);
        vao.SetBindingPointDivisor(0, 1);

        vao.EnableVertexAttributeArray(TextBlockShaderProgram.AttributePositions.Position);
        vao.BindAttributeToPoint(TextBlockShaderProgram.AttributePositions.Position, 0);
        vao.SetAttributeFormat(
            TextBlockShaderProgram.AttributePositions.Position,
            2, VertexAttribType.HalfFloat);

        offset += displayedCharacters * PosSize;
        vao.SetBindingPointToBuffer(1, dataVBO.Id, offset, IndexSize);
        vao.SetBindingPointDivisor(1, 1);

        vao.EnableVertexAttributeArray(TextBlockShaderProgram.AttributePositions.CharacterId);
        vao.BindAttributeToPoint(TextBlockShaderProgram.AttributePositions.CharacterId, 1);
        vao.SetIntegerAttributeFormat(
            TextBlockShaderProgram.AttributePositions.CharacterId,
            1, VertexAttribIntegerType.UnsignedByte);

        offset += displayedCharacters * IndexSize;
        vao.SetBindingPointToBuffer(2, dataVBO.Id, offset);

        vao.EnableVertexAttributeArray(TextBlockShaderProgram.AttributePositions.Color);
        vao.BindAttributeToPoint(TextBlockShaderProgram.AttributePositions.Color, 2);
        vao.SetAttributeFormat(TextBlockShaderProgram.AttributePositions.Color,
            4, VertexAttribType.UnsignedByte, true);

        vao.EnableVertexAttributeArray(TextBlockShaderProgram.AttributePositions.SizeMult);
        vao.BindAttributeToPoint(TextBlockShaderProgram.AttributePositions.SizeMult, 2);
        vao.SetAttributeFormat(TextBlockShaderProgram.AttributePositions.SizeMult,
            1, VertexAttribType.Float, offset: ColorSize);

        shader = TextBlockShaderProgram.GetInstance(fontAtlas);
        shader.SetProjectionMatrix(ref projMat);
        shader.SetTextureUnit(fontTexture.Unit);
    }

    private static int CalculateDisplayedCharacterCount(ReadOnlySpan<char> chars) {
        return chars.Length - chars.Count(' ') - chars.Count('\n');
    }

    private static unsafe void CalculateAndStoreCharacterPositions(ReadOnlySpan<char> chars,
        Vector2 basePos, float sizeMult, FontAtlas fontAtlas, IntPtr positionsPtr,
        float padding,
        int displayedCharacters) {
        basePos += new Vector2(padding);

        float x = basePos.X;
        float y = basePos.Y + fontAtlas.MaxAscender;

        var characterMetrics = fontAtlas.CharacterMetrics;

        var positions = new Span<System.Half>(positionsPtr.ToPointer(), displayedCharacters * 2);
        int positionInd = 0;

        for (int i = 0; i < chars.Length; ++i) {
            char c = chars[i];
            switch (c) {
                case ' ':
                    x += fontAtlas.SpaceAdvance * sizeMult;
                    break;

                case '\n':
                    x = basePos.X;
                    
                    y += fontAtlas.CharacterHeight * sizeMult;
                    break;

                default:
                    var (bearingX, bearingY, advance) = characterMetrics[c - FontAtlas.MinChar];

                    positions[positionInd] = (System.Half)(x + bearingX * sizeMult);
                    positions[positionInd + 1] = (System.Half)(y - bearingY * sizeMult);
                    positionInd += 2;

                    x += advance * sizeMult;
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

    private static unsafe void StoreColor(Color color, IntPtr colorPtr) {
        ref var endCol = ref colorPtr.ToReference<int>();
        endCol = color.ToInt();
    }

    private static unsafe void StoreSizeMult(float sizeMult, IntPtr sizePtr) {
        ref var mult = ref sizePtr.ToReference<float>();
        mult = sizeMult;
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
