using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.models;
using XcyUI.views;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.models.XFunctions;
using static XcyUI.widgets.XWidget;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XViewBuilder MultiDropdown(List<(object, string)> items, XState<List<object>> selectItemsState = null, XFunction<(object, string)> onSelected = null, int popCardWidth = 200)
        {
            selectItemsState = selectItemsState ?? StateValueOf(new List<object>(), true);
            var selectItems = selectItemsState.Value ?? new List<object>();
            var selectValue = items.Where(n => selectItems.Contains(n.Item1)).Select(n => n.Item2);
            var selectStr = string.Join(",", selectValue);
            var selectState = StateValueOf(selectStr, true);
            
            var popupVisible = StateValueOf(false);
            return Row(() =>
            {
                Text().Weight(1).SingleLine()
                .Bind(selectState,select=> select)
                .LayoutEnd(b=>
                {
                    var text = b.AsView<XText>();
                    b.Tooltip(text.IsContentOver ? selectState.Value : "");
                });
                var isAnimating = StateValueOf(false);
                var aniateValue = AnimateFloatOf(isAnimating);
                Icon(SvgRes.ArrowDown).Size(20)
                .Bind(popupVisible, (b, visible) => isAnimating.Value = true)
                .Bind(aniateValue,(b,value)=>
                {
                    if (isAnimating.Value)
                    {
                        b.Rotate(180 * (popupVisible.Value ? value : (1-value)));
                    }
                });
            })
            .PrimaryInput()
            .Width(200)
            .Space(10)
            .AccessibilityRole(XAccessibilityRole.ComboBox)
            .AccessibilityName("多选下拉")
            .AccessibilityValue(selectState.Value)
            .AccessibilityExpanded(popupVisible.Value)
            .AccessibilityMergeDescendants()
            .Bind(selectState, (builder, select) => builder.AccessibilityValue(select))
            .Bind(popupVisible, (builder, visible) => builder.AccessibilityExpanded(visible))
            .Popover(popupVisible, () =>
            {
                Column(() =>
                {
                    var index = 1;
                    foreach (var item in items)
                    {
                        var isSelected = (selectItemsState.Value ?? new List<object>()).Contains(item.Item1);
                        Checkbox(isSelected, item.Item2, isChecked =>
                        {
                            var selectItem = (item.Item1, item.Item2);
                            onSelected?.Invoke(selectItem);
                            var list = selectItemsState.Value?.ToList() ?? new List<object>();
                            if (isChecked)
                            {
                                list.Add(item.Item1);
                            }
                            else
                            {
                                list.Remove(item.Item1);
                            }
                            var value = items.Where(n => list.Contains(n.Item1)).Select(n => n.Item2);
                            var select = string.Join(",", value);
                            selectState.Value = select;
                            selectItemsState.Value = list;
                        })
                        .AccessibilitySetPosition(index, items.Count)
                        .Margin(10);
                        index++;
                    }
                })
                .AccessibilityRole(XAccessibilityRole.ListBox)
                .Scrollable()
                .HorizontalAlignment(XHorizontalAlignment.Left)
                .Margin(5).Size(WRAP).MinWidth(150).MaxHeight(500).Width(popCardWidth);
            });
        }

        public static XViewBuilder MultiDropdown(List<(object, string)> items,List<object> selectItems = null, XFunction<(object, string)> onSelected = null, int popCardWidth = 200)
        {
            var selectItemsState = StateValueOf(selectItems, true);
            return MultiDropdown(items, selectItemsState, onSelected,popCardWidth);
        }

        public static XViewBuilder Dropdown(List<(object, string)> items, XState<object> selectItemState = null, XFunction<(object, string)> onSelected = null, int popCardWidth = 200)
        {
            selectItemState = selectItemState ?? StateValueOf<object>(null, true);
            var selectValue = items.FirstOrDefault(n => n.Item1.Equals(selectItemState?.Value)).Item2;
            var selectState = StateValueOf(selectValue?.ToString()??"", true);
            var popupVisible = StateValueOf(false);
            return Row(() =>
            {
                Text().Weight(1).SingleLine()
                .Bind(selectState, select=> select)
                .LayoutEnd(b =>
                {
                    var text = b.AsView<XText>();
                    b.Tooltip(text.IsContentOver ? selectState.Value : "");
                });
                var isAnimating = StateValueOf(false);
                var aniateValue = AnimateFloatOf(isAnimating);
                Icon(SvgRes.ArrowDown).Size(20)
                .Bind(popupVisible, (b, visible) => isAnimating.Value = true)
                .Bind(aniateValue, (b, value) =>
                {
                    if (isAnimating.Value)
                    {
                        b.Rotate(180 * (popupVisible.Value ? value : (1 - value)));
                    }
                });
            })
            .PrimaryInput()
            .Width(200)
            .Space(10)
            .AccessibilityRole(XAccessibilityRole.ComboBox)
            .AccessibilityName("下拉选择")
            .AccessibilityValue(selectState.Value)
            .AccessibilityExpanded(popupVisible.Value)
            .AccessibilityMergeDescendants()
            .Bind(selectState, (builder, select) => builder.AccessibilityValue(select))
            .Bind(popupVisible, (builder, visible) => builder.AccessibilityExpanded(visible))
            .Popover(popupVisible, () =>
            {
                Column(() =>
                {
                    var index = 1;
                    foreach (var item in items)
                    {
                        Row(() =>
                        {
                            Icon(SvgRes.Select).IconSize(20)
                            .Color(xTheme.Colors.Primary)
                            .InVisible(item.Item1.Equals(selectItemState?.Value));
                            Text(item.Item2);
                        })
                        .Width(FILL).Space(10).Padding(10)
                        .AccessibilityRole(XAccessibilityRole.ListItem)
                        .AccessibilityName(item.Item2)
                        .AccessibilitySelected(item.Item1.Equals(selectItemState?.Value))
                        .AccessibilitySetPosition(index, items.Count)
                        .Click(()=>
                        {
                            var selectItem = (item.Item1, item.Item2);
                            onSelected?.Invoke(selectItem);
                            selectState.Value = item.Item2;
                            selectItemState.Value = item.Item1;
                            popupVisible.Value = false;
                        });
                        index++;
                    }
                })
                .AccessibilityRole(XAccessibilityRole.ListBox)
                .Scrollable()
                .HorizontalAlignment(XHorizontalAlignment.Left)
                .Margin(5).Size(WRAP).MinWidth(150).MaxHeight(500).Width(popCardWidth);
            });
        }

        public static XViewBuilder Dropdown(List<(object, string)> items, object selectItem = null, XFunction<(object, string)> onSelected = null, int popCardWidth = 200)
        {
            var selectItemState = StateValueOf(selectItem, true);
            return Dropdown(items, selectItemState, onSelected,popCardWidth);
        }
     }
}
