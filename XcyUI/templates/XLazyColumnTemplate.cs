using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.views;

namespace XcyUI.templates
{
    internal class XLazyColumnTemplate: XLazyTemplate
    {
        internal XLazyColumnTemplate()
        {
            Width = XLayoutParams.Fill;
            Height = XLayoutParams.Wrap;
            MeausreWidth = XLayoutParams.Fill;
            MeausreHeight = XLayoutParams.Wrap;
        }
        internal static XLazyTemplate Create<T>(List<T> datas, Action<XGroup, T,int> funtion)
        {
            var template = new XLazyColumnTemplate();
            template.Datas.Clear();           
            template.CacheSize.Clear();
            datas?.ForEach(n => template.Datas.Add(n));
            template.Content = (v, d, index) =>
            {
                funtion.Invoke(v, (T)d, index);
            };
            return template;
        }

        internal override void MeasureFixed(XLazy lazy)
        {
            Space = lazy.Space;
            var item = ItemAt(lazy, 0);
            Height = item.View.Height;
            SumHeight = item.View.Height * Datas.Count + Space * (Datas.Count - 1);
            SumWidth = item.View.Width;
        }

        internal override void MeasureDynamic(XLazy lazy)
        {
            Space = lazy.Space;
            if (Height <= 0)
            {
                var contentRect = lazy.ContentRect;
                var items = new List<XLazyItem>();
                var top = 0;
                for (int i = 0; i < Datas.Count; i++)
                {
                    var item = LayoutItem(lazy, i, top, 0)?.Also(n => items.Add(n));
                    if (top > contentRect.Height)
                    {
                        break;
                    }
                    top += item.View.Height + Space;
                }
                var sumHeight = items.Sum(n => n.View.Height);
                Height = sumHeight / items.Count;
                SumWidth = items.Max(n => n.View.Childs.Max(a => a.Width));
            }
            SumHeight = Height * Datas.Count + Space * (Datas.Count - 1);
        }

        internal override XLazyItem LayoutItem(XLazy lazy, int index, int top,int left)
        {
            var isMeasureHeight = CacheSize.ContainsKey(index);
            var itemHeight = ItemHeightAt(index);
            itemHeight = Math.Max(0, itemHeight);
            var bottom = top + itemHeight;
            if (bottom >= 0)
            {
                var item = ItemAt(lazy, index);
                if (item.View.Width != lazy.ContentRect.Width)
                {
                    item.View.Measure();
                }
                if (Height > 0)
                {
                    SumHeight += item.View.Height - itemHeight;
                    var groupIndex = index / GroupItemCount;
                    if (!GroupSize.ContainsKey(groupIndex))
                    {
                        GroupSize.Add(groupIndex, Height * GroupItemCount);
                    }
                    GroupSize[groupIndex] += item.View.Height - itemHeight;
                }
                item.View.Y = top;
                item.View.X = left;
                CacheSize[index] = item.View.Height;
                item.View.Layout();
                return item;
            }
            return null;
        }
    }
}
