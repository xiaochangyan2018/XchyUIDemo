using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.views;

namespace XcyUI.templates
{
    internal class XLazyGridTemplate : XLazyTemplate
    {
        internal int Span { get; set; }
        internal int Cells { get; set; }
        internal List<XLazyGridTemplateGroup> Groups { get; private set; }

        internal XLazyGridTemplate()
        {
            Height = XLayoutParams.Wrap;
            Width = XLayoutParams.Fill;
            MeausreHeight = XLayoutParams.Wrap;
            MeausreWidth = XLayoutParams.Fill;
        }
        internal static XLazyTemplate Create<T>(List<T> datas,int span,int cells, Action<XGroup, T, int> funtion)
        {
            var template = new XLazyGridTemplate();
            template.Span = span;
            template.Cells = cells;
            template.Datas.Clear();
            datas.ForEach(n => template.Datas.Add(n));
            template.Content = (v, d, index) =>
            {
                funtion.Invoke(v, (T)d, index);
            };
            template.Grouping();
            return template;
        }

        private void Grouping()
        {
            Groups = new List<XLazyGridTemplateGroup>();
            var goupCount = Datas.Count / Cells;
            for (int i = 0; i < Cells; i++)
            {
                var group = new XLazyGridTemplateGroup();
                group.GroupNum = i;
                group.Count = goupCount;
                if (Datas.Count % Cells > i)
                {
                    group.Count += 1;
                }
                Groups.Add(group);
            }
        }

        internal override void MeasureFixed(XLazy lazy)
        {
            CacheSize.Clear();
            Space = lazy.Space;
            var width = (float)(lazy.Width - (Cells + 1) * Space) / Cells;
            Width = (int)(width * Span) + (Span - 1) * Space;
            MeausreWidth = Width;
            var item = ItemAt(lazy, 0);
            Height = item.View.Height;
            MeausreHeight = Height;
            var rowCount = (int)Math.Ceiling((float)Datas.Count / Cells);
            SumHeight = item.View.Height * rowCount + Space * (rowCount - 1);
        }

        internal override void MeasureDynamic(XLazy lazy)
        {
            CacheSize.Clear();
            Space = lazy.Space;
            Width = (lazy.Width - (Cells + 1) * Space) / Cells;
            MeausreWidth = Width;
            var contentRect = lazy.ContentRect;
            var itemsMap = new Dictionary<int, List<XLazyItem>>();
            foreach (var groupList in Groups)
            {
                var top = 0;
                itemsMap[groupList.GroupNum] = new List<XLazyItem>();
                for (int y = 0; y < groupList.Count; y++)
                {
                    var index = y * Cells + groupList.GroupNum;
                    var item = LayoutItem(lazy, index, top, 0)?.Also(n => itemsMap[groupList.GroupNum].Add(n));
                    item.IndexInGroup = y;
                    if (top > contentRect.Height)
                    {
                        break;
                    }
                    top += item.View.Height + Space;
                }
            }
            var items = new List<XLazyItem>();
            foreach (var item in itemsMap.Values)
            {
                items.AddRange(item);
            }
            var sumHeight = items.Sum(n => n.View.Height);
            Height = sumHeight / items.Count;
            Groups.ForEach(n =>
            {
                n.ItemHeight = Height;
                var count = itemsMap[n.GroupNum].Count;
                var currentHeight = itemsMap[n.GroupNum].Sum(a => a.View.Height);
                n.SumHeight = Height * (n.Count - count) + currentHeight + Space * (n.Count - 1);
            });
            SumHeight = Groups.Max(n => n.SumHeight);
        }

        internal override XLazyItem LayoutItem(XLazy lazy, int index, int top,int left)
        {            
            var itemHeight = ItemHeightAt(index);
            itemHeight = Math.Max(0, itemHeight);
            var bottom = top + itemHeight;
            if (bottom >= 0)
            {
                var item = ItemAt(lazy, index);
                CacheSize[index] = item.View.Height;
                if (item.View.Width != MeausreWidth)
                {
                    item.View.LayoutParams.Width = MeausreWidth;
                    item.View.Measure();
                }
                item.View.Y = top;
                item.View.X = left;
                item.View.Layout();
                return item;
            }
            return null;
        }
    }

    internal class XLazyGridTemplateGroup
    {
        internal int GroupNum { get; set; }
        internal int Count { get; set; }
        internal int SumHeight { get; set; }
        internal int ItemHeight { get; set; }
    }
}
