using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static XcyUI.Controls.Controls;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public enum FilterType
    {
        Selecter,
        String,
        Number,
        Date
    }
    public static class Filter
    {
        public static XModify FilterView<T>(XState<List<T>> state, XState<List<T>> originState, XState<bool> notifyGridState, XState<Dictionary<ColumnCell<T>, Func<T, bool>>> filterFuncs, ColumnCell<T> cell)
        {
            if (cell.FilterType == FilterType.Selecter)
            {
                return SelectFilter(state, originState, notifyGridState, filterFuncs, cell);
            }
            else if (cell.FilterType == FilterType.Date)
            {
                return DateFilter(state, originState, notifyGridState, filterFuncs, cell);
            }
            else if (cell.FilterType == FilterType.Number)
            {
                return NumberFilter(state, originState, notifyGridState, filterFuncs, cell);
            }
            else
            {
                return StringFilter(state, originState, notifyGridState, filterFuncs, cell);
            }
        }
        private static XModify FilterIcon()
        {
            return Icon(SvgRes.ArrowDown).Size(20)
            .Color(XTheme.Color.SecondaryText)
            .HoverCursor(XCursorType.Hand)
            .HoverColor(XTheme.Color.Primary);
        }
        public static XModify SelectFilter<T>(XState<List<T>> state, XState<List<T>> originState, XState<bool> notifyGridState, XState<Dictionary<ColumnCell<T>, Func<T, bool>>> filterFuncs, ColumnCell<T> cell)
        {
            var visibleState = StateValueOf(false);
            var items = state.Value.GroupBy(cell.ValueFun).Select(n => ((object)n.Key, n.Key.ToString())).ToList();
            var selectItemsState = StateValueOf(items.Select(n => n.Item1).ToList(), true);
            var itemsState = StateValueOf(items, true);
            return FilterIcon().MuiltiDropDown(
               visibleState: visibleState,
               selectItemsState: selectItemsState,
               items: items,
               getItems: () =>
               {
                   return originState.Value.GroupBy(cell.ValueFun).Select(n => ((object)n.Key, n.Key.ToString())).ToList();
               },
               onSelected: isSelected =>
               {
                   
                   Func<T, bool> func = n =>
                   {
                       var value = cell.ValueFun(n);
                       return selectItemsState.Value.Contains(value);
                   };
                   filterFuncs.Value[cell] = func;
                   state.Value = originState.Value.Where(n => filterFuncs.Value.Values.All(f => f.Invoke(n))).ToList();
               },
               isShowSearchInput: true,
               defaultEffect: false);
        }

        public static XModify StringFilter<T>(XState<List<T>> state, XState<List<T>> originState, XState<bool> notifyGridState, XState<Dictionary<ColumnCell<T>, Func<T, bool>>> filterFuncs, ColumnCell<T> cell)
        {
            var items = new List<(object, string)>()
                    {
                        (1,"=="),
                        (2,"!="),
                        (3,"正则"),
                    };
            var selectState = StateValueOf(items[0].Item1);
            var stringState = StateValueOf("");
            return FilterIcon().Popover(content: () =>
            {
                Row(() =>
                {
                    Dropdown(items, selectState).Width(100);
                    Input().BindInput(stringState).PrimaryInput().Focus(isSelect: true)
                    .Width(200).Margin(10).OnEnter(text =>
                    {
                        Func<T, bool> func = n =>
                        {
                            var value = (cell.ValueFun(n) as string) ?? "";
                            var selectType = (int)selectState.Value;
                            return selectType == 1 ? value.Contains(text): selectType == 2? !value.Contains(text) : Regex.IsMatch(value, text);
                        };
                        filterFuncs.Value[cell] = func;
                        state.Value = originState.Value.Where(n => filterFuncs.Value.Values.All(f => f.Invoke(n))).ToList();
                    });
                }).Space(10).Padding(10);

            }, defaultEffect: false);
        }

        public static XModify DateFilter<T>(XState<List<T>> state, XState<List<T>> originState, XState<bool> notifyGridState, XState<Dictionary<ColumnCell<T>, Func<T, bool>>> filterFuncs, ColumnCell<T> cell)
        {
            var startDate = StateValueOf(DateTime.Now);
            var endDate = StateValueOf(DateTime.Now);
            return FilterIcon().Popover(content: () =>
            {
                Column(() =>
                {
                    Row(() =>
                    {
                        DateTimePicker(startDate.Value, date=>
                        {
                            startDate.Value = date;
                        });
                        Spacer(1).Height(FILL).RightBorder();
                        Box(startDate, value =>
                        {
                            DateTimePicker(endDate.Value, date =>
                            {
                                endDate.Value = date;
                            }, startTime: value);
                        }).Size(WRAP);
                        
                    }).BottomBorder();
                    Row(() =>
                    {
                        Text("确定").PrimaryButton(() =>
                        {
                            Func<T, bool> func = n =>
                            {
                                var value = Convert.ToDateTime(cell.ValueFun(n));
                                return startDate.Value <= value && value <= endDate.Value;
                            };
                            filterFuncs.Value[cell] = func;
                            state.Value = originState.Value.Where(n => filterFuncs.Value.Values.All(f => f.Invoke(n))).ToList();
                        });
                    })
                    .Width(FILL)
                    .Space(30)
                    .Padding(10)
                    .HorizontalAlignment(XHorizontalAlignment.Right);
                }).Size(WRAP).Space(10);

            }, defaultEffect: false);
        }

        public static XModify NumberFilter<T>(XState<List<T>> state, XState<List<T>> originState, XState<bool> notifyGridState, XState<Dictionary<ColumnCell<T>, Func<T, bool>>> filterFuncs, ColumnCell<T> cell)
        {
            var items = new List<(object, string)>()
                    {
                        (1,">"),
                        (2,">="),
                        (3,"=="),
                        (4,"<"),
                        (5,"<="),
                    };
            var selectState = StateValueOf(items[2].Item1);
            var numberState = StateValueOf(20.ToString());
            return FilterIcon().Popover(content: () =>
            {
                Row(() =>
                {
                    Dropdown(items, selectState).Width(100);
                    NumberInput(numberState).Width(150).Focus(isSelect: true).OnEnter(text =>
                    {
                        int.TryParse(text, out int number);
                        numberState.Value = text;
                        Func<T, bool> func = n =>
                        {
                            var value = (cell.ValueFun(n) as int?) ?? 0;
                            var selectType = (int)selectState.Value;
                            return selectType == 1 ? value > number : selectType == 2 ? value >= number : selectType == 3 ? value == number : selectType == 4 ? value < number : value <= number;
                        };
                        filterFuncs.Value[cell] = func;
                        state.Value = originState.Value.Where(n => filterFuncs.Value.Values.All(f => f.Invoke(n))).ToList();
                    });
                }).Space(10).Padding(10);

            }, defaultEffect: false);
        }
    }
}
