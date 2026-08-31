using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.views;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public enum Fixed
    {
        None,
        Left,
        Right
    }
    public class ColumnCell<T>
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public bool IsSort { get; set; }
        public bool IsFilter { get; set; }
        public XHorizontalAlignment Alignment { get; set; } = XHorizontalAlignment.Left;
        public FilterType FilterType { get; set; }
        public bool IsResize { get; set; }
        public XState<List<T>> SelectItemsState { get; set; }
        public Fixed Fixed { get; set; }
        public Func<T, object> ValueFun { get; set; }
        public Action<ColumnCell<T>> HeaderContent { get; private set; }
        public Action<ColumnCell<T>, T> CellContent { get; set; }
        
        
        public Func<T, bool> IgnoreSelect { get; set; }
        public Action<bool> OnSort { get; set; }
        internal XState<bool> HeaderSelectState { get; set; }
        internal XState<int> ResizeState { get; set; }
        public ColumnCell(string name, int width, Func<T, object> valueFun)
        {
            Name = name;
            Width = width;
            Fixed = Fixed.None;
            ValueFun = valueFun;
            HeaderSelectState = new XState<bool>();
            ResizeState = new XState<int>();
            FilterType = FilterType.String;
        }
    }
    
    public static partial class Controls
    {
        private static XShadow fresszeShadow = new XShadow()
        {
            Dx = 0,
            Blur = 6,
            Color = XColors.Black.Copy(0.2f)
        };
        public static XModify DataGrid<T>(
            XState<List<T>> state, 
            List<ColumnCell<T>> cells,
            bool isGridBorder = false,
            bool isHideHeader = false,
            bool isHideBorder = false,
            bool isAnimate = false,
            Action<XModify,int> modify = null)
        {
            return Column(() =>
            {
                var scolledState = StateValueOf(0);
                var hoverIndexState = StateValueOf(-1);
                var isScolledToRightState = StateValueOf(false);
                var isScolledToLeftState = StateValueOf(false);
                var notifyGridState = StateValueOf(false);
                var originState = StateValueOf(state.Value);
                var selectState = StateValueOf(cells[0].SelectItemsState?.Value);
                var filterFuncs = StateValueOf(new Dictionary<ColumnCell<T>, Func<T, bool>>());
                if (!isHideHeader)
                {
                    Box(() =>
                    {
                        DataRow(state,originState, notifyGridState, filterFuncs, cells, default, isGridBorder)
                        .Size(FILL, WRAP)
                        .Scrollable(isVertical: false, enableScollerBar: false,enableWheel: false)
                        .Clip(false, false)
                        .LayoutEnd(builder =>
                        {
                            isScolledToRightState.Value = builder.IsScrolledToRight();
                            isScolledToLeftState.Value = builder.IsScrolledToLeft();
                        })
                        .Bind(scolledState, (builder, size) =>
                        {
                            if (size != 0)
                            {
                                builder.TranslationChilds(size, 0);
                                scolledState.Value = 0;
                                isScolledToRightState.Value = builder.IsScrolledToRight();
                                isScolledToLeftState.Value = builder.IsScrolledToLeft();
                            }
                        });
                        FixedLeft(state,originState, notifyGridState, filterFuncs, cells, isScolledToLeftState, default,-1, isGridBorder, modify);
                        FixedRight(state,originState, notifyGridState, filterFuncs, cells, isScolledToRightState, default,-1, isGridBorder, modify);
                    })
                    .Size(FILL, WRAP)
                    .MinHeight(60)
                    .Background(XTheme.Color.LighterFill)
                    .ContentAlignment(XAlignment.LeftCenter)
                    .Also(n =>
                    {
                        if(cells.Count(n=>n.Fixed != Fixed.None) > 0)
                        {
                            n.Clip();
                        }
                        if (!isHideHeader)
                        {
                            n.BottomBorder();
                        }
                        modify?.Invoke(n,-1);
                    });
                }

                LazyColumn(state, datas =>
                {
                    LazyItem(datas, isAnimate, (data, index) =>
                    {
                        DataRow(state,originState, notifyGridState, filterFuncs, cells, data, isGridBorder);
                        FixedLeft(state,originState, notifyGridState, filterFuncs, cells, isScolledToLeftState, data, index, isGridBorder, modify);
                        FixedRight(state,originState, notifyGridState, filterFuncs, cells, isScolledToRightState, data,index, isGridBorder, modify);
                    })
                    .OnViewSetting<T>((builder, data, index) =>
                    {
                        builder
                        .Background(XTheme.Color.LighterFill)
                        .HoverBackgroundAllColor(XTheme.Color.LightFill)
                        .Clip()
                        .Also(n =>
                        {
                            if (!isHideHeader)
                            {
                                n.BottomBorder();
                            }
                            modify?.Invoke(n, index);
                        });
                    });
                })
                .Weight(1).Height(WRAP).OnScrolled((builder, eventInfo) =>
                {
                    scolledState.Value = eventInfo.X;
                })
                .Bind(notifyGridState, (b, notify) =>
                {
                    if (notify)
                    {
                        b.View.NotifyLazy();
                        notifyGridState.Value = false;
                    }
                });
            })
            .Also(n=>
            {
                if (isGridBorder)
                {
                    n.DefaultBorder();
                }
            });
        }

        private static void FixedLeft<T>(XState<List<T>> state, XState<List<T>> originState, XState<bool> notifyGridState, XState<Dictionary<ColumnCell<T>, Func<T, bool>>> filterFuncs, List<ColumnCell<T>> cells, XState<bool> isScolledToLeftState, T data, int index, bool isGridBorder, Action<XModify, int> modify)
        {
            var first = cells.First();
            if (first.Fixed == Fixed.Left)
            {
                var m = Row(() =>
                {
                    foreach (var item in cells)
                    {
                        if (item.Fixed != Fixed.Left) break;
                        Cell(state,originState, notifyGridState, filterFuncs, item, data, true, isGridBorder);
                    }
                })
                .Size(WRAP, FILL).Freeze()
                .Background(XTheme.Color.LighterFill)
                .Bind(isScolledToLeftState, (builder, isLeft) =>
                {
                    builder.Shadow(isLeft ? XShadow.Empty : fresszeShadow).CacheShadow(true);
                }).Clip();
                modify?.Invoke(m, index);
            }
        }

        private static void FixedRight<T>(XState<List<T>> state, XState<List<T>> originState, XState<bool> notifyGridState, XState<Dictionary<ColumnCell<T>, Func<T, bool>>> filterFuncs, List<ColumnCell<T>> cells, XState<bool> isScolledToRightState, T data,int index, bool isGridBorder, Action<XModify,int> modify)
        {
            var last = cells.Last();
            if (last.Fixed == Fixed.Right)
            {
                var tempCells = new List<ColumnCell<T>>();
                for (int i = cells.Count - 1; i >= 0; i--)
                {
                    if (cells[i].Fixed != Fixed.Right) break;
                    tempCells.Insert(0, cells[i]);
                }
                var m = Row(() =>
                {
                    foreach (var item in tempCells)
                    {
                        Cell(state,originState, notifyGridState, filterFuncs, item, data, true, isGridBorder);
                    }
                })
                .Size(WRAP, FILL)
                .Freeze()
                .Background(XTheme.Color.LighterFill)
                .Alignment(XAlignment.RightCenter)
                .Bind(isScolledToRightState, (builder, isRight) =>
                {
                    builder.Shadow(isRight ? XShadow.Empty : fresszeShadow).CacheShadow(true);
                    if(isRight && isGridBorder)
                    {
                        builder.LeftBorder();
                    }
                }).Clip();
                modify?.Invoke(m, index);
            }
        }

        private static XModify DataRow<T>(XState<List<T>> state, XState<List<T>> originState, XState<bool> notifyGridState, XState<Dictionary<ColumnCell<T>, Func<T, bool>>> filterFuncs, List<ColumnCell<T>> cells, T? data, bool isGridBorder)
        {
            return Row(() =>
            {
                foreach (var item in cells)
                {
                    Cell(state,originState, notifyGridState, filterFuncs, item, data, false, isGridBorder);
                }
            }).MinHeight(60).Width(cells.Count(n=>n.Width <0)>0?FILL:WRAP);
        }

        internal static XModify Cell<T>(XState<List<T>> state, XState<List<T>> originState, XState<bool> notifyGridState, XState<Dictionary<ColumnCell<T>, Func<T, bool>>> filterFuncs,ColumnCell<T> cell, T data, bool isFixed, bool isGridBorder)
        {
            if (!isFixed && cell.Fixed != Fixed.None)
            {
                return Spacer().CellStyle(cell).Also(n=>
                {
                    if (cell.IsResize)
                    {
                        n.Bind(cell.ResizeState, (b, size) =>
                        {
                            if (size > 0)
                            {
                                n.View.LayoutParams.Width = size;
                            }
                        }, needLayout: true);
                    }
                });
            }
            var isTitle = data == null || data?.Equals(default(T)) == true;
            XModify builder = null;
            if (isTitle)
            {
                builder = Row(()=>
                {
                    if (cell.HeaderContent != null)
                    {
                        cell.HeaderContent.Invoke(cell);
                    }
                    else if (cell.SelectItemsState !=null)
                    {
                        Box(cell.HeaderSelectState, selected =>
                        {
                            var isAllUnSelected = cell.SelectItemsState.Value.Count > 0;
                            if (!selected && isAllUnSelected)
                            {
                                Icon(SvgRes.SemiSelect)
                                .Size(20)
                                .Color(XTheme.Color.White)
                                .Radius(4)
                                .Padding(4)
                                .Background(XTheme.Color.Primary)
                                .HoverCursor(XCursorType.Hand)
                                .Click(()=>
                                {
                                    cell.HeaderSelectState.Value = true;
                                    cell.SelectItemsState.Value = originState.Value.Where(n => !(cell.IgnoreSelect?.Invoke(n) ?? false)).ToList();
                                    notifyGridState.Value = true;
                                }, false);
                            }
                            else
                            {
                                Checkbox(selected, onChecked: isCheck =>
                                {
                                    if (isCheck)
                                    {
                                        cell.SelectItemsState.Value = originState.Value.Where(n => !(cell.IgnoreSelect?.Invoke(n) ?? false)).ToList();
                                    }
                                    else
                                    {
                                        cell.SelectItemsState.Value = new List<T>();
                                    }
                                    notifyGridState.Value = true;
                                    cell.HeaderSelectState.Value = isCheck;
                                });
                            }
                        }).Size(WRAP);
                    }
                    else
                    {
                        TextCell(cell, data);
                    }
                    // 排序
                    if (cell.IsSort)
                    {
                        Spacer(5);
                        var sortState = StateValueOf(0);
                        Box(sortState, sort =>
                        {
                            Icon(SvgRes.DCaret).Size(20).Color(XTheme.Color.SecondaryText);
                            Box(() =>
                            {
                                Icon(SvgRes.DCaret).Size(20)
                                .Margin(top: sort == 1 || sort == 0 ? 0 : -12)
                                .Color(sort != 0 ? XTheme.Color.Primary : XTheme.Color.SecondaryText);
                            })
                            .ContentAlignment(XAlignment.TopCenter)
                            .Size(20, 10)
                            .Alignment(XAlignment.TopCenter)
                            .Clip()
                            .Also(n=>
                            {
                                n.Alignment(sort == 1|| sort == 0?XAlignment.TopCenter: XAlignment.BottomCenter);
                            });
                            
                        })
                        .Size(20).HoverCursor(XCursorType.Hand).Click(() =>
                        {
                            sortState.Value = sortState.Value == 1 ? 2 : 1;
                            if (cell.OnSort != null)
                            {
                                cell.OnSort?.Invoke(sortState.Value == 1);
                            }
                            else
                            {
                                if (sortState.Value == 1)
                                {
                                    state.Value = state.Value.OrderBy(cell.ValueFun).ToList();
                                }
                                else
                                {
                                    state.Value = state.Value.OrderByDescending(cell.ValueFun).ToList();
                                }
                            }
                        }, false);
                    }
                    // 过滤
                    if (cell.IsFilter)
                    {
                        Filter.FilterView(state, originState, notifyGridState,filterFuncs, cell);
                    }
                })
                .CellStyle(cell)        
                //.Background(XTheme.Color.Background)
                .Resize(right: cell.IsResize) // 拖动列宽
                .OnUp((b,info)=>
                {
                    (builder.View.Parent as XGroup)?.Also(n =>
                    {
                        n.Scroller?.Also(a => a.EnableScrolled = true);
                        n.StartLayout();
                    });
                    notifyGridState.Value = true;
                })
                .OnResize(b =>
                {
                    (b.View.Parent as XGroup)?.Also(n => n.Scroller?.Also(a=>
                    {
                        a.EnableScrolled = false;
                    }));
                    cell.ResizeState.Value = b.View.Width;
                });
            }
            else
            {
                if (cell is TreeColumnCell<T>)
                {
                    builder = TreeCell(state, notifyGridState, cell, data, isGridBorder);
                }
                else if (cell.CellContent != null)
                {
                    builder = Box(() =>
                    {
                        cell.CellContent.Invoke(cell, data);
                    }).CellStyle(cell);
                }
                else if (cell.SelectItemsState !=null)
                {
                    var ignoreSelect = cell.IgnoreSelect?.Invoke(data) ?? false;
                    var selected = cell.SelectItemsState.Value.IndexOf(data) >= 0 && !ignoreSelect;
                    builder = Checkbox(selected, onChecked: isChecked =>
                    {
                        if (isChecked)
                        {
                            cell.SelectItemsState.Value.Add(data);
                            int count = originState.Value.Count(n => !(cell.IgnoreSelect?.Invoke(n) ?? false));
                            cell.HeaderSelectState.Value = cell.SelectItemsState.Value.Count == count;
                        }
                        else
                        {
                            cell.SelectItemsState.Value.Remove(data);
                            cell.HeaderSelectState.Value = false;
                        }
                    })
                    .CellStyle(cell)
                    .Disable(cell.IgnoreSelect?.Invoke(data)??false);
                }
                else
                {
                    builder = TextCell(cell, data).CellStyle(cell);
                }
                if (cell.IsResize)
                {
                    builder.Bind(cell.ResizeState, (b, size) =>
                    {
                        if (size > 0)
                        {
                            builder.View.LayoutParams.Width = size;
                        }
                    }, needLayout: true);
                }
            }
            if (isGridBorder)
            {
                builder.RightBorder();
            }
            return builder;
        }

        private static XModify CellStyle<T>(this XModify builder, ColumnCell<T> cell)
        {
            return builder.Weight(cell.Width < 0 ? Math.Abs(cell.Width) : 0)
                    .Padding(horizontal: 20)
                    .MinWidth(50)
                    .HorizontalAlignment(cell.Alignment)
                    .Size(cell.Width, FILL);
        }

        public static XModify TextCell<T>(ColumnCell<T> cell, T data)
        {
            var isTitle = data == null || data?.Equals(default(T)) == true;
            return Text(isTitle ? cell.Name : cell.ValueFun(data)?.ToString()??"")
                .TextAlignment(cell.Alignment == XHorizontalAlignment.Left?XAlignment.LeftCenter:cell.Alignment == XHorizontalAlignment.Center? XAlignment.Center:XAlignment.RightCenter)
                .Also(n =>
                {
                    if (isTitle)
                    {
                        n.FontWeight(XTheme.Weight.Large).Color(XTheme.Color.SecondaryText).SingleLine();
                    }
                });
        }
    }
}
