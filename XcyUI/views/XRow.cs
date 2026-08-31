using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.Core.Utils;
using XcyUI.models;

namespace XcyUI.views
{
    public class XRow : XColumn
    {
        protected override void MeasureChilds()
        {
            var contentRect = ContentRect;
            contentRect.X = 0;
            contentRect.Y = 0;
            var childs = _layoutChilds;
            var spaceSize = Space * (childs.Count - 1);
            if (HorizontalAlignment == XHorizontalAlignment.Bisect)
            {
                var bisectWidth = (contentRect.Width - spaceSize) / childs.Count;
                foreach (var child in childs)
                {
                    child.LayoutParams.Width = bisectWidth - child.LayoutParams.Margin.HorizontalSize;
                    child.Measure();
                    ChildRectWidth += child.Width + child.LayoutParams.Margin.HorizontalSize;
                    ChildRectHeight = Math.Max(child.Height + child.LayoutParams.Margin.VerticalSize, ChildRectHeight);
                }
            }
            else
            {
                var sumWeights = 0;
                _weightItems.Clear();
                var normalWidth = 0;
                _retryMeasureChilds.Clear();
                for (int i = 0; i < childs.Count; i++)
                {
                    var child = childs[i];
                    sumWeights += child.LayoutParams.Weight;
                    if (child.LayoutParams.Weight == 0)
                    {
                        child.Measure();
                        if (child.Width <= 0 || child.Height <= 0)
                        {
                            _retryMeasureChilds.Add(child);
                        }
                        normalWidth += child.Width + child.LayoutParams.Margin.HorizontalSize + Space;
                        ChildRectWidth += child.Width + child.LayoutParams.Margin.HorizontalSize;
                        ChildRectHeight = Math.Max(child.Height + child.LayoutParams.Margin.VerticalSize, ChildRectHeight);
                    }
                    else
                    {
                        _weightItems.Add(child);
                    }
                }

                if (LayoutParams.IsWrapWidth)
                {
                    foreach (var child in _weightItems)
                    {
                        child.Measure();
                        ChildRectHeight = Math.Max(child.Height + child.LayoutParams.Margin.VerticalSize, ChildRectHeight);
                        ChildRectWidth += child.Width + child.LayoutParams.Margin.HorizontalSize;
                        normalWidth += child.Width + child.LayoutParams.Margin.HorizontalSize + Space;
                    }
                    if (LayoutParams.MaxWidth > 0)
                    {
                        Width = Math.Min(LayoutParams.MaxWidth, normalWidth);
                    }
                    
                    if (LayoutParams.MaxWidth > 0 && Width == LayoutParams.MaxWidth)
                    {
                        foreach (var child in _weightItems)
                        {
                            ChildRectWidth -= child.Width + child.LayoutParams.Margin.HorizontalSize;
                            normalWidth -= child.Width + child.LayoutParams.Margin.HorizontalSize + Space;
                        }
                    }
                    else
                    {
                        _weightItems.Clear();
                    }
                }

                var weightWidth = Width - normalWidth - (_weightItems.Count - 1) * Space - LayoutParams.Padding.HorizontalSize;
                foreach (var child in _weightItems)
                {
                    child.LayoutParams.Width = (int)((float)child.LayoutParams.Weight / sumWeights * weightWidth - child.LayoutParams.Margin.HorizontalSize);
                    child.Measure();
                    ChildRectWidth += child.Width + child.LayoutParams.Margin.HorizontalSize;
                    ChildRectHeight = Math.Max(child.Height + child.LayoutParams.Margin.VerticalSize, ChildRectHeight);
                }
            }

            ChildRectWidth += Math.Max(0, spaceSize);
            MeasureWrapSize();
            foreach (var child in _retryMeasureChilds)
            {
                child.Measure();
            }
        }

        protected override void OnLayout()
        {
            var contentRect = ContentRect;
            var childs = _layoutChilds;
            var scollerHeight = Scroller?.ScrollerHeight ?? 0;
            var scollerWidth = Scroller?.ScrollerWidth ?? 0;
            var top = scollerHeight;
            var left = X + LayoutParams.PaddingLeft + scollerWidth;
            var horizontalAlignment = HorizontalAlignment;
            for (int i = 0; i < childs.Count; i++)
            {
                var child = childs[i];
                left += child.LayoutParams.MarginLeft;
                var align = child.LayoutParams.Alignment != XAlignment.None ? child.LayoutParams.Alignment : GetVerticalAlignment();

                var point = XRectUtils.GetDockedPoint(contentRect, child.RenderRect, align, child.LayoutParams.Margin);

                child.Location = new XPoint(left + RemainWidth(horizontalAlignment), point.Y + top);

                left += child.Width + child.LayoutParams.MarginRight + Space;
                child.Layout();
            }
        }

        private XAlignment GetVerticalAlignment()
        {
            return VerticalAlignment == XVerticalAlignment.Top ? XAlignment.TopCenter : VerticalAlignment == XVerticalAlignment.Bottom ? XAlignment.BottomCenter : XAlignment.Center;
        }
        private int RemainWidth(XHorizontalAlignment alignment)
        {
            switch (alignment)
            {
                case XHorizontalAlignment.Right:
                    return ContentRect.Width - ChildRectWidth;
                case XHorizontalAlignment.Center:
                    return (ContentRect.Width - ChildRectWidth) / 2;
            }
            return 0;
        }
    }
}
