using System;
using System.Collections.Generic;
using System.Linq;
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
        public static XModify Checkbox(XState<bool> selectState, string text = default, Action<bool> onChecked = default)
        {
            var isHoverState = StateValueOf(false);
            return Row(selectState, select =>
            {
                Icon(SvgRes.Check)
                .Size(20)
                .Color(XTheme.Color.White)
                .Radius(4)
                .Also(n =>
                {
                    if (select)
                    {
                        n.Padding(2).Background(XTheme.Color.Primary);
                    }
                    else
                    {
                        n.Border(XTheme.Color.LightBorder, 2).ResId(0);
                    }
                })
                .Bind(isHoverState, (b, isHover) =>
                {
                    b.Border(isHover ? XTheme.Color.Primary : XTheme.Color.BaseBorder);
                });
                if (text != null)
                {
                    Text(text);
                }
            })
             .Tabindex(0)
            .Space(10)
            .HoverCursor(XCursorType.Hand)
            .Color(XTheme.Color.White)
            .ToggleHover(isHover => isHoverState.Value = isHover)
            .Click(() =>
            {
                selectState.Value = !selectState.Value;
                onChecked?.Invoke(selectState.Value);
            }, false);
        }
        public static XModify Checkbox(bool isSelect = false, string text = default, Action<bool> onChecked = default)
        {
            var selectState = StateValueOf(isSelect, true);
            return Checkbox(selectState, text, onChecked);
        }

        public static XModify CheckboxGroup<T>(List<(string,T)> items, List<T> values, Action<List<T>> onSelect = null)
        {
            return Row(() =>
            {
                var selectItems = StateValueOf(values);
                foreach (var item in items)
                {
                    Checkbox(selectItems.Value.Contains(item.Item2), item.Item1, isChecked =>
                    {
                        var list = selectItems.Value.ToList();
                        if (isChecked)
                        {
                            list.Add(item.Item2);
                        }
                        else
                        {
                            list.Remove(item.Item2);
                        }
                        selectItems.Value = list;
                        onSelect?.Invoke(selectItems.Value);
                    });
                }
            }).Space(10);
        }
    }
}
