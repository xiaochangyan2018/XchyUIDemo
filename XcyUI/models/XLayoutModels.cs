using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XcyUI.models
{
    public enum XAlignment
    {
        None,
        LeftTop,
        LeftCenter,
        LeftBottom,
        RightTop,
        RightCenter,
        RightBottom,
        Center,
        TopCenter,
        BottomCenter,
    }

    public enum XHorizontalAlignment
    {
        Left = 1,
        Center = 2,
        Right = 3,
        Bisect = 20
    }

    public enum XVerticalAlignment
    {
        Top = 0,
        Center = 3,
        Bottom = 6,
        Bisect = 10
    }

    public enum XCursorType
    {
        Arrow,
        Input,
        Crosshair,
        Hand,
        HResize,
        VResize,
        NwseResize,
        NeswResize,
        AllResize,
        NotAllowed
    }

    public struct XPoint
    {
        public static readonly XPoint Empty = new XPoint(int.MinValue, int.MinValue);
        public XPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public int X;
        public int Y;

        public void Offset(int x, int y)
        {
            X += x;
            Y += y;
        }
    }
    public struct XRect
    {
        public static readonly XRect Empty;
        public XRect() { }
        public XRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public XRect(int width, int height)
        {
            X = 0;
            Y = 0;
            Width = width;
            Height = height;
        }

        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int Top { get => Y; }
        public int Left { get => X; }
        public int Right { get => X + Width; }
        public int Bottom { get => Y + Height; }
        public XPoint Point => new XPoint(X, Y);
        public XPoint LeftTop => new XPoint(X, Y);
        public XPoint RightTop => new XPoint(Right, Y);
        public XPoint LeftCenter => new XPoint(X, Center.Y);
        public XPoint LeftBottom => new XPoint(X, Bottom);
        public XPoint RightCenter => new XPoint(Right, Center.Y);
        public XPoint RightBottom => new XPoint(Right, Bottom);
        public XPoint TopCenter => new XPoint(Right - Width / 2, Y);
        public XPoint BottomCenter => new XPoint(Right - Width / 2, Bottom);
        public XPoint BottomLeft => new XPoint(Left, Bottom);
        public XSize Size => new XSize(Width, Height);
        public float Area => Width * Height;

        public bool Contain(XPoint point)
        {
            return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
        }

        public XPoint Center => new XPoint(X + Width / 2, Y + Height / 2);


        public void Scale(int size)
        {
            X -= size;
            Y -= size;
            Width += size * 2;
            Height += size * 2;
        }

        public void ScaleRevert(XSpace size)
        {
            Scale(-(int)size.Left, -(int)size.Top, -(int)size.Right, -(int)size.Bottom);
        }
        public XRect InsetSpace(XSpace size)
        {
            Scale(-(int)size.Left, -(int)size.Top, -(int)size.Right, -(int)size.Bottom);
            return this;
        }
        public void Scale(int left, int top, int right, int bottom)
        {
            X -= left;
            Y -= top;
            Width += left + right;
            Height += top + bottom;
        }

        public void Translation(int x, int y)
        {
            X += x;
            Y += y;
        }


        public void Move(XRect rect, XAlignment align, XSpace margin = new XSpace())
        {
            var startPoint = new XPoint(rect.Left, rect.Top);
            var widthDiff = Width == 0 ? 0 : (rect.Width - Width);
            var heightDiff = Height == 0 ? 0 : (rect.Height - Height);
            switch (align)
            {
                case XAlignment.LeftTop:
                    startPoint.Offset((int)margin.Left, (int)margin.Top);
                    break;
                case XAlignment.TopCenter:
                    startPoint.Offset(widthDiff / 2, (int)margin.Top);
                    break;
                case XAlignment.RightTop:
                    startPoint.Offset(widthDiff - (int)margin.Right, (int)margin.Top);
                    break;
                case XAlignment.LeftCenter:
                    startPoint.Offset((int)margin.Left, heightDiff / 2);
                    break;
                case XAlignment.Center:
                    startPoint.Offset(widthDiff / 2, heightDiff / 2);
                    break;
                case XAlignment.RightCenter:
                    startPoint.Offset(widthDiff - (int)margin.Right, heightDiff / 2);
                    break;
                case XAlignment.LeftBottom:
                    startPoint.Offset((int)margin.Left, heightDiff - (int)margin.Bottom);
                    break;
                case XAlignment.BottomCenter:
                    startPoint.Offset(widthDiff / 2, heightDiff - (int)margin.Bottom);
                    break;
                case XAlignment.RightBottom:
                    startPoint.Offset(widthDiff - (int)margin.Right, heightDiff - (int)margin.Bottom);
                    break;
            }
            X = startPoint.X;
            Y = startPoint.Y;
        }

        public bool Intersect(int left, int top, int right, int bottom)
        {
            if (Left < right && left < Right && Top < bottom && top < Bottom)
            {
                if (Left < left)
                {
                    Width = Right - left;
                    X = left;
                }
                if (Top < top)
                {
                    Height = Bottom - top;
                    Y = top;
                }
                if (Right > right) Width = right - Left;
                if (Bottom > bottom) Height = bottom - Top;
                return true;
            }
            return false;
        }

        public bool Intersect(XRect other)
        {
            return Intersect(other.Left, other.Top, other.Right, other.Bottom);
        }

        public XRect TopIntersectRect(XRect other)
        {
            var rect = this;
            rect.IntersectUnchecked(other);
            return rect;
        }

        public void IntersectUnchecked(XRect other)
        {
            var right = Right;
            var bottom = Bottom;
            X = Math.Max(X, other.X);
            Y = Math.Max(Y, other.Y);
            Width = Math.Min(right, other.Right) - X;
            Height = Math.Min(bottom, other.Bottom) - Y;
        }
    }

    public struct XSize
    {
        public XSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
        public int Width;
        public int Height;
    }

    public struct XSpace
    {
        public XSpace(float all)
        {
            Left = all;
            Top = all;
            Right = all;
            Bottom = all;
        }

        public XSpace(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public float All
        {
            get => IsFullSize ? Left : 0;
            set
            {
                Left = Top = Right = Bottom = All;
            }
        }
        public bool IsFullSize => Left == Top && Top == Right && Right == Bottom;

        public bool HasSize => Left > 0 || Top > 0 || Right > 0 || Bottom > 0;

        public int HorizontalSize => (int)(Left + Right);
        public int VerticalSize => (int)(Top + Bottom);
    }
}
