using System;
namespace XcyUI.models
{
    public class XStyle
    {
        public static readonly XStyle Empty = new();
        public bool IsClip;
        public bool IsClipPadding;
        public bool IsOverDraw;
        public bool IsOnlySet;
        public XSpace ClipPadding;
        public XBrush Background;
        public XBorder Border;
        public XSpace Radius;
        public XShadow Shadow;
        
        public void Reset()
        {
            //IsClip = false;
            //IsClipPadding = false;
            IsOverDraw = false;
            ClipPadding = new XSpace();
            Background = new XBrush();
            Border = new XBorder();
            Radius = new XSpace();
            Shadow = new XShadow();
        }

        // 复制一份
        public XStyle Copy()
        {
            return new XStyle()
            {
                Background = Background,
                Border = Border,
                Radius = Radius,
                Shadow = Shadow,
                IsClip = IsClip,
                IsClipPadding = IsClipPadding,
                ClipPadding = ClipPadding
            };
        }
    }    
}
