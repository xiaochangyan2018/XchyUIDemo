using System;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XModify Switch(XState<bool> selectedState, Action<bool> onSwitched = null)
        {
            var visibleState = StateValueOf(false);
            var animateState = AnimateFloatOf(visibleState);
            var switchState = StateValueOf(false);
            var firstValue = StateValueOf(selectedState.Value);
            return Box(() =>
            {
                Spacer(30)
                .Alignment(selectedState.Value ? XAlignment.RightCenter : XAlignment.LeftCenter)
                .Background(XTheme.Light.BlankFill)
                .Circle().Shadow(XTheme.Shadow.MinCard)
                .Bind(animateState, (builder, value) =>
                {
                    if (visibleState.Value)
                    {
                        var pWidth = builder.View.Parent.ContentRect.Width;
                        var tWidth = pWidth - builder.View.Width;
                        if (firstValue.Value)
                        {
                            tWidth = -tWidth;
                        }
                        builder.Translate(tWidth * (switchState.Value ? value : (1 - value)));
                    }
                });
            })
            .Size(66, 33).Padding(horizontal: 2).Radius(16)
            .Bind(selectedState, (builder, isSelect) =>
            {
                var backgroundColor = isSelect ? XTheme.Color.Primary : XTheme.Color.BaseBorder;
                builder.Background(backgroundColor);
            })
            .Click(() =>
            {
                selectedState.Value = !selectedState.Value;
                switchState.Value = !switchState.Value;
                visibleState.Value = true;
                onSwitched?.Invoke(selectedState.Value);
            });
        }
        public static XModify Switch(bool enable, Action<bool> onSwitched = null)
        {
            var selectedState = StateValueOf(enable, true);
            return Switch(selectedState, onSwitched);
        }
    }
}
