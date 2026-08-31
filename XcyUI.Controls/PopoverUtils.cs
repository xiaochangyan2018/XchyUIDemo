using XcyUI.expansions;
using XcyUI.models;
using XcyUI.utils;

namespace XcyUI.Controls
{
    public class PopoverUtils
    {
        internal static ArrowDirection GetArrowDirection(XRect rect, XRect sourceRect, int width, int height, bool isAlignLeft = true, int space = 20)
        {
            space = space.AsPx();
            var marginX = isAlignLeft ? sourceRect.X : sourceRect.X - (rect.Width - sourceRect.Width) / 2;
            var marginY = sourceRect.Bottom + space;
            var destRect = new XRect(marginX, marginY, rect.Width, rect.Height);
            var arrowDirection = ArrowDirection.Top;
            if (destRect.Bottom > height)
            {
                destRect.Y = sourceRect.Y - space - rect.Height;
                arrowDirection = ArrowDirection.Bottom;
            }
            if (destRect.Y < 0)
            {
                destRect.Y = sourceRect.Center.Y - rect.Height / 2;
                destRect.X = sourceRect.Right + space;
                arrowDirection = ArrowDirection.Left;                
            }
            if (destRect.Right > width)
            {
                destRect.X = sourceRect.X - space - rect.Width;
                destRect.Y = sourceRect.Center.Y - rect.Height / 2;
                arrowDirection = ArrowDirection.Right;
            }
            if (destRect.X < 0)
            {
                destRect.X = marginX;
                destRect.Y = marginY;
                arrowDirection = ArrowDirection.Top;
            }
            return arrowDirection;
        }
        
        
        public static void DrawRoundedArrowBubble(
            XRect rect,
            XStyle style,
            bool isCache,
            XRect hoverRect,
            int radius,        // 气泡圆角
            int arrowSize,     // 箭头长度
            ArrowDirection dir,
            int strokeWidth = 1)
        {
            int l = rect.Left;
            int t = rect.Top;
            int r = rect.Right;
            int b = rect.Bottom;
            int w = rect.Width;
            int h = rect.Height;
            int cx = hoverRect.Center.X; // 中心X
            int cy = hoverRect.Center.Y; // 中心Y
            // 计算方向
            RenderImp.DrawPath(rect, style, isCache, () =>
            {
                RenderImp.MoveTo(l + radius, t);
                if (dir == ArrowDirection.Top)
                {
                    RenderImp.LineTo(cx - arrowSize, t);
                    RenderImp.LineTo(cx, t - arrowSize);
                    RenderImp.LineTo(cx + arrowSize, t);
                }
                RenderImp.LineTo(r - radius, t);
                RenderImp.ArcTo(r, t + radius, radius);
                if (dir == ArrowDirection.Right)
                {
                    RenderImp.LineTo(r, cy - arrowSize);
                    RenderImp.LineTo(r + arrowSize, cy);
                    RenderImp.LineTo(r, cy + arrowSize);
                }
                RenderImp.LineTo(r, b - radius);
                RenderImp.ArcTo(r - radius, b, radius);
                if (dir == ArrowDirection.Bottom)
                {
                    RenderImp.LineTo(cx + arrowSize, b);
                    RenderImp.LineTo(cx, b + arrowSize);
                    RenderImp.LineTo(cx - arrowSize, b);
                }
                RenderImp.LineTo(l + radius, b);
                RenderImp.ArcTo(l, b - radius, radius);
                if (dir == ArrowDirection.Left)
                {
                    RenderImp.LineTo(l, cy + arrowSize);
                    RenderImp.LineTo(l - arrowSize, cy);
                    RenderImp.LineTo(l, cy - arrowSize);
                }

                RenderImp.LineTo(l, t + radius);
                RenderImp.ArcTo(l + radius, t, radius);
            });
        }

        public static XPoint GetLocation(XRect rect, XRect sourceRect, int width, int height, bool isAlignLeft = true, int space = 20)
        {
            space = space.AsPx();
            var marginX = isAlignLeft ? sourceRect.X : sourceRect.X - (rect.Width - sourceRect.Width) / 2;
            var marginY = sourceRect.Bottom + space;
            var destRect = new XRect(marginX, marginY, rect.Width, rect.Height);
            if (destRect.Bottom > height)
            {
                destRect.Y = sourceRect.Y - space - rect.Height;
            }
            if (destRect.Y < 0)
            {
                destRect.Y = sourceRect.Center.Y - rect.Height / 2;
                destRect.X = sourceRect.Right + space;
                if (destRect.Y < 0)
                {
                    destRect.Y = 0;
                }
                else if (destRect.Bottom > height)
                {
                    destRect.Y -= destRect.Bottom - height;
                }
            }
            if (destRect.Right > width)
            {
                destRect.X = sourceRect.X - space - rect.Width;
                destRect.Y = sourceRect.Center.Y - rect.Height / 2;
                if (destRect.Y < 0)
                {
                    destRect.Y = 0;
                }
                else if (destRect.Bottom > height)
                {
                    destRect.Y -= destRect.Bottom - height;
                }
            }
            if (destRect.X < 0)
            {
                destRect.X = marginX;
                destRect.Y = marginY;
            }
            if(destRect.X <=2)
            {
                destRect.X = 3;
            }
            return destRect.Point;
        }

        public enum ArrowDirection
        {
            Top,
            Bottom,
            Left,
            Right
        }
    }
}
