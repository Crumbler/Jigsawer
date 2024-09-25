
using Jigsawer.GLBuffers;
using Jigsawer.GLObjects;
using Jigsawer.Helpers;
using Jigsawer.Shaders.Programs;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Models;

public record struct PanelInfo(Box2 Rect,
    Color4 Color);

public class PanelsModel : IRenderableModel {
    private const int BytesForRect = sizeof(float) * 4,
        BytesForColor = sizeof(byte) * 4;
    private const int BytesForRectAndColor = BytesForRect + BytesForColor;

    private readonly VAO vao;
    private readonly VBO dataVBO;
    private readonly PanelsShaderProgram shader;
    private readonly PanelInfo[] panels;

    public PanelsModel(int sharedInfoUboBindgPoint, params PanelInfo[] panels) {
        this.panels = panels;

        shader = new PanelsShaderProgram(sharedInfoUboBindgPoint);

        dataVBO = new VBO(panels.Length * BytesForRectAndColor);
        FillVBO();

        vao = new VAO();
        SetupVAO();
    }

    private record struct RectAndColor(Box2 Rect, int Color);

    private void FillVBO() {
        IntPtr ptr = dataVBO.Map();

        ReadOnlySpan<PanelInfo> panels = this.panels;
        var span = ptr.ToSpan<RectAndColor>(panels.Length);
        
        for (int i = 0; i < panels.Length; ++i) {
            var panel = panels[i];
            span[i] = new RectAndColor(panel.Rect, panel.Color.ToInt());
        }

        dataVBO.Unmap();
    }

    public void SetPanelRect(int index, Box2 rect) {
        ref var panelRect = ref dataVBO.MapRange(BytesForRectAndColor * index, BytesForRect, true)
            .ToReference<Box2>();

        panelRect = rect;

        dataVBO.Unmap();
    }

    private void SetupVAO() {
        vao.SetBindingPointToBuffer(0, dataVBO.Id, stride: BytesForRectAndColor);
        vao.SetBindingPointDivisor(0, 1);

        vao.EnableVertexAttributeArray(PanelsShaderProgram.AttributePositions.Rectangle);
        vao.BindAttributeToPoint(PanelsShaderProgram.AttributePositions.Rectangle, 0);
        vao.SetAttributeFormat(PanelsShaderProgram.AttributePositions.Rectangle,
            4, VertexAttribType.Float);

        vao.EnableVertexAttributeArray(PanelsShaderProgram.AttributePositions.Color);
        vao.BindAttributeToPoint(PanelsShaderProgram.AttributePositions.Color, 0);
        vao.SetAttributeFormat(PanelsShaderProgram.AttributePositions.Color,
            4, VertexAttribType.UnsignedByte, true, sizeof(float) * 4);
    }

    public void Delete() {
        shader.Delete();
        vao.Delete();
        dataVBO.Delete();
    }

    public void Render() {
        shader.Use();
        vao.Bind();

        GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, panels.Length);
    }
}
