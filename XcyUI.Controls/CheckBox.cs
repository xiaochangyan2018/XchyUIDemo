using XcyUI.expansions;
using XcyUI.models;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.models.XFunctions;
using static XcyUI.widgets.XWidget;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XViewBuilder Checkbox(bool isSelect = false, string text = default, XFunction<bool> onChecked = default)
        {
            var selectState = StateValueOf(isSelect, true);
            return Row(selectState, select =>
            {
                Icon(SvgRes.Check)
                .Size(20)
                .Color(xTheme.Colors.White)
                .Radius(xTheme.Radius.Low)
                .Also(n =>
                {
                    if (select)
                    {
                        n.Padding(2).Background(xTheme.Colors.Primary);
                    }
                    else
                    {
                        n.Border(xTheme.Colors.LightBorder, 1).ResId(0);
                    }
                })
                .HoverBorderColor(xTheme.Colors.Primary);
                if (text != null)
                {
                    Text(text);
                }
            })
            .Space(10)
            .HoverCursor(XCursorType.Hand)
            .Color(xTheme.Colors.White)
            .AccessibilityRole(XAccessibilityRole.CheckBox)
            .AccessibilityName(text)
            .AccessibilityChecked(selectState.Value)
            .AccessibilityMergeDescendants()
            .Bind(selectState, (builder, select) => builder.AccessibilityChecked(select))
            .Click(() =>
            {
                selectState.Value = !selectState.Value;
                onChecked?.Invoke(selectState.Value);
            }, false);
        }
    }
}
