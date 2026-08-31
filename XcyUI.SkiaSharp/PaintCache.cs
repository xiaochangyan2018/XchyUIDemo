using SkiaSharp;
using System;
using System.Drawing;
using System.Net.NetworkInformation;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;

namespace XcyUI.SkiaSharp
{
    public class PaintCache
    {
        // 缓存
        private readonly static LinkedHashMap<int, SKPaint> Cache = new LinkedHashMap<int, SKPaint>();
        private readonly static LinkedHashMap<int, SKImageFilter> ImageFilter = new LinkedHashMap<int, SKImageFilter>();
        private readonly static LinkedHashMap<int, SKPathEffect> pathEffect = new LinkedHashMap<int, SKPathEffect>();

        private readonly static SKPaint gradientPaint = new SKPaint();
        private readonly static SKPaint gradientBorderPaint = new SKPaint();
        private readonly static SKPaint shadowPaint = new SKPaint();

        public static SKPaint GetBackgoundPaint(XBrush background)
        {
            var key = background.StartColor.GetHashCode();
            lock (Cache)
            {
                if (!Cache.Map.TryGetValue(key, out SKPaint paint))
                {
                    paint = new SKPaint();
                    paint.IsAntialias = true;
                    paint.Color = background.StartColor.ToSKColor();
                    Cache[key] = paint;

                }
                return paint;
            }
        }

        public static SKPaint GetBorderPaint(XBrush background, float size)
        {
            var key = (background.StartColor.Value, background.EndColor.Value, size).GetHashCode();
            lock (Cache)
            {
                if (!Cache.Map.TryGetValue(key, out SKPaint paint))
                {
                    paint = new SKPaint();
                    paint.IsAntialias = true;
                    paint.Color = background.StartColor.ToSKColor();
                    paint.IsStroke = true;
                    paint.StrokeCap = SKStrokeCap.Round;
                    paint.StrokeWidth = size;
                    Cache[key] = paint;
                }
                return paint;
            }
        }

        public static SKPaint GetGradientPaint(XRect rect, XBrush backgound)
        {
            gradientPaint.IsAntialias = true;
            gradientPaint.Color = backgound.StartColor.ToSKColor();
            gradientPaint.Shader = rect.ToShader(backgound);
            gradientPaint.BlendMode = SKBlendMode.SrcOver;
            return gradientPaint;
        }

        public static SKPaint GetGradientBorderPaint(XRect rect, XBrush backgound, float size)
        {
            gradientBorderPaint.IsAntialias = true;
            gradientBorderPaint.Color = backgound.StartColor.ToSKColor();
            gradientBorderPaint.IsStroke = true;
            gradientBorderPaint.StrokeWidth = size;
            gradientBorderPaint.Shader = rect.ToShader(backgound);
            gradientBorderPaint.PathEffect = null;
            return gradientBorderPaint;
        }

        public static SKPaint GetShadowPaint(XBrush backgound, XShadow shadow)
        {
            shadowPaint.IsAntialias = true;
            shadowPaint.Color = backgound.StartColor.ToSKColor();
            shadowPaint.ImageFilter = GetImageFilter(shadow);
            return shadowPaint;
        }

        public static SKImageFilter GetImageFilter(XShadow shadow)
        {
            var key = shadow.ShadowHashCode();
            if (!ImageFilter.Map.TryGetValue(key, out SKImageFilter imageFilter))
            {
                imageFilter = shadow.ToImageFilter();
                ImageFilter[key] = imageFilter;
            }
            return imageFilter;
        }

        private static XRect ShadowRect(XShadow shadow, XRect rect)
        {
            var tempRect = rect;
            var dx = Math.Abs(shadow.Dx);
            var dy = Math.Abs(shadow.Dy);
            tempRect.Scale(dx, dy, shadow.Blur + dx, shadow.Blur + dy);
            tempRect.Scale(shadow.Blur * 2);
            return tempRect;
        }

        internal static void DrawShadowRect(SKCanvas canvas, XShadow shadow, XRect rect, SKPath path)
        {
            var tempRect = ShadowRect(shadow, rect);
            var bitmap = GetBitmapForShdow(shadow, rect, path);
            canvas.DrawBitmap(bitmap, tempRect.ToSKRect());
        }
        internal static SKBitmap GetBitmapForShdow(XShadow shadow, XRect rect, SKPath path)
        {
            var key = $"{shadow.ShadowHashCode()}_{rect.Width}_{rect.Height}";
            if (!XTheme.Images.Map.TryGetValue(key, out object image))
            {
                var tempRect = ShadowRect(shadow, rect);
                var bitmap = new SKBitmap(tempRect.Width, tempRect.Height);
                using (var offscreenCanvas = new SKCanvas(bitmap))
                using (var paint = new SKPaint())
                {
                    paint.Color = SKColors.White;
                    paint.IsAntialias = true;
                    paint.ImageFilter = GetImageFilter(shadow);
                    offscreenCanvas.Translate(-tempRect.X, -tempRect.Y);
                    offscreenCanvas.DrawPath(path, paint);
                }
                image = bitmap;
                XTheme.Images[key] = bitmap;
            }
            return image as SKBitmap;
        }

        public static SKPathEffect GetPathEffect(XDashType type)
        {
            var key = type.GetHashCode();
            if (!pathEffect.Map.TryGetValue(key, out SKPathEffect effect))
            {
                effect = type.ToPathEffect();
                pathEffect[key] = effect;
            }
            return effect;
        }
    }
}
