using System;
using System.Collections.Generic;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XModify RadioGroup<T>(List<(string,T)> items, T value, Action<T> onSelected = null)
        {
            var selectItemState = StateValueOf(value, true);
            return Row(selectItemState, selectItem =>
            {
                foreach (var item in items)
                {
                    RadioBox(item.Item2.Equals(selectItem), item.Item1).Click(() =>
                    {
                        selectItemState.Value = item.Item2;
                        onSelected?.Invoke(item.Item2);
                    },false);
                }
            }).Space(20);
        }
        public static XModify RadioBox(bool select,string text = null)
        {
            var isHoverState = StateValueOf(false);
            return Row(() =>
            {
                Box(() =>
                {
                    Spacer(20, key:1000).Circle().Border(XTheme.Color.BaseBorder, 2).Also(n=>
                    {
                        if (select)
                        {
                            n.Border(XColor.Empty, 0).Background(XTheme.Color.Primary);
                        }
                    })
                    .Bind(isHoverState, (b,isHover)=>
                    {
                        b.Border(isHover?XTheme.Color.Primary:XTheme.Color.BaseBorder);
                    });

                    if (select)
                    {
                        Spacer(8).Circle().Background(XTheme.Color.Background);
                    }
                })
                .Size(WRAP);
                Text(text);
            })
            .Space(10)
            .Tabindex(0)
            .HoverCursor(XCursorType.Hand).ToggleHover(isHover=> isHoverState.Value = isHover);
        }
    }
}
