using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.views;

namespace XcyUI.templates
{
    internal class XLazyRowTemplate : XLazyTemplate
    {
        internal XLazyRowTemplate()
        {
            Height = XLayoutParams.Fill;
            Width = XLayoutParams.Wrap;
            MeausreHeight = XLayoutParams.Fill;
            MeausreWidth = XLayoutParams.Wrap;
        }
        internal static XLazyTemplate Create<T>(List<T> datas, Action<XGroup, T, int> funtion)
        {
            var template = new XLazyRowTemplate();
            template.Datas.Clear();
            datas?.ForEach(n => template.Datas.Add(n));
            template.Content = (v, d, index) =>
            {
                funtion.Invoke(v, (T)d, index);
            };
            return template;
        }

        internal override void MeasureFixed(XLazy lazy)
        {
            CacheSize.Clear();
            Space = lazy.Space;
            var item = ItemAt(lazy, 0);
            Width = item.View.Width;
            SumWidth = item.View.Width * Datas.Count + Space * (Datas.Count - 1);
            SumHeight = item.View.Height;
        }

        internal override void MeasureDynamic(XLazy lazy)
        {
            Space = lazy.Space;
            if (Width <= 0)
            {
                var contentRect = lazy.ContentRect;
                var items = new List<XLazyItem>();
                var left = 0;
                for (int i = 0; i < Datas.Count; i++)
                {
                    var item = LayoutItem(lazy, i, 0, left)?.Also(n => items.Add(n));
                    if (left > contentRect.Width)
                    {
                        break;
                    }
                    left += item.View.Width + Space;
                }
                var sumWidth = items.Sum(n => n.View.Width);
                Width = sumWidth / items.Count;
                SumHeight = items.Max(n => n.View.Childs.Max(a => a.Height));
            }
            SumWidth = Width * Datas.Count + Space * (Datas.Count - 1);
        }

        internal override XLazyItem LayoutItem(XLazy lazy, int index, int top,int left)
        {
            var isMeasureWidth = CacheSize.ContainsKey(index);
            var itemWidth = ItemWidthAt(index);
            itemWidth = Math.Max(0, itemWidth);
            var right = left + itemWidth;
            if (right >= 0)
            {
                var item = ItemAt(lazy, index);
                if (item.View.Height != lazy.ContentRect.Height)
                {
                    item.View.Measure();
                }
                
                if (Width > 0)
                {
                    SumWidth += item.View.Width - itemWidth;
                    var groupIndex = index / GroupItemCount;
                    if (!GroupSize.ContainsKey(groupIndex))
                    {
                        GroupSize.Add(groupIndex, Width * GroupItemCount);
                    }
                    GroupSize[groupIndex] += item.View.Width - itemWidth;
                }
                item.View.Y = top;
                item.View.X = left;
                CacheSize[index] = item.View.Width;
                item.View.Layout();
                return item;
            }
            return null;
        }

        internal override XLazyItem CreateItem(XLazy lazy, int index)
        {
            var item = base.CreateItem(lazy, index);
            CacheSize[index] = item.View.Width;
            return item;
        }
    }
}
