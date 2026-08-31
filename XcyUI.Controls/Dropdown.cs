using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XModify MultiDropdown(
            List<(object, string)> items,
            XState<List<object>> selectItemsState = null,
            Action<(object, string)> onSelected = null,
            Func<List<(object, string)>> getItems = null,
            bool isShowSearchInput = false)
        {
            var selectItems = selectItemsState.Value ?? new List<object>();
            var selectValue = items.Where(n => selectItems.Contains(n.Item1)).Select(n => n.Item2);
            var selectStr = string.Join(",", selectValue);
            var selectState = StateValueOf(selectStr, true);
            
            var popupVisible = StateValueOf(false);
            return Row(() =>
            {
                Text().Weight(1).SingleLine()
                .Bind(selectState, select => select)
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
            .Padding(XTheme.Size.Space16, XTheme.Size.Space12)
            .Width(200)
            .Space(10)
            .MuiltiDropDown(popupVisible, items, selectItemsState, selectState, onSelected, getItems, isShowSearchInput: isShowSearchInput);
        }

        private static XModify CheckedStyle(this XModify builder)
        {
            return builder.Padding(10).Width(FILL).HoverBackgroundColor(XTheme.Color.LightFill);
        }

        public static XModify MuiltiDropDown(
            this XModify builder,
            XState<bool> visibleState,
            List<(object, string)> items,
            XState<List<object>> selectItemsState = null,
            XState<string> selectState = null,
            Action<(object, string)> onSelected = null,
            Func<List<(object,string)>> getItems = null,
            bool isShowSearchInput = false,
            bool defaultEffect = true)
        {
            if (items == null) return builder;
            return builder.Popover(visibleState, content: () =>
             {
                 
                 var itemsState = StateValueOf(getItems?.Invoke()??items, true);
                 var maxItem = items.OrderByDescending(n => n.Item2).First().Item2;
                 var width = RenderImp.MeasureText(maxItem, new XFont()).Width;
                 selectItemsState.Join(itemsState);
                 Column(selectItemsState, _selectItems =>
                 {
                     var selectItems = _selectItems ?? new List<object>();
                     if (isShowSearchInput)
                     {
                         Input().PrimaryInput().Width(FILL).Margin(10).KeyPress((b, info) =>
                         {
                             itemsState.Value = items.Where(n => n.Item2.ToLower().Contains(b.Content().ToLower())).ToList();

                         }); ;
                     }
                     Box(() =>
                     {
                         if(selectItems.Count > 0 && selectItems.Count != items.Count)
                         {
                             Row(() =>
                             {
                                 Icon(SvgRes.SemiSelect)
                                 .Size(20)
                                 .Color(XTheme.Color.White)
                                 .Radius(4)
                                 .Padding(4)
                                 .Background(XTheme.Color.Primary);
                                 Text("全选");
                             })
                             .Space(10).Width(FILL)
                             .HoverCursor(XCursorType.Hand)
                             .Click(()=>
                             {
                                 selectItemsState.Value = items.Select(n => n.Item1).ToList();
                                 var value = items.Select(n => n.Item2);
                                 var select = string.Join(",", value);
                                 selectState?.Also(n=>n.Value = select);
                                 onSelected?.Invoke(items.First());
                             }, false);
                         }
                         else
                         {
                             Checkbox(selectItems.Count > 0, "全选", onChecked: isChecked =>
                             {
                                 if (isChecked)
                                 {
                                     selectItemsState.Value = items.Select(n => n.Item1).ToList();
                                     var value = items.Select(n => n.Item2);
                                     var select = string.Join(",", value);
                                     selectState?.Also(n=>n.Value = select);
                                     onSelected?.Invoke(items.First());
                                 }
                                 else
                                 {
                                     selectItemsState.Value = new List<object>();
                                     selectState?.Also(n => n.Value = "");
                                     onSelected?.Invoke(default);
                                 }
                             }).Width(FILL);
                         }
                     })
                     .Size(FILL, WRAP)
                     .ContentAlignment(XAlignment.LeftCenter)
                     .CheckedStyle();
                     
                     foreach (var item in itemsState.Value)
                     {
                         var isSelected = selectItems.Contains(item.Item1);
                         Checkbox(isSelected, item.Item2, isChecked =>
                         {
                             var selectItem = (item.Item1, item.Item2);
                             
                             var list = selectItemsState?.Value?.ToList() ?? new List<object>();
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
                             if (selectState != null)
                             {
                                 selectState.Value = select;
                             }
                             if (selectItemsState != null)
                             {
                                 selectItemsState.Value = list;
                             }
                             onSelected?.Invoke(selectItem);
                         }).CheckedStyle();
                     }
                 })
                 .Radius(XTheme.Radius.Low)
                 .Scrollable()
                 .HorizontalAlignment(XHorizontalAlignment.Left)
                 .Size(WRAP)
                 .MinWidth(200)
                 .MaxWidth(800)
                 .MaxHeight(600)
                 .Width(width);
             }, defaultEffect: defaultEffect);
        }

        public static XModify MultiDropdown(
            List<(object, string)> items,
            List<object> selectItems = null,
            Action<(object, string)> onSelected = null,
            Func<List<(object,string)>> getItems = null,
            bool isShowSearchInput = false)
        {
            var selectItemsState = StateValueOf(selectItems, true);
            return MultiDropdown(items, selectItemsState, onSelected,getItems, isShowSearchInput);
        }

        public static XModify Dropdown(List<(object, string)> items, XState<object> selectItemState = null, Action<(object, string)> onSelected = null)
        {
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
            .Padding(XTheme.Size.Space16, XTheme.Size.Space12)
            .Width(200)
            .Space(10)
            .Popover(popupVisible, content: () =>
            {
                Column(() =>
                {
                    foreach (var item in items)
                    {
                        Row(() =>
                        {
                            Icon(SvgRes.Select).IconSize(20)
                            .Color(XTheme.Color.Primary)
                            .InVisible(item.Item1.Equals(selectItemState?.Value));
                            Text(item.Item2);
                        })
                        .Width(FILL).Space(10).Padding(10)
                        .InterceptEvent(XEventType.Down)
                        .InterceptEvent(XEventType.Click)
                        .Click(()=>
                        {
                            var selectItem = (item.Item1, item.Item2);
                            onSelected?.Invoke(selectItem);
                            selectState.Value = item.Item2;
                            selectItemState.Value = item.Item1;
                            popupVisible.Value = false;
                        });
                    }
                })
                .Scrollable()
                .HorizontalAlignment(XHorizontalAlignment.Left)
                .Radius(XTheme.Radius.Low).Size(FILL, WRAP).MaxHeight(500);
            }, isSameWidth: true);
        }

        public static XModify Dropdown(List<(object, string)> items, object selectItem = null, Action<(object, string)> onSelected = null)
        {
            var selectItemState = StateValueOf(selectItem, true);
            return Dropdown(items, selectItemState, onSelected);
        }
     }
}
