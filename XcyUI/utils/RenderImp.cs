using System;
using System.Collections.Generic;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.views;

namespace XcyUI.utils
{
    public class RenderImp
    {
        private static IDraw _draw;
        private static IWindow _window;
        private static bool isInvalidate = false;
        public static bool lockInvalidate = false;
        private static object _lock = new object();
        public static void SetDraw(IDraw draw)
        {
            lock (_lock)
            {
                _draw = draw;
            }
        }

        public static void SetWindow(IWindow window)
        {
            lock (_lock)
            {
                _window = window;
            }
        }

        public static IWindow GetWindow()
        {
            return _window;
        }

        public static void DrawRect(XRect rect, XStyle style)
        {
            _draw?.DrawRect(rect, style, null);
        }

        public static void Draw(XRect rect, XStyle style, XDrawCache cache, Action onDraw)
        {
            if (cache.EnableCache)
            {
                _draw.DrawCache(rect, style, cache, onDraw);
            }
            else
            {
                _draw?.DrawRect(rect, style, onDraw);
            }
        }

        public static void DrawRect(XRect rect, XStyle style, Action onDraw)
        {
            _draw?.DrawRect(rect, style, onDraw);
        }


        public static void DrawText(List<XChar> chars)
        {
            _draw?.DrawText(chars);
        }

        public static void DrawImage(int resId, XRect rect, XBrush color, XScaleType scaleType)
        {
            _draw?.DrawImage(resId, rect, color, scaleType);
        }

        public static void DrawSvg(int resId, XRect rect, XBrush color)
        {
            _draw?.DrawSvg(resId, rect, color);
        }

        public static object GetSvg(string svgContent)
        {
            lock (_lock)
            {
                return _draw?.GetSvg(svgContent);
            }
        }

        public static XBitmap GetBitmap(string base64, bool hasBuffer = false)
        {
            return _draw?.GetBitmap(base64, hasBuffer);
        }

        public static void DrawImage(byte[] images, XRect rect, XBrush color, XScaleType scaleType)
        {
            _draw?.DrawImage(images, rect, color, scaleType);
        }

        public static XRect MeasureText(string text, XFont font)
        {
            return _draw?.MeasureText(text, font) ?? new XRect();
        }

        public static XRect MeasureText(char text, XFont font)
        {
            return _draw?.MeasureText(text.ToString(), font) ?? new XRect();
        }

        public static void Invalidate(XView view)
        {
            view?.RefreshParentCache();
            if (view != null)
            {
                view.DrawCache.IsRefreshCache = true;
            }
            if (view == null)
            {
                _draw?.RefreshCache(true);
            }
            _window?.Invalidate();
            _draw?.RefreshCache(false);
        }

        public static void Invalidate()
        {
            Post(() =>
            {
                _draw?.RefreshCache(true);
                _window?.Invalidate();
                _draw?.RefreshCache(false);
            });
        }

        public static void ChangedImmPosition(XPoint point)
        {
            _window?.UpdateImmPosition(point);
        }

        public static void Post(Action function)
        {
            _window?.Post(function);
        }

        public static void PostToQueue(Action function)
        {
            _window?.PostToQueue(function);
        }

        public static void PostToRender(Action action)
        {
            _window?.PostToRender(action);
        }

        public static void SetCursor(XAlignment align)
        {
            switch (align)
            {
                case XAlignment.RightCenter:
                case XAlignment.LeftCenter:
                    _window?.SetCursor(XCursorType.HResize);
                    break;
                case XAlignment.TopCenter:
                case XAlignment.BottomCenter:
                    _window?.SetCursor(XCursorType.VResize);
                    break;
                case XAlignment.LeftTop:
                case XAlignment.RightBottom:
                    _window?.SetCursor(XCursorType.AllResize);
                    break;
                case XAlignment.RightTop:
                case XAlignment.LeftBottom:
                    _window?.SetCursor(XCursorType.AllResize);
                    break;
                default:
                    _window?.SetCursor(XCursorType.Arrow);
                    break;
            }
        }

        public static void SetCursor(XCursorType type)
        {
            _window?.SetCursor(type);
        }

        public static object GetCanvas()
        {
            return _draw?.GetCanvas();
        }

        public static void DrawArc(XRect rect, XStyle style, float startAngle, float sweepAngle, bool userCenter = false)
        {
            _draw?.DrawArc(rect, style, startAngle, sweepAngle, userCenter);
        }

        public static void DrawPath(XRect rect, XStyle style, bool isCache, Action content)
        {
            _draw?.DrawPath(rect, style, isCache, content);
        }

        public static void MoveTo(int x,int y)
        {
            _draw?.MoveTo(x,y);
        }
        
        public static void LineTo(int x, int y)
        {
            _draw?.LineTo(x, y);
        }

        public static void CubicTo(XPoint point1, XPoint point2, XPoint point3)
        {
            _draw?.CubicTo(point1, point2, point3);
        }
        
        public static void ArcTo(int x,int y, int radius)
        {
            _draw?.ArcTo(x, y, radius);
        }

        public static void AddRect(XRect rect, XColor[] colors, XGradientDirection direction)
        {
            _draw?.AddRect(rect, colors, direction);
        }
    }
}
