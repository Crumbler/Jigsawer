
using Jigsawer.Models;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Jigsawer.Entities;

public record struct ButtonTextInfo(ButtonInfo BInfo, TextInfo TInfo,
    Action<MouseButtonEventArgs> OnClick);

public sealed class Buttons : IRenderableModel {
    private readonly float[] hoverFactors;
    private readonly ButtonInfo[] buttonInfos;
    private readonly TextBlock[] textBlocks;
    private readonly Action<MouseButtonEventArgs>[] clickActions;
    private readonly ButtonsModel buttonModels;

    public Buttons(params ButtonTextInfo[] infos) {
        hoverFactors = new float[infos.Length];

        buttonInfos = infos.Select(inf => inf.BInfo).ToArray();
        buttonModels = new ButtonsModel(buttonInfos);

        clickActions = infos.Select(inf => inf.OnClick).ToArray();

        textBlocks = infos.Select(inf => new TextBlock(inf.TInfo)).ToArray();
    }

    public void SetButtonEnabledStatus(int index, bool enabled) {
        buttonInfos[index].Enabled = enabled;
    }

    public void TryClick(Vector2 cursorPos, MouseButtonEventArgs eventArgs) {
        ReadOnlySpan<ButtonInfo> buttons = buttonInfos;
        ReadOnlySpan<Action<MouseButtonEventArgs>> actions = clickActions;

        for (int i = 0; i < buttons.Length; ++i) {
            var button = buttons[i];
            bool isHovered = button.Box.ContainsExclusive(cursorPos);
            bool isEnabled = button.Enabled;

            if (isHovered && isEnabled) {
                actions[i](eventArgs);
                break;
            }
        }
    }

    public void Update(Vector2 cursorPos, int elapsedMs) {
        ReadOnlySpan<ButtonInfo> buttons = this.buttonInfos;
        Span<float> hoverFactorsSpan = this.hoverFactors;

        for (int i = 0; i < buttons.Length; ++i) {
            var button = buttons[i];

            bool isHovered = button.Enabled && button.Box.ContainsExclusive(cursorPos);

            float hoverChangeDirection = isHovered ? 1f : -1f;

            float newVal = hoverFactorsSpan[i] + hoverChangeDirection * elapsedMs / 200f;

            hoverFactorsSpan[i] = Math.Clamp(newVal, 0f, 1f);
        }

        this.buttonModels.StoreHoverFactors(hoverFactorsSpan);
    }

    public void Render() {
        buttonModels.Render();

        foreach (var textBlock in textBlocks) {
            textBlock.Render();
        }
    }

    public void Delete() {
        buttonModels.Delete();

        foreach (var textBlock in textBlocks) {
            textBlock.Delete();
        }
    }
}
