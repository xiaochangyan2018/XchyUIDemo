using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public class TreeColumnCell<T> : ColumnCell<T>
    {
        public bool IsAyncLoad { get; set; }
        public Func<T, int> GetLevel { get; set; }
        public Func<T, bool> HasChildren { get; set; }
        public Func<T, bool> IsExpaned { get; set; }
        public Action<T, bool> Expaned { get; set; }
        public Func<T, List<T>> GetChildren { get; set; }
        public Func<T, List<T>> LoadChildren { get; set; }
        public Action<T, bool> SetExpand { get; set; }
        public TreeColumnCell(string name, int width, Func<T, object> valueFun) : base(name, width, valueFun)
        {
        }
    }
    public static partial class Controls
    {
        internal static XModify TreeCell<T>(XState<List<T>> state, XState<bool> notifyGridState, ColumnCell<T> cell, T data, bool isGridBorder)
        {
            var treeCell = (TreeColumnCell<T>)cell;
            return Row(() =>
            {
                int level = treeCell.GetLevel?.Invoke(data) ?? 0;
                bool isExpand = treeCell.IsExpaned?.Invoke(data) ?? false;
                bool hasChildren = treeCell.HasChildren?.Invoke(data) ?? false;
                Spacer().Width(level * 20).Background(XColors.Red);
                var arrowIcon = isExpand ? SvgRes.ArrowDown : SvgRes.ArrowRight;
                var isAyncState = StateValueOf(false, keyPrefix: data.GetHashCode().ToString());
                Box(isAyncState, isAync =>
                {
                    if (isAync)
                    {
                        ColorLoading(XTheme.Color.PrimaryText, 24, 2).Padding(5);
                    }
                    else
                    {
                        Icon(arrowIcon).Size(24).HoverCursor(XCursorType.Hand)
                        .InVisible(hasChildren)
                        .Click(() =>
                        {
                            if (treeCell.IsAyncLoad && !(treeCell.GetChildren?.Invoke(data)!=null))
                            {
                                isAyncState.Value = true;
                                XTask.Run(() =>
                                {
                                    Expand(state, treeCell, data);
                                });
                            }
                            else
                            {
                                Expand(state, treeCell, data);
                            }
                        }, false);
                    }

                }).Size(WRAP);

                Text(treeCell.ValueFun(data).ToString()).SingleLine();
            }).CellStyle(cell).Space(5).Also(n =>
            {
                if (isGridBorder)
                {
                    n.RightBorder();
                }
            });
        }
        private static void Expand<T>(XState<List<T>> state, TreeColumnCell<T> treeCell, T data)
        {
            bool isExpand = treeCell.IsExpaned?.Invoke(data) ?? false;
            if (treeCell.Expaned != null)
            {
                treeCell.Expaned?.Invoke(data, !isExpand);
            }
            else
            {
                var list = state.Value;
                isExpand = treeCell.IsExpaned?.Invoke(data) ?? false;
                if (isExpand)
                {
                    treeCell.SetExpand?.Invoke(data, false);
                    RemoveItem(list, treeCell, data);
                }
                else
                {
                    var index = list.IndexOf(data);
                    treeCell.SetExpand?.Invoke(data, true);
                    if (treeCell.GetChildren?.Invoke(data) != null) {
                        AddItem(list, treeCell, data);
                    }
                    else
                    {
                        var childs = treeCell.LoadChildren?.Invoke(data) ?? new List<T>();
                        for (int i = 0; i < childs.Count; i++)
                        {
                            list.Insert(index + 1 + i, childs[i]);
                        }
                    }
                }
                state.Value = list.ToList();
            }
        }
        private static void RemoveItem<T>(List<T> list, TreeColumnCell<T> treeCell, T data)
        {
            var childs = treeCell.GetChildren?.Invoke(data);
            childs?.ForEach(n =>
            {
                list.Remove(n);
                RemoveItem(list, treeCell, n);
            });
        }

        private static void AddItem<T>(List<T> list, TreeColumnCell<T> treeCell, T data)
        {
            var childs = treeCell.GetChildren?.Invoke(data);
            var index = list.IndexOf(data);
            if(childs != null && (treeCell.IsExpaned?.Invoke(data)??false))
            {
                for (int i = 0; i < childs.Count; i++)
                {
                    list.Insert(index + 1 + i, childs[i]);
                    AddItem(list, treeCell, childs[i]);
                }
            }
        }
    }
}