using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.templates;
using XcyUI.theme;
using XcyUI.views;

namespace XcyUI.utils
{
    internal class ViewUtils
    {
        public static void LazyDeleteAnimate(XLazy lazy)
        {
            var isLazyColumn = lazy is XLazyColumn;
            var deleteItems = new List<XLazyItem>();
            var datas = lazy.Datas;
            var newIndex = -1;
            foreach (var item in lazy.AnimateItems)
            {
                if (lazy.Templates[item.TempleteIndex].Datas.IndexOf(item.Data) < 0)
                {
                    deleteItems.Add(item);
                    if (newIndex == -1)
                    {
                        newIndex = item.Index;
                        if (item.TempleteIndex > 0)
                        {
                            for (int i = 0; i < item.TempleteIndex; i++)
                            {
                                newIndex += lazy.Templates[i].Datas.Count;
                            }
                        }
                    }
                }
            }
            if (deleteItems.Count > 0 && newIndex != -1)
            {
                var data = datas.ElementAtOrDefault(newIndex);
                var firstAfaterItem = lazy.Items.FirstOrDefault(n => n.Data == data);
                int start = 0, end = 0;
                List<XView> afaterChilds = null;
                var index = 0;
                var isDrop = false;
                if (firstAfaterItem != null)
                {
                    index = lazy.Items.IndexOf(firstAfaterItem);
                    var old = lazy.AnimateItems.FirstOrDefault(n => n.Data.Equals(firstAfaterItem.Data));
                    if (isLazyColumn)
                    {
                        start = old?.View?.Y ?? lazy.RenderRect.Bottom;
                        end = firstAfaterItem.View.Y;
                    }
                    else
                    {
                        start = old?.View?.X ?? lazy.RenderRect.Right;
                        end = firstAfaterItem.View.X;
                    }
                    var isLast = start == end;
                    if (!isLast)
                    {
                        afaterChilds = lazy.Childs.GetRange(index, lazy.Items.Count - index);
                    }
                    else
                    {
                        isDrop = true;
                        afaterChilds = lazy.Childs.GetRange(0, index);
                        if (isLazyColumn)
                        {
                            start = deleteItems.First().View.Y;
                            end = deleteItems.Last().View.RenderRect.Bottom;
                        }
                        else
                        {
                            start = deleteItems.First().View.X;
                            end = deleteItems.Last().View.RenderRect.Right;
                        }
                    }
                }
                else
                {
                    index = lazy.Items.Count - 1;
                    var scollerSize = isLazyColumn ? lazy.Scroller.ScrollerHeight : lazy.Scroller.ScrollerWidth;
                    if (scollerSize < 0)
                    {
                        var old = lazy.AnimateItems.FirstOrDefault(n => n.Data.Equals(lazy.Items.Last().Data));
                        if (isLazyColumn)
                        {
                            start = deleteItems.First().View.Y;
                            end = deleteItems.Last().View.RenderRect.Bottom;
                        }
                        else
                        {
                            start = deleteItems.First().View.X;
                            end = deleteItems.Last().View.RenderRect.Right;
                        }
                        afaterChilds = lazy.Childs.ToList();
                        isDrop = true;
                    }
                    else
                    {
                        afaterChilds = new List<XView>();
                    }
                }

                afaterChilds.ForEach(n =>
                {
                    if (isLazyColumn)
                    {
                        n.Translation(0, -(end - start));
                    }
                    else
                    {
                        n.Translation(-(end - start), 0);
                    }
                    n.DrawCache.IsRefreshCache = true;
                });
                lazy.Childs.InsertRange(index, deleteItems.Select(n => n.View));
                lazy.UpdateDrawViews();
                lazy.RefreshParentCache();
                lazy.Animate.OnCallback = (value,i) =>
                {
                    deleteItems.ForEach(n => n.View.DrawCache.Alpha = 1 - value);
                    var nextPoint = (int)lazy.Animate.Value(value, start, end);
                    if (afaterChilds.Count > 0)
                    {
                        var currentPoint = isLazyColumn? afaterChilds.First().Y: afaterChilds.First().X;
                        if (isDrop)
                        {
                            currentPoint = isLazyColumn? afaterChilds.Last().RenderRect.Bottom: afaterChilds.Last().RenderRect.Right;
                        }
                        afaterChilds.ForEach(n =>
                        {
                            if (isLazyColumn)
                            {
                                n.Translation(0, nextPoint - currentPoint);
                            }
                            else
                            {
                                n.Translation(nextPoint - currentPoint, 0);
                            }
                            n.RefreshParentCache();
                        });
                    }
                };
                lazy.Animate.OnFinished = () =>
                {
                    lazy.Refresh();
                };
                lazy.Animate.Start();
                lazy.Invalidate();
            }
        }

        public static void LazyAddAnimate(XLazy lazy)
        {
            var isLazyColumn = lazy is XLazyColumn;
            var addItems = new List<XLazyItem>();
            var index = -1;
            var isNeedAnimate = false;
            foreach (var item in lazy.Items)
            {
                var isAdd = lazy.AnimateItems.Count(n => n.Data == item.Data) == 0;
                if (isAdd)
                {
                    addItems.Add(item);
                    if (index == -1) index = lazy.Items.IndexOf(item);
                }
                else
                {
                    isNeedAnimate = true;
                }
            }
            if (isNeedAnimate && addItems.Count > 0)
            {
                var oldAfaterViews = lazy.AnimateItems.GetRange(index, lazy.AnimateItems.Count - index).Select(n => n.View);
                var lastAddViewIndex = lazy.Items.IndexOf(addItems.Last()) + 1;
                if (lastAddViewIndex < lazy.Childs.Count)
                {
                    lazy.Childs.RemoveRange(lastAddViewIndex, lazy.Childs.Count - lastAddViewIndex);
                }
                lazy.Childs.AddRange(oldAfaterViews);
                var start = addItems.First().View.Y;
                var end = addItems.Last().View.RenderRect.Bottom;
                if (!isLazyColumn)
                {
                    start = addItems.First().View.X;
                    end = addItems.Last().View.RenderRect.Right;
                }
                addItems.ForEach(n =>
                {
                    n.View.LayoutParams.ZIndex = -1;
                    if (isLazyColumn)
                    {
                        n.View.Translation(0, -(end - start));
                    }
                    else
                    {
                        n.View.Translation(-(end - start), 0);
                    }
                    n.View.DrawCache.IsRefreshCache = true;
                });
                var height = end - start;
                start -= height;
                end -= height;
                var afaterChilds = lazy.Childs.GetRange(index, lazy.Childs.Count - index);
                lazy.UpdateDrawViews();
                lazy.RefreshParentCache();
                lazy.Animate.OnCallback = (value,i) =>
                {
                    var nextPoint = (int)lazy.Animate.Value(value, start, end);
                    if (afaterChilds.Count > 0)
                    {
                        var currentPoint = isLazyColumn ? afaterChilds.First().Y : afaterChilds.First().X;
                        afaterChilds.ForEach(n =>
                        {
                            if (isLazyColumn)
                            {
                                n.Translation(0, nextPoint - currentPoint);
                            }
                            else
                            {
                                n.Translation(nextPoint - currentPoint,0);
                            }
                            n.RefreshParentCache();
                        });
                    }
                };
                lazy.Animate.OnFinished = () =>
                {
                    lazy.Refresh();
                };
                lazy.Animate.Start();
                lazy.Invalidate();
            }
        }
    }
}
