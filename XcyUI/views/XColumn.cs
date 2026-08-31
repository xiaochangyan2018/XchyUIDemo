using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.Core.Utils;
using XcyUI.models;

namespace XcyUI.views
{
    public class XColumn: XGroup
    {
        public XHorizontalAlignment HorizontalAlignment = XHorizontalAlignment.Center;
        public XVerticalAlignment VerticalAlignment = XVerticalAlignment.Top;
        protected List<XView> _weightItems = new();

        protected override void MeasureChilds()
        {
            var contentRect = ContentRect;
            contentRect.X = 0;
            contentRect.Y = 0;
            var childs = _layoutChilds;
            var spaceSize = Space * (childs.Count - 1);
            if (VerticalAlignment == XVerticalAlignment.Bisect)
            {
                var bisectHeight = (contentRect.Height - spaceSize) / childs.Count;
                foreach (var child in childs)
                {
                    child.LayoutParams.Height = bisectHeight - child.LayoutParams.Margin.VerticalSize;
                    child.Measure();
                    ChildRectWidth = Math.Max(child.Width + child.LayoutParams.Margin.HorizontalSize, ChildRectWidth);
                    ChildRectHeight += child.Height + child.LayoutParams.Margin.VerticalSize;
                }
            }
            else
            {
                var sumWeights = 0;
                _weightItems.Clear();
                var normalHeight = 0;
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
                        normalHeight += child.Height + child.LayoutParams.Margin.VerticalSize + Space;
                        ChildRectWidth = Math.Max(child.Width + child.LayoutParams.Margin.HorizontalSize, ChildRectWidth);
                        ChildRectHeight += child.Height + child.LayoutParams.Margin.VerticalSize;
                    }
                    else
                    {
                        _weightItems.Add(child);
                    }
                }

                if (LayoutParams.IsWrapHeight)
                {
                    foreach (var child in _weightItems)
                    {
                        child.Measure();
                        ChildRectWidth = Math.Max(child.Width + child.LayoutParams.Margin.HorizontalSize, ChildRectWidth);
                        ChildRectHeight += child.Height + child.LayoutParams.Margin.VerticalSize;
                        normalHeight += child.Height + child.LayoutParams.Margin.VerticalSize + Space;
                    }
                    if (LayoutParams.MaxHeight > 0)
                    {
                        Height = Math.Min(LayoutParams.MaxHeight, normalHeight);
                    }
                    if (LayoutParams.MaxHeight > 0 && Height == LayoutParams.MaxHeight)
                    {
                        foreach (var child in _weightItems)
                        {
                            ChildRectHeight -= child.Height + child.LayoutParams.Margin.VerticalSize;
                            normalHeight -= child.Height + child.LayoutParams.Margin.VerticalSize + Space;
                        }
                    }
                    else
                    {
                        _weightItems.Clear();
                    }
                }

                var weightHeight = Height - normalHeight - (_weightItems.Count - 1) * Space - LayoutParams.Padding.VerticalSize;
                foreach (var n in _weightItems)
                {
                    n.LayoutParams.Height = (int)((float)n.LayoutParams.Weight / sumWeights * weightHeight - n.LayoutParams.Margin.VerticalSize);
                    n.Measure();
                    ChildRectWidth = Math.Max(n.Width + n.LayoutParams.Margin.HorizontalSize, ChildRectWidth);
                    ChildRectHeight += n.Height + n.LayoutParams.Margin.VerticalSize;
                }
            }

            ChildRectHeight += Math.Max(spaceSize, 0);
            MeasureWrapSize();
            foreach (var item in _retryMeasureChilds)
            {
                item.Measure();
            }
        }

        protected override void OnLayout()
        {
            var contentRect = ContentRect;
            var childs = _layoutChilds;
            var scollerHeight = Scroller?.ScrollerHeight ?? 0;
            var scollerWidth = Scroller?.ScrollerWidth ?? 0;
            var top = Y + (int)LayoutParams.Padding.Top + scollerHeight;
            var left = scollerWidth;
            var verticalAlignment = VerticalAlignment;
            for (int i = 0; i < childs.Count; i++)
            {
                var child = childs[i];
                top += child.LayoutParams.MarginTop;
                
                var align = child.LayoutParams.Alignment != XAlignment.None ? child.LayoutParams.Alignment : GetHorizontalAlignment();
                var point = XRectUtils.GetDockedPoint(contentRect, child.RenderRect, align, child.LayoutParams.Margin);
                
                child.Location = new XPoint(point.X + left, top + RemainHeight(verticalAlignment));
                top += child.Height + child.LayoutParams.MarginBottom + Space;
                child.Layout();
            }
        }

        private XAlignment GetHorizontalAlignment()
        {
            return HorizontalAlignment == XHorizontalAlignment.Left ? XAlignment.LeftCenter : HorizontalAlignment == XHorizontalAlignment.Right ? XAlignment.RightCenter : XAlignment.Center;
        }
        private int RemainHeight(XVerticalAlignment alignment)
        {
            switch (alignment)
            {
                case XVerticalAlignment.Bottom:
                    return ContentRect.Height - ChildRectHeight;
                case XVerticalAlignment.Center:
                    return (ContentRect.Height - ChildRectHeight) / 2;
            }
            return 0;
        }
    }
}
