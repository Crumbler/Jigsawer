
using Jigsawer.GLBuffers;
using Jigsawer.GLObjects;
using Jigsawer.Helpers;
using Jigsawer.Shaders.Programs;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Models;

public record struct ButtonInfo(Box2 Box,
    Color4 Color,
    Color4 HoverColor,
    bool Enabled = true);

public sealed class ButtonsModel : IRenderableModel {
    // 4 floats in box, 4 bytes per color + 1 float for hoverFactor
    private const int BytesForBoxAndColors = sizeof(float) * 4 + sizeof(byte) * 4 * 2;
    private const int BytesForHoverFactor = sizeof(float);
    private const int BytesPerButton = BytesForBoxAndColors + BytesForHoverFactor;

    private readonly VAO vao;
    private readonly VBO dataVBO;
    private readonly ButtonsShaderProgram shader;
    private readonly int buttonCount;

    public ButtonsModel(params ButtonInfo[] buttons) {
        buttonCount = buttons.Length;

        shader = new ButtonsShaderProgram();

        dataVBO = new VBO(buttonCount * BytesPerButton);
        FillVBO(buttons);

        vao = new VAO();
        SetupVAO();
    }

    private void FillVBO(ReadOnlySpan<ButtonInfo> buttons) {
        IntPtr ptr = dataVBO.Map();

        FillButtonBoxesAndColors(ptr, buttons);

        ptr += buttonCount * BytesForBoxAndColors;

        FillButtonHoverFactors(ptr);

        dataVBO.Unmap();
    }

    private record struct BoxAndColors(Box2 Box, int Color, int HoverColor);

    private static void FillButtonBoxesAndColors(IntPtr ptr, ReadOnlySpan<ButtonInfo> buttons) {
        var span = ptr.ToSpan<BoxAndColors>(buttons.Length);

        for (int i = 0; i < span.Length; ++i) {
            var button = buttons[i];
            span[i] = new(button.Box, button.Color.ToInt(), button.HoverColor.ToInt());
        }
    }

    private void FillButtonHoverFactors(IntPtr ptr) {
        var span = ptr.ToSpan<float>(buttonCount);
        span.Clear();
    }

    private void SetupVAO() {
        vao.SetBindingPointToBuffer(0, dataVBO.Id, stride: BytesForBoxAndColors);
        vao.SetBindingPointDivisor(0, 1);

        vao.EnableVertexAttributeArray(ButtonsShaderProgram.AttributePositions.Rectangle);
        vao.BindAttributeToPoint(ButtonsShaderProgram.AttributePositions.Rectangle, 0);
        vao.SetAttributeFormat(ButtonsShaderProgram.AttributePositions.Rectangle,
            4, VertexAttribType.Float);

        vao.EnableVertexAttributeArray(ButtonsShaderProgram.AttributePositions.Color);
        vao.BindAttributeToPoint(ButtonsShaderProgram.AttributePositions.Color, 0);
        vao.SetAttributeFormat(ButtonsShaderProgram.AttributePositions.Color,
            4, VertexAttribType.UnsignedByte, true, sizeof(float) * 4);

        vao.EnableVertexAttributeArray(ButtonsShaderProgram.AttributePositions.HoverColor);
        vao.BindAttributeToPoint(ButtonsShaderProgram.AttributePositions.HoverColor, 0);
        vao.SetAttributeFormat(ButtonsShaderProgram.AttributePositions.HoverColor,
            4, VertexAttribType.UnsignedByte, true, sizeof(float) * 4 + sizeof(byte) * 4);

        vao.SetBindingPointToBuffer(1, dataVBO.Id, buttonCount * BytesForBoxAndColors,
            BytesForHoverFactor);
        vao.SetBindingPointDivisor(1, 1);

        vao.EnableVertexAttributeArray(ButtonsShaderProgram.AttributePositions.HoverFactor);
        vao.BindAttributeToPoint(ButtonsShaderProgram.AttributePositions.HoverFactor, 1);
        vao.SetAttributeFormat(ButtonsShaderProgram.AttributePositions.HoverFactor,
            1, VertexAttribType.Float);
    }
    
    public void StoreHoverFactors(ReadOnlySpan<float> hoverFactors) {
        IntPtr ptr = dataVBO.MapRange(buttonCount * BytesForBoxAndColors,
            buttonCount * BytesForHoverFactor, true);

        var span = ptr.ToSpan<float>(hoverFactors.Length);

        hoverFactors.CopyTo(span);

        dataVBO.Unmap();
    }

    public void Render() {
        vao.Bind();
        shader.Use();

        GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, buttonCount);
    }

    public void Delete() {
        vao.Delete();
        dataVBO.Delete();
        shader.Delete();
    }
}
