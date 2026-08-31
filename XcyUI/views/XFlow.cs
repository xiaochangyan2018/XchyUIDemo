using System;
using System.Linq;
using XcyUI.Core.Utils;
using XcyUI.models;

namespace XcyUI.views
{
    public class XFlow: XGroup
    {
        public int Cells;
        protected override void MeasureChilds()
        {
            var contentRect = ContentRect;
            var left = 0;
            var top = 0;
            var rowHeight = 0;
            var contentWidth = contentRect.Width;
            var childWidth = 0;
            if (Cells > 0)
            {
                childWidth = (contentWidth - (Space * (Cells - 1))) / Cells;
            }
            for (int i = 0; i < _layoutChilds.Count; i++)
            {
                var child = _layoutChilds[i];
                if (childWidth > 0)
                {
                    var colspan = child.LayoutParams.Colspan == 0 ? 1 : child.LayoutParams.Colspan;
                    if (colspan > Cells)
                    {
                        colspan = Cells;
                    }
                    child.LayoutParams.Width = colspan * childWidth + (colspan - 1) * Space;
                }

                child.Measure();
                if (left + child.Width > contentWidth)
                {
                    left = 0;
                    top += rowHeight + Space;
                    ChildRectHeight += rowHeight+ Space;
                    rowHeight = 0;
                }
                rowHeight = Math.Max(rowHeight, child.Height);
                left += child.Width + Space;
                ChildRectWidth = Math.Max(0, left);
            }
            ChildRectWidth -= Space;
            ChildRectHeight += rowHeight;
            MeasureWrapSize();
        }

        protected override void OnLayout()
        {
            var scollerHeight = Scroller?.ScrollerHeight ?? 0;
            var scollerWidth = Scroller?.ScrollerWidth ?? 0;
            var contentRect = ContentRect;
            var left = contentRect.X + scollerWidth;
            var top = contentRect.Y + scollerHeight;
            var rowHeight = 0;
            foreach (var child in _layoutChilds)
            {
                if (left + child.Width > contentRect.Right)
                {
                    left = contentRect.X + scollerWidth;
                    top += rowHeight + Space;
                    rowHeight = 0;
                }
                child.Location = new XPoint(left, top);
                child.Layout();
                rowHeight = Math.Max(rowHeight, child.Height);
                left += child.Width + Space;
            }
        }
    }
}
