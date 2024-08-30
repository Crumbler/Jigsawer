

using Jigsawer.GLBuffers;
using Jigsawer.GLObjects;
using Jigsawer.Shaders.Programs;
using Jigsawer.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Runtime.CompilerServices;

namespace Jigsawer.Models;

public class TextBlock {
    private readonly VAO vao;
    private readonly VBO dataVBO;
    private readonly Texture fontTexture;
    private readonly TextBlockShaderProgram shader;
    private readonly FontAtlas fontAtlas;
    private readonly int displayedCharacters;
    private const int PosSize = sizeof(float) * 2,
        IndexSize = sizeof(int),
        ColorSize = sizeof(float) * 3;

    public TextBlock(string text, Vector2 position, Color4 color, FontAtlas fontAtlas, ref Matrix3 projMat) {
        this.fontAtlas = fontAtlas;
        fontTexture = fontAtlas.Texture;

        dataVBO = new VBO(BufferUsageHint.StaticDraw);

        displayedCharacters = CalculateDisplayedCharacterCount(text);

        dataVBO.Reset(displayedCharacters * (PosSize + IndexSize) + ColorSize);

        IntPtr ptr = dataVBO.Map();
        CalculateAndStoreCharacterPositions(text, position,
            fontAtlas, ptr, displayedCharacters);

        ptr += displayedCharacters * PosSize;
        StoreCharacterIndices(text, ptr, displayedCharacters);

        ptr += displayedCharacters * IndexSize;
        StoreColor(color, ptr);

        dataVBO.Unmap();

        vao = new VAO();
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

        vao.EnableVertexAttributeArray(TextBlockShaderProgram.AttributePositions.Color);
        vao.BindAttributeToPoint(TextBlockShaderProgram.AttributePositions.Color, 2);
        vao.SetBindingPointToBuffer(2, dataVBO.Id, displayedCharacters * (PosSize + IndexSize));
        vao.SetAttributeFormat(TextBlockShaderProgram.AttributePositions.Color, 3, VertexAttribType.Float);

        shader = TextBlockShaderProgram.GetInstance(fontAtlas);
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

    private static unsafe void StoreColor(Color4 color, IntPtr colorPtr) {
        ref var endVec = ref Unsafe.AsRef<Vector3>(colorPtr.ToPointer());
        endVec = new Vector3(color.R, color.G, color.B);
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
