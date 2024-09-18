
using Jigsawer.GLBuffers;
using Jigsawer.GLObjects;
using Jigsawer.Helpers;
using Jigsawer.Shaders.Programs;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Models;

public record struct ButtonInfo(Box2 Box,
    System.Drawing.Color Color,
    System.Drawing.Color HoverColor,
    System.Drawing.Color TextColor,
    float Padding,
    float TextSize,
    string Text,
    Action OnClick);

public sealed class ButtonsModel : IRenderableModel {
    // 4 floats in box, 4 bytes per color + 1 float for hoverFactor
    private const int BytesForBoxAndColors = sizeof(float) * 4 + sizeof(byte) * 4 * 2;
    private const int BytesForHoverFactor = sizeof(float);
    private const int BytesPerButton = BytesForBoxAndColors + BytesForHoverFactor;

    private readonly VAO vao;
    private readonly VBO dataVBO;
    private readonly ButtonsShaderProgram shader;
    private readonly ButtonInfo[] buttons;
    private readonly TextBlock[] textBlocks;
    private readonly float[] hoverFactors;

    public ButtonsModel(ref Matrix3 projMat, params ButtonInfo[] buttons) {
        this.buttons = buttons;
        hoverFactors = new float[buttons.Length];
        textBlocks = new TextBlock[buttons.Length];
        FillTextBlocks(ref projMat);

        shader = new ButtonsShaderProgram();
        shader.SetProjectionMatrix(ref projMat);

        dataVBO = new VBO(buttons.Length * BytesPerButton);
        FillVBO();

        vao = new VAO();
        SetupVAO();
    }

    private void FillTextBlocks(ref Matrix3 projMat) {
        ReadOnlySpan<ButtonInfo> buttons = this.buttons;
        Span<TextBlock> textBlocks = this.textBlocks;

        for (int i = 0; i < textBlocks.Length; ++i) {
            var button = buttons[i];
            textBlocks[i] = new TextBlock(button.Text, button.Box.Min,
                button.TextColor, button.Padding, 
                button.TextSize, ref projMat);
        }
    }

    private void FillVBO() {
        IntPtr ptr = dataVBO.Map();

        FillButtonBoxesAndColors(ptr, buttons);

        ptr += buttons.Length * BytesForBoxAndColors;

        FillButtonHoverFactors(ptr, buttons.Length);

        dataVBO.Unmap();
    }

    private record struct BoxAndColors(Box2 Box, int Color, int HoverColor);

    private static unsafe void FillButtonBoxesAndColors(IntPtr ptr, ReadOnlySpan<ButtonInfo> buttons) {
        var span = new Span<BoxAndColors>(ptr.ToPointer(), buttons.Length);

        for (int i = 0; i < span.Length; ++i) {
            var button = buttons[i];
            span[i] = new(button.Box, button.Color.ToInt(), button.HoverColor.ToInt());
        }
    }

    private static unsafe void FillButtonHoverFactors(IntPtr ptr, int buttonCount) {
        var span = new Span<float>(ptr.ToPointer(), buttonCount);
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

        vao.SetBindingPointToBuffer(1, dataVBO.Id, buttons.Length * BytesForBoxAndColors,
            BytesForHoverFactor);
        vao.SetBindingPointDivisor(1, 1);

        vao.EnableVertexAttributeArray(ButtonsShaderProgram.AttributePositions.HoverFactor);
        vao.BindAttributeToPoint(ButtonsShaderProgram.AttributePositions.HoverFactor, 1);
        vao.SetAttributeFormat(ButtonsShaderProgram.AttributePositions.HoverFactor,
            1, VertexAttribType.Float);
    }

    public void TryClick(Vector2 cursorPos) {
        ReadOnlySpan<ButtonInfo> buttons = this.buttons;

        for (int i = 0; i < buttons.Length; ++i) {
            var button = buttons[i];
            bool isHovered = button.Box.ContainsExclusive(cursorPos);

            if (isHovered) {
                button.OnClick();
                break;
            }
        }
    }

    public void Update(Vector2 cursorPos, int elapsedMs) {
        ReadOnlySpan<ButtonInfo> buttons = this.buttons;
        Span<float> hoverFactors = this.hoverFactors;
        
        for (int i = 0; i < buttons.Length; ++i) {
            bool isHovered = buttons[i].Box.ContainsExclusive(cursorPos);
            
            float hoverChangeDirection = isHovered ? 1f : -1f;

            float newVal = hoverFactors[i] + hoverChangeDirection * elapsedMs / 200f;

            hoverFactors[i] = Math.Clamp(newVal, 0f, 1f);
        }

        StoreHoverFactors();
    }

    private unsafe void StoreHoverFactors() {
        ReadOnlySpan<float> hoverFactors = this.hoverFactors;

        IntPtr ptr = dataVBO.MapRange(buttons.Length * BytesForBoxAndColors,
            buttons.Length * BytesForHoverFactor, true);

        var span = new Span<float>(ptr.ToPointer(), hoverFactors.Length);

        hoverFactors.CopyTo(span);

        dataVBO.Unmap();
    }

    public void UpdateProjectionMatrix(ref Matrix3 mat) {
        shader.SetProjectionMatrix(ref mat);
        Span<TextBlock> textBlocks = this.textBlocks;
        foreach (var textBlock in textBlocks) {
            textBlock.UpdateProjectionMatrix(ref mat);
        }
    }

    public void Render() {
        vao.Bind();
        shader.Use();

        GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, buttons.Length);

        ReadOnlySpan<TextBlock> textBlocks = this.textBlocks;
        foreach (var textBlock in textBlocks) {
            textBlock.Render();
        }
    }

    public void Delete() {
        vao.Delete();
        dataVBO.Delete();
        shader.Delete();

        Span<TextBlock> textBlocks = this.textBlocks;
        foreach (var textBlock in textBlocks) {
            textBlock.Delete();
        }
    }
}
