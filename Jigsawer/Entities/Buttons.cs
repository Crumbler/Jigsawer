
using Jigsawer.Models;

using OpenTK.Mathematics;

namespace Jigsawer.Entities;

public record struct ButtonTextInfo(ButtonInfo BInfo, TextInfo TInfo, Action OnClick);

public sealed class Buttons : IRenderableModel {
    private readonly float[] hoverFactors;
    private readonly ButtonInfo[] buttonInfos;
    private readonly TextBlock[] textBlocks;
    private readonly Action[] clickActions;
    private readonly ButtonsModel buttons;

    public Buttons(params ButtonTextInfo[] infos) {
        hoverFactors = new float[infos.Length];

        buttonInfos = infos.Select(inf => inf.BInfo).ToArray();
        buttons = new ButtonsModel(buttonInfos);

        clickActions = infos.Select(inf => inf.OnClick).ToArray();

        textBlocks = infos.Select(inf => new TextBlock(inf.TInfo)).ToArray();
    }

    public void SetButtonEnabledStatus(int index, bool enabled) {
        buttonInfos[index].Enabled = enabled;
    }

    public void TryClick(Vector2 cursorPos) {
        ReadOnlySpan<ButtonInfo> buttons = buttonInfos;
        ReadOnlySpan<Action> actions = clickActions;

        for (int i = 0; i < buttons.Length; ++i) {
            var button = buttons[i];
            bool isHovered = button.Box.ContainsExclusive(cursorPos);
            bool isEnabled = button.Enabled;

            if (isHovered && isEnabled) {
                actions[i]();
                break;
            }
        }
    }

    public void Update(Vector2 cursorPos, int elapsedMs) {
        ReadOnlySpan<ButtonInfo> buttons = buttonInfos;
        Span<float> hoverFactors = this.hoverFactors;

        for (int i = 0; i < buttons.Length; ++i) {
            var button = buttons[i];

            bool isHovered = button.Enabled && button.Box.ContainsExclusive(cursorPos);

            float hoverChangeDirection = isHovered ? 1f : -1f;

            float newVal = hoverFactors[i] + hoverChangeDirection * elapsedMs / 200f;

            hoverFactors[i] = Math.Clamp(newVal, 0f, 1f);
        }

        this.buttons.StoreHoverFactors(hoverFactors);
    }

    public void Render() {
        buttons.Render();

        ReadOnlySpan<TextBlock> textBlocks = this.textBlocks;
        foreach (var textBlock in textBlocks) {
            textBlock.Render();
        }
    }

    public void Delete() {
        buttons.Delete();

        Span<TextBlock> textBlocks = this.textBlocks;
        foreach (var textBlock in textBlocks) {
            textBlock.Delete();
        }
    }
}
