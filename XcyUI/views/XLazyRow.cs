using System;
using System.Linq;
using XcyUI.animation;
using XcyUI.expansions;
using XcyUI.templates;
using XcyUI.utils;

namespace XcyUI.views
{
    public class XLazyRow : XLazy
    {
        protected override void OnMeasure()
        {
            this.MeasureSize();
            if (Datas.Count == 0) return;
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
                Height = this.ParentHeight();
            }

            this.MeasureMaxOrMin();
            MeasureItems();
            LayoutItems();

            if (LayoutParams.IsWrapWidth)
            {
                Width = Items.Last().View.RenderRect.Right - Items.First().View.X + Space * 2;
            }

            if (LayoutParams.IsWrapHeight)
            {
                Height = ChildRectHeight;
            }

            this.MeasureMaxOrMin();
            isScolled = false;
        }

        protected override void LayoutFixedItems()
        {
            Items.Clear();
            int contentWidth = ContentRect.Width;
            var template = Templates[0];
            var scollerWidth = Math.Abs(Scroller.ScrollerWidth);
            var index = scollerWidth / (template.Width + Space);
            var left = -scollerWidth % (template.Width + Space);
            for (int i = index; i < template.Datas.Count; i++)
            {
                var item = template.LayoutItem(this, i, 0, left);

                if (item != null)
                {
                    Items.Add(item);
                    var maxHeight = item.View.Childs.Max(a => a.Height);
                    ChildRectHeight = Math.Max(ChildRectHeight, maxHeight);
                }
                if (left > contentWidth)
                {
                    break;
                }
                left += template.ItemWidthAt(i) + Space;
            }
        }

        protected override void LayoutDynamicItems()
        {
            var size = Items.Count;
            Items.Clear();
            ChildRectHeight = 0;
            var left = Scroller.ScrollerWidth;
            int contentWidth = ContentRect.Width;
            var isLastIndex = false;
            for (int a = 0; a < Templates.Count; a++)
            {
                var template = Templates[a];
                var sumWidth = template.SumWidth;
                var isGroup = true;
                for (int i = 0; i < template.Datas.Count; i++)
                {
                    isLastIndex = a == Templates.Count - 1 && i == template.Datas.Count - 1;
                    if (isGroup)
                    {
                        var groupIndex = i / XLazyTemplate.GroupItemCount;
                        var groupWidth = template.GetGroupWidth(groupIndex);
                        if (left + groupWidth < 0 && (groupIndex < (template.Datas.Count / XLazyTemplate.GroupItemCount) - 2))
                        {
                            i = (groupIndex + 1) * XLazyTemplate.GroupItemCount - 1;
                            left += groupWidth;
                            continue;
                        }
                        else
                        {
                            isGroup = false;
                        }
                    }
                    var item = template.LayoutItem(this, i, 0, left);

                    if (item != null)
                    {
                        Items.Add(item);
                        var maxHeight = item.View.Childs.Max(n => n.Height);
                        ChildRectHeight = Math.Max(ChildRectHeight, maxHeight);
                    }

                    if (left > contentWidth)
                    {
                        break;
                    }

                    left += template.ItemWidthAt(i) + Space;
                }
                ChildRectWidth += template.SumWidth - sumWidth;
            }

            var scollerBottom = Math.Abs(Scroller.HorizontalScollerBar.RenderRect.Right - ContentRect.Right);
            LayoutNum++;
            if (contentWidth < ChildRectWidth && Items.Count > 0 && isScollForward && LayoutNum < 100)
            {
                var right = Items.Last().View.RenderRect.Right;
                if (right != contentWidth && (scollerBottom < 2 || isLastIndex && !isScolled))
                {
                    Scroller.ScrollerWidth += contentWidth - right;
                    LayoutDynamicItems();
                }
            }
        }

        protected override void OnLayout()
        {
            var contentRect = ContentRect;
            var contentX = contentRect.X - childRect.X;
            var contentY = contentRect.Y - childRect.Y;
            childRect = contentRect;
            Childs.ForEach(n =>
            {
                n.X += contentX;
                n.Y += contentY;
                ((XGroup)n).Childs.ForEach(a =>
                {
                    a.Translation(contentX, contentY + Scroller.ScrollerHeight);
                });
            });
            DoAnimate();
        }

        public override void ScolledChilds(int x, int y)
        {
            // 垂直滑动
            if (x != 0)
            {
                LayoutNum = 0;
                foreach (var item in Items)
                {
                    item.View.Translation(x, 0);
                    if (!item.View.RenderRect.Intersect(ContentRect))
                    {
                        var template = Templates[item.TempleteIndex];
                        template.FirstCache.Remove(item.Index);
                        template.SecondCache[item.Index] = item;
                    }
                }
                isScolled = true;
                isScollForward = x < 0;
                LayoutItems();
                Scroller.UpdateScollerSize(ContentRect, ChildSize);
                Scroller.Layout(this);
                OnLayout();
            }
            else if (y != 0)
            {
                Childs.ForEach(n => ((XGroup)n).Childs.ForEach(a => a.Translation(0, y)));
            }
        }

        public override void ScrolledToIndex(int templeateIndex, int index, bool isSmooth)
        {
            // 只适合一个模板
            index = Math.Min(index, Datas.Count);
            index = Math.Max(index, 0);
            var oldScolleWidth = Scroller.ScrollerWidth;
            int x = 0;
            for (int i = 0; i < index; i++)
            {
                var width = Templates[0].ItemWidthAt(i);
                x += width + Space;
            }
            Scroller.ScrollerWidth = -x;
            Scroller.UpdateScollerSize(ContentRect, ChildSize);
            var newScollerWidth = Scroller.ScrollerWidth;
            Scroller.ScrollerWidth = oldScolleWidth;
            isScolled = true;
            isScollForward = true;
            var size = newScollerWidth - oldScolleWidth;
            if (isSmooth)
            {
                var animate = XAnimation.AnimateFloatOf();
                animate.Duration = 300;
                var start = Scroller.ScrollerHeight;
                animate.OnCallback = (value,i) =>
                {
                    OnScolled(false, (int)(start + size * value) - Scroller.ScrollerWidth);
                };
                animate.Start();
            }
            else
            {
                OnScolled(false, size);
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
