
using Jigsawer.GLBuffers;
using Jigsawer.GLObjects;
using Jigsawer.Helpers;
using Jigsawer.Shaders.Programs;
using static Jigsawer.Shaders.Programs.UpDownArrowsShaderProgram;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Models;

public readonly record struct UpDownArrowInfo(Box2 Box, bool IsDownArrow);

public sealed class UpDownArrowsModel : IRenderableModel {
    private const int BoxSize = sizeof(float) * 4,
        IsDownArrowSize = sizeof(int),
        ColorSize = sizeof(byte) * 4;

    private readonly Box2[] arrowBoxes;
    private readonly VAO vao = new();
    private readonly VBO dataVBO;
    private readonly UpDownArrowsShaderProgram shader = new();

    public Span<Box2> ArrowBoxes => arrowBoxes;

    public UpDownArrowsModel(Color4 arrowColor, params UpDownArrowInfo[] arrowInfos) {
        arrowBoxes = arrowInfos.Select(inf => inf.Box).ToArray();

        dataVBO = new VBO(arrowInfos.Length * (BoxSize + IsDownArrowSize) + ColorSize);
        FillVBO(arrowInfos, arrowColor);

        SetupVAO();
    }

    private void FillVBO(ReadOnlySpan<UpDownArrowInfo> infos, Color4 arrowColor) {
        IntPtr ptr = dataVBO.Map();

        FillBoxes(ptr, arrowBoxes);

        ptr += infos.Length * BoxSize;

        FillIsDownArrows(ptr, infos);

        ptr += infos.Length * IsDownArrowSize;

        FillColor(ptr, arrowColor);

        dataVBO.Unmap();
    }

    private static void FillBoxes(IntPtr ptr, ReadOnlySpan<Box2> boxes) {
        var span = ptr.ToSpan<Box2>(boxes.Length);

        boxes.CopyTo(span);
    }

    private static void FillIsDownArrows(IntPtr ptr, ReadOnlySpan<UpDownArrowInfo> infos) {
        var span = ptr.ToSpan<int>(infos.Length);

        for (int i = 0; i < span.Length; ++i) {
            span[i] = infos[i].IsDownArrow ? 1 : 0;
        }
    }

    private static void FillColor(IntPtr ptr, Color4 color) {
        ref var col = ref ptr.ToReference<int>();

        col = color.ToInt();
    }

    private void SetupVAO() {
        int offset = 0;

        vao.SetBindingPointToBuffer(0, dataVBO.Id, offset, BoxSize);
        vao.SetBindingPointDivisor(0, 1);

        vao.EnableVertexAttributeArray(AttributePositions.Rectangle);
        vao.BindAttributeToPoint(AttributePositions.Rectangle, 0);
        vao.SetAttributeFormat(AttributePositions.Rectangle, 4, VertexAttribType.Float);

        offset += arrowBoxes.Length * BoxSize;

        vao.SetBindingPointToBuffer(1, dataVBO.Id, offset, IsDownArrowSize);
        vao.SetBindingPointDivisor(1, 1);

        vao.EnableVertexAttributeArray(AttributePositions.IsDownArrow);
        vao.BindAttributeToPoint(AttributePositions.IsDownArrow, 1);
        vao.SetIntegerAttributeFormat(AttributePositions.IsDownArrow, 1, VertexAttribIntegerType.Int);

        offset += arrowBoxes.Length * IsDownArrowSize;

        vao.SetBindingPointToBuffer(2, dataVBO.Id, offset, 0);

        vao.EnableVertexAttributeArray(AttributePositions.Color);
        vao.BindAttributeToPoint(AttributePositions.Color, 2);
        vao.SetAttributeFormat(AttributePositions.Color, 4,
            VertexAttribType.UnsignedByte, true);
    }

    public void UpdateBoxes() {
        IntPtr ptr = dataVBO.MapRange(0, arrowBoxes.Length * BoxSize, true);

        FillBoxes(ptr, arrowBoxes);

        dataVBO.Unmap();
    }

    public void Render() {
        vao.Bind();
        shader.Use();

        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 3, arrowBoxes.Length);
    }

    public void Delete() {
        vao.Delete();
        dataVBO.Delete();
        shader.Delete();
    }
}
