using System;
using System.Collections.Generic;
using System.Text;
using XcyUI.models;

namespace XcyUI.Core.Utils
{
    public static class XRectUtils
    {

        public static XPoint GetDockedPoint(XRect rect, XRect childRect, XAlignment alignment, XSpace margin = default)
        {
            var width = (rect.Width - childRect.Width) / 2;
            var height = (rect.Height - childRect.Height) / 2;
            var left = (int)margin.Left;
            var top = (int)margin.Top;
            var right = (int)margin.Right;
            var bottom = (int)margin.Bottom;
            return alignment switch
            {
                XAlignment.LeftTop => new(rect.X + left, rect.Y + top),
                XAlignment.TopCenter => new(rect.X + width + left - right, rect.Y+ top),
                XAlignment.RightTop => new(rect.Right - right - childRect.Width, rect.Y + top),
                XAlignment.RightCenter => new(rect.Right - right - childRect.Width, rect.Y + height + top - bottom),
                XAlignment.RightBottom => new(rect.Right - right-childRect.Width, rect.Bottom - bottom - childRect.Height),
                XAlignment.BottomCenter => new(rect.X + width + left - right, rect.Bottom - bottom-childRect.Height),
                XAlignment.LeftBottom => new(rect.X + left, rect.Bottom - bottom - childRect.Height),
                XAlignment.LeftCenter => new(rect.X + left, rect.Y + height + top - bottom),
                XAlignment.Center => new(rect.X + width + left - right, rect.Y + height + top - bottom),
                XAlignment.None => new(rect.X + left, rect.Y + top),
                _ => new(rect.X + left, rect.Y + top)
            };
        }
    }
}
