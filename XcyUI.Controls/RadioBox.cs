using System.Collections.Generic;
using System.Linq;
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
        public static XViewBuilder RadioGroup(List<(string,object)> items, XFunction<(string,object)> onSelected = null)
        {
            var selectItemState = StateValueOf(items.First());
            return Row(selectItemState, selectItem =>
            {
                foreach (var item in items)
                {
                    RadioBox(item.Equals(selectItem), item.Item1).Click(() =>
                    {
                        selectItemState.Value = item;
                        onSelected?.Invoke(item);
                    },false);
                }
            }).Space(20);
        }
        public static XViewBuilder RadioBox(bool select,string text = null)
        {
            return Row(() =>
            {
                Box(() =>
                {
                    Spacer(20).Circle().DefaultBorder().Also(n=>
                    {
                        if (select)
                        {
                            n.Border(XColor.Empty, 0).Background(xTheme.Colors.Primary);
                        }
                    }).HoverBorderColor(xTheme.Colors.Primary);

                    if (select)
                    {
                        Spacer(8).Circle().Background(xTheme.Colors.Background);
                    }
                }).Size(WRAP);
                Text(text);
            }).Space(10)
            .HoverCursor(XCursorType.Hand)
            .AccessibilityRole(XAccessibilityRole.RadioButton)
            .AccessibilityName(text)
            .AccessibilityChecked(select)
            .AccessibilityMergeDescendants();
        }
    }
}
