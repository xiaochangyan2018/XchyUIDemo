using XcyUI.models;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.models.XFunctions;
using static XcyUI.widgets.XWidget;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XViewBuilder Switch(XState<bool> selectedState, XFunction<bool> onSwitched = null)
        {
            var visibleState = StateValueOf(false);
            var animateState = AnimateFloatOf(visibleState);
            var switchState = StateValueOf(false);
            var firstValue = StateValueOf(selectedState.Value);
            return Box(() =>
            {
                Spacer(30)
                .Alignment(selectedState.Value ? XAlignment.RightCenter : XAlignment.LeftCenter)
                .Background(xTheme.Light.BlankFill)
                .Circle().Shadow(xTheme.Shadows.MinCard)
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
            .AccessibilityRole(XAccessibilityRole.Switch)
            .AccessibilityName("开关")
            .AccessibilityChecked(selectedState.Value)
            .Bind(selectedState, (builder, isSelect) =>
            {
                var backgroundColor = isSelect ? xTheme.Colors.Primary : xTheme.Colors.BaseBorder;
                builder.Background(backgroundColor)
                    .AccessibilityChecked(isSelect);
            })
            .Click(() =>
            {
                selectedState.Value = !selectedState.Value;
                switchState.Value = !switchState.Value;
                visibleState.Value = true;
                onSwitched?.Invoke(selectedState.Value);
            });
        }
        public static XViewBuilder Switch(bool enable, XFunction<bool> onSwitched = null)
        {
            var selectedState = StateValueOf(enable, true);
            return Switch(selectedState, onSwitched);
        }
    }
}
