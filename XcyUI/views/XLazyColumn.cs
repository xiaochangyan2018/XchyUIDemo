using System;
using System.Diagnostics;
using System.Linq;
using XcyUI.animation;
using XcyUI.expansions;
using XcyUI.templates;
using XcyUI.utils;

namespace XcyUI.views
{
    public class XLazyColumn : XLazy
    {
        protected override void OnMeasure()
        {
            if (Datas.Count == 0)
            {
                Items.Clear();
                Childs.Clear();
                ChildRectHeight = 0;
                ChildRectWidth = 0;
                return;
            }

            if (LayoutParams.IsWrapWidth)
            {
                Width = this.ParentWidth();
            }

            if (LayoutParams.IsWrapHeight)
            {
                Height = 5000;
                if (Height < 0)
                {
                    Height = int.MaxValue;
                }
            }
            this.MeasureMaxOrMin();
            MeasureItems();
            LayoutItems();

            if (LayoutParams.IsWrapWidth)
            {
                Width = ChildRectWidth;
            }

            if (LayoutParams.IsWrapHeight)
            {
                Height = Items.Last().View.RenderRect.Bottom - Items.First().View.Y + Space * 2;
            }

            this.MeasureMaxOrMin();
            isScolled = false;
            //isScollForward = false;
        }

        protected override void LayoutFixedItems()
        {
            var size = Items.Count;
            Items.Clear();
            ChildRectWidth = 0;
            int contentHeight = ContentRect.Height;            
            var template = Templates[0];
            var scollerHeight = Math.Abs(Scroller.ScrollerHeight);
            var index = scollerHeight / (template.Height + Space);
            var top = -scollerHeight % (template.Height + Space);
            for (int i = index; i < template.Datas.Count; i++)
            {
                var item = template.LayoutItem(this, i, top, 0);
                if (item != null)
                {
                    Items.Add(item);
                    var maxWidth = item.View.Childs.Max(a => a.Width);
                    ChildRectWidth = Math.Max(ChildRectWidth, maxWidth);
                }
                var itemHeight = template.ItemHeightAt(i);
                if (top > contentHeight)
                {
                    break;
                }
                top += itemHeight + Space;

            }
        }

        protected override void LayoutDynamicItems()
        {
            var size = Items.Count;
            Items.Clear();
            ChildRectWidth = 0;
            var top = Scroller.ScrollerHeight;
            int contentHeight = ContentRect.Height;
            var isLastIndex = false;
            for (int a = 0; a < Templates.Count; a++)
            {
                var template = Templates[a];
                if (top > contentHeight)
                {
                    break;
                }
                var sumHeight = template.SumHeight;
                var isGroup = true;
                for (int i = 0; i < template.Datas.Count; i++)
                {
                    isLastIndex = a == Templates.Count - 1 && i == template.Datas.Count - 1;
                    if (isGroup)
                    {
                        var groupIndex = i / XLazyTemplate.GroupItemCount;
                        var groupHeight = template.GetGroupHeight(groupIndex);
                        if (top + groupHeight < 0 && (groupIndex < (template.Datas.Count / XLazyTemplate.GroupItemCount) - 2))
                        {
                            i = (groupIndex + 1) * XLazyTemplate.GroupItemCount - 1;
                            top += groupHeight;
                            continue;
                        }
                        else
                        {
                            isGroup = false;
                        }
                    }
                    var item = template.LayoutItem(this, i, top, 0);
                    if (item != null && item.View.Childs.Count > 0)
                    {
                        Items.Add(item);
                        var maxWidth = item.View.Childs.Max(n => n.Width);
                        ChildRectWidth = Math.Max(ChildRectWidth, maxWidth);
                    }

                    if (top > contentHeight)
                    {
                        break;
                    }
                    top += template.ItemHeightAt(i) + Space;
                }
                ChildRectHeight += template.SumHeight - sumHeight;
            }
            var scollerBottom = Math.Abs(Scroller.VerticalScollerBar.RenderRect.Bottom - ContentRect.Bottom);
            LayoutNum++;
            if (contentHeight < ChildRectHeight && Items.Count > 0 && isScollForward && LayoutNum < 100)
            {
                var bottom = Items.Last().View.RenderRect.Bottom;
                if (bottom != contentHeight && (scollerBottom < 2 || (!isScolled && isLastIndex)))
                {
                    Scroller.ScrollerHeight += contentHeight - bottom;
                    LayoutDynamicItems();
                }
            }
        }

        private readonly Stopwatch stopwatch = new Stopwatch();
        protected override void OnScolledItems()
        {
            stopwatch.Restart();
            isScolled = true;
            LayoutItems();
            Scroller.UpdateScollerSize(ContentRect, ChildSize);
            Scroller.Layout(this);
            OnLayout();
            stopwatch.Stop();
        }

        public override void ScolledChilds(int x, int y)
        {
            // 垂直滑动
            if (y != 0)
            {
                LayoutNum = 0;
                foreach (var item in Items)
                {
                    item.View.Translation(0, y);
                    if (!item.View.RenderRect.Intersect(ContentRect))
                    {
                        var template = Templates[item.TempleteIndex];
                        template.FirstCache.Remove(item.Index);
                        template.SecondCache[item.Index] = item;
                    }
                }
                isScollForward = y < 0;
                RenderImp.PostToRender(OnScolledItems);
            }
            else if (x != 0)
            {
                foreach (var item in DrawViews)
                {
                    var box = (XGroup)item;
                    box.DrawCache.IsRefreshCache = true;
                    foreach (var child in box.DrawViews)
                    {
                        if (!child.LayoutParams.Freeze)
                        {
                            child.Translation(x, 0);
                        }
                    }
                }
            }
        }

        public override void ScrolledToIndex(int templeateIndex, int index, bool isSmooth)
        {
            index = Math.Min(index, Templates[templeateIndex].Datas.Count - 1);
            index = Math.Max(index, 0);
            var oldScollerHeight = Scroller.ScrollerHeight;
            int y = 0;
            for (int t = 0; t <= templeateIndex; t++)
            {
                var template = Templates[t];
                var endIndex = t == templeateIndex ? index : template.Datas.Count;
                var isGroup = true;
                for (int i = 0; i < endIndex; i++)
                {
                    if (isGroup)
                    {
                        var groupIndex = i / XLazyTemplate.GroupItemCount;
                        var groupHeight = template.GetGroupHeight(groupIndex);
                        var nextIndex = (groupIndex + 1) * XLazyTemplate.GroupItemCount - 1;
                        if (nextIndex < endIndex)
                        {
                            i = nextIndex;
                            y += groupHeight;
                            continue;
                        }
                        else
                        {
                            isGroup = false;
                        }
                    }
                    var height = template.ItemHeightAt(i);
                    y += height + Space;
                }
            }
            Scroller.ScrollerHeight = -y;
            Scroller.UpdateScollerSize(ContentRect, ChildSize);
            var newScollerHeight = Scroller.ScrollerHeight;
            Scroller.ScrollerHeight = oldScollerHeight;
            isScolled = true;
            isScollForward = true;
            var size = newScollerHeight - oldScollerHeight;
            if (isSmooth)
            {
                var animate = XAnimation.AnimateFloatOf();
                animate.Duration = 300;
                var start = Scroller.ScrollerHeight;
                animate.OnCallback = (value, i) =>
                {
                    OnScolled(true, (int)(start + size * value) - Scroller.ScrollerHeight);
                };
                animate.Start();
            }
            else
            {
                OnScolled(true, size);
            }

        }

        protected override void DeleteAnimate()
        {
            ViewUtils.LazyDeleteAnimate(this);
        }

        protected override void AddAnimate()
        {
            ViewUtils.LazyAddAnimate(this);
        }
    }
}