using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.Core.Utils;
using XcyUI.models;

namespace XcyUI.views
{
    public class XBox : XGroup
    {
        public XAlignment ContentAlignment = XAlignment.Center;

        protected override void MeasureChilds()
        {
            var contentRect = ContentRect;
            contentRect.X = 0;
            contentRect.Y = 0;
            var childs = _layoutChilds;
            _retryMeasureChilds.Clear();
            for (int i = 0; i < childs.Count; i++)
            {
                var child = childs[i];
                child.Measure();
                if (child.Width <= 0 || child.Height <= 0)
                {
                    _retryMeasureChilds.Add(child);
                }
                ChildRectWidth = Math.Max(ChildRectWidth, child.Width + child.LayoutParams.Margin.HorizontalSize);
                ChildRectHeight = Math.Max(ChildRectHeight, child.Height + child.LayoutParams.Margin.VerticalSize);
            }
            MeasureWrapSize();
            foreach (var child in _retryMeasureChilds)
            {
                child.Measure();
            }
        }

        protected override void OnLayout()
        {
            var contentRect = ContentRect;
            for (int i = 0; i < _layoutChilds.Count; i++)
            {
                var child = _layoutChilds[i];
                var align = child.LayoutParams.Alignment;
                if (align == XAlignment.None)
                {
                    align = ContentAlignment;
                }
                var point = XRectUtils.GetDockedPoint(contentRect, child.RenderRect, align, child.LayoutParams.Margin);
                child.Location = point;
                child.Layout();
            }
        }
    }
}
