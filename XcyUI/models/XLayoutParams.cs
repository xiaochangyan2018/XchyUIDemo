using System;
namespace XcyUI.models
{
    public class XLayoutParams
    {
        public XLayoutParams()
        {
            Width = Wrap;
            Height = Wrap;
            Visible = XVisibleType.Visible;
        }
        public const int Fill = -1;
        public const int Wrap = -2;
        public int Width;
        public int Height;
        public int Weight;
        public float AspectRatio;
        public int MaxHeight;
        public int MinHeight;
        public int MaxWidth;
        public int MinWidth;
        public int ZIndex;
        public XAlignment Alignment;
        public XSpace Padding;
        public XSpace Margin;
        public bool Freeze;
        public int Colspan;
        public int Rowspan;
        public XVisibleType Visible = XVisibleType.Visible;

        public int PaddingLeft => (int)Padding.Left;
        public int PaddingTop => (int)Padding.Top;
        public int PaddingRight => (int)Padding.Right;
        public int PaddingBottom => (int)Padding.Bottom;
        public int MarginLeft => (int)Margin.Left;
        public int MarginTop => (int)Margin.Top;
        public int MarginRight => (int)Margin.Right;
        public int MarginBottom => (int)Margin.Bottom;
        public bool IsFillWidth => Width == Fill;
        public bool IsWrapWidth => Width == Wrap;
        public bool IsFillHeight => Height == Fill;
        public bool IsWrapHeight => Height == Wrap;

        public int MeasureHashCode()
        {
            var leftCode = (Width, Height, Weight, MaxWidth, MaxHeight, MinWidth, MinHeight).GetHashCode();
            var rightCode = (Padding, Margin, Alignment, Visible, Freeze, Colspan, AspectRatio).GetHashCode();
            return (leftCode, rightCode).GetHashCode();
        }

        internal void Reset()
        {
            Width = Wrap;
            Height = Wrap;
            Weight = 0;
            MaxHeight = 0;
            MinHeight = 0;
            MaxWidth = 0;
            MinWidth = 0;
            ZIndex = 0;
            Colspan = 0;
            Alignment = XAlignment.None;
            Padding = new XSpace(0);
            Margin = new XSpace(0);
            AspectRatio = 0;
            Freeze = false;
            Visible = XVisibleType.Visible;
        }
    }

    public enum XVisibleType
    {
        Gone,
        InVisible,
        Visible,
    }
}
