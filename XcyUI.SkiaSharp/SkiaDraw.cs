using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.views;
namespace XcyUI.SkiaSharp
{
    public class SkiaDraw : IDraw
    {
        public SKCanvas Canvas { get; set; }
        public SKSurface Surface { get; set; }
        
        private SKPaint animationPain = new SKPaint();
        private SKFont skFont;
        private SKPath skPath = new SKPath();
        private SKPaint pictruePaint = new SKPaint();
        private SKPaint bitmapPaint = new SKPaint();
        private SKPaint debugPaint = null;
        private bool isRefreshCache = false;
        private SKTextBlobBuilder blobBuilder = new SKTextBlobBuilder();
        private static readonly Regex ViewBoxRegex = new Regex(@"viewBox=""([^""]+)""", RegexOptions.Compiled);
        private static readonly Regex PathRegex = new Regex(@"d=""([^""]+)""", RegexOptions.Compiled);
        public SkiaDraw()
        {
            animationPain.IsAntialias = true;
            pictruePaint.IsAntialias = true;            
            skFont = new SKFont()
            {
                Subpixel = true,
                LinearMetrics = true,
                Hinting = SKFontHinting.Full,
                Edging = SKFontEdging.SubpixelAntialias
            };
        }

        public void DrawCache(XRect rect, XStyle style, XDrawCache cache, Action onDraw)
        {
            if (cache.CacheType == XCacheType.Bitmap)
            {
                DrawRectBitmap(rect, style, cache, onDraw);
            }
            else
            {
                DrawRectPictrue(rect, style, cache, onDraw);
            }
        }

        public void DrawRect(XRect rect, XStyle style, Action onDraw)
        {
            DrawBaseRect(rect, style, null, onDraw);
        }

        private void DrawRectBitmap(XRect rect, XStyle style, XDrawCache cache, Action onDraw)
        {
            if (Canvas == null) return;
            var tempRect = rect;
            if (!style.Shadow.IsEmpty)
            {
                var dx = Math.Abs(style.Shadow.Dx);
                var dy = Math.Abs(style.Shadow.Dy);
                tempRect.Scale(dx,dy, style.Shadow.Blur + dx, style.Shadow.Blur + dy);
                tempRect.Scale(5);
            }
            if (cache.CacheData == null || cache.IsRefreshCache || isRefreshCache)
            {
                SKBitmap cacheBitmap = null;
                if(cache.CacheData is SKBitmap)
                {
                    cacheBitmap = (SKBitmap)cache.CacheData;
                }
                cache.CacheData = GetDrawRectBitmap(cacheBitmap, tempRect, rect, style, cache, onDraw);
                cache.IsRefreshCache = false;
            }
            Canvas.Save();
            bitmapPaint.Reset();
            var bitmap = (SKBitmap)cache.CacheData;
            var skRect = tempRect.ToSKRect();
            SetCanvsStyle(Canvas, cache, rect, skRect);
            if (cache.Alpha != -1)
            {
                var color = XColors.Black.Copy(cache.Alpha).ToSKColor();
                bitmapPaint.Color = color;
            }
            Canvas.DrawBitmap(bitmap, skRect, bitmapPaint);
            Canvas.Restore();
        }
        private SKBitmap GetDrawRectBitmap(SKBitmap bitmap, XRect bitmapRect, XRect rect, XStyle style,XDrawCache cache, Action onDraw)
        {
            if (bitmap == null || bitmap.Width != bitmapRect.Width || bitmap.Height != bitmapRect.Height)
            {
                bitmap?.Dispose();
                bitmap = new SKBitmap(bitmapRect.Width, bitmapRect.Height);
            }
            using (var offscreenCanvas = new SKCanvas(bitmap))
            {
                offscreenCanvas.Clear(SKColors.Transparent);
                var tempCanvas = Canvas;
                Canvas = offscreenCanvas;
                offscreenCanvas.Translate(-bitmapRect.X, -bitmapRect.Y);
                DrawBaseRect(rect, style,cache, onDraw);
                Canvas = tempCanvas;
            }
            return bitmap;
        }

        private void DrawRectPictrue(XRect rect, XStyle style, XDrawCache cache, Action onDraw)
        {
            if (Canvas == null) return;
            var tempRect = rect;
            if (isRefreshCache || cache.CacheData == null || cache.IsRefreshCache)
            {
                SKPicture picture = null;
                if (cache.CacheData is SKPicture)
                {
                    picture = (SKPicture)cache.CacheData;
                }
                cache.CacheData = GetDrawRectPicture(picture, tempRect, rect, style, cache, onDraw);
                cache.IsRefreshCache = false;
                if (style.IsOnlySet) return;
            }
            var pictrue = (SKPicture)cache.CacheData;
            Canvas.Save();
            SetCanvsStyle(Canvas, cache, rect, pictrue.CullRect);
            // 设置阴影
            if (!style.Shadow.IsEmpty)
            {
                pictruePaint.ImageFilter = PaintCache.GetImageFilter(style.Shadow);
            }
            else
            {
                pictruePaint.ImageFilter = null;
            }
            pictruePaint.Color = style.Background.StartColor.ToSKColor();
            if (cache.Alpha != -1)
            {
                var color = XTheme.Color.Black.Copy(cache.Alpha).ToSKColor();
                pictruePaint.Color = color;
                Canvas.DrawPicture(pictrue, pictruePaint);
            }
            else if(!style.Shadow.IsEmpty)
            {
                Canvas.DrawPicture(pictrue, pictruePaint);
            }
            else if (cache.BlurSigma > 0)
            {
                pictruePaint.ImageFilter = SKImageFilter.CreateBlur(sigmaX: cache.BlurSigma, sigmaY: cache.BlurSigma);
                pictruePaint.Color = style.Background.StartColor.ToSKColor().WithAlpha(128);
                Canvas.DrawPicture(pictrue, pictruePaint);
            }
            else
            {
                Canvas.DrawPicture(pictrue);
            }
            Canvas.Restore();
        }

        private SKPicture GetDrawRectPicture(SKPicture picture, XRect bitmapRect, XRect rect, XStyle style,XDrawCache cache, Action onDraw)
        {
            picture?.Dispose();
            using (var recorder = new SKPictureRecorder())
            {
                var pictureCanvas = recorder.BeginRecording(bitmapRect.ToSKRect());
                var tempCanvas = Canvas;
                Canvas = pictureCanvas;
                DrawBaseRect(rect, style, cache, onDraw);
                picture = recorder.EndRecording();
                Canvas = tempCanvas;
                return picture;
            }
        }

        private void SetCanvsStyle(SKCanvas canvas, XDrawCache cache,XRect rect,SKRect srcRect)
        {
            var degreesPoint = cache.DegreesPoint;
            if (degreesPoint.Equals(XPoint.Empty))
            {
                degreesPoint = rect.Center;
            }
            if (cache.Degrees != -1)
            {
                canvas.Translate(degreesPoint.X, degreesPoint.Y);
                canvas.RotateDegrees(cache.Degrees);
                canvas.Translate(-degreesPoint.X, -degreesPoint.Y);
            }
            var scalePoint = cache.ScalePoint;
            if (scalePoint.Equals(XPoint.Empty))
            {
                scalePoint = rect.Center;
            }
            if (cache.ScaleX != -1 && cache.ScaleY != -1)
            {
                canvas.Translate(scalePoint.X, scalePoint.Y);
                canvas.Scale(cache.ScaleX, cache.ScaleY);
                canvas.Translate(-scalePoint.X, -scalePoint.Y);
            }
            else if (cache.ScaleX != -1)
            {
                canvas.Translate(scalePoint.X, scalePoint.Y);
                canvas.Scale(cache.ScaleX, 1);
                canvas.Translate(-scalePoint.X, -scalePoint.Y);
            }
            else if(cache.ScaleY !=-1)
            {
                canvas.Translate(scalePoint.X, scalePoint.Y);
                canvas.Scale(1, cache.ScaleY);
                canvas.Translate(-scalePoint.X, -scalePoint.Y);
            }

            if (cache.TranslateX != -1 && cache.TranslateY != -1)
            {
                canvas.Translate(cache.TranslateX, cache.TranslateY);
            }
            else if (cache.TranslateX != -1)
            {
                canvas.Translate(cache.TranslateX, 0);
            }
            else if (cache.TranslateY != -1)
            {
                canvas.Translate(0, cache.TranslateY);
            }
            else
            {
                canvas.Translate(rect.X - srcRect.Left, rect.Y - srcRect.Top);
            }
        }
        private void FillPaint(SKRect rect, XStyle style, SKPaint paint)
        {
            // 添加矩形
            var isCircular = style.Radius.All == 0.5f;
            if (isCircular)
            {
                var radius = rect.Width > rect.Height ? rect.Height / 2 : rect.Width / 2;
                Canvas.DrawCircle(rect.MidX, rect.MidY, radius, paint);
            }
            else if (style.Radius.HasSize)
            {
                Canvas.DrawRoundRect(rect.ToSkRoundRect(style.Radius), paint);
            }
            else
            {
                Canvas.DrawRect(rect, paint);
            }
        }
        private void AddPath(SKRect rect, XStyle style)
        {
            // 添加矩形
            skPath.Reset();
            var isCircular = style.Radius.All == 0.5f;
            if (isCircular)
            {
                var radius = rect.Width > rect.Height ? rect.Height / 2 : rect.Width / 2;
                skPath.AddCircle(rect.MidX, rect.MidY, radius);
            }
            else if (style.Radius.HasSize)
            {
                skPath.AddRoundRect(rect.ToSkRoundRect(style.Radius));
            }
            else
            {
                skPath.AddRect(rect);
            }
        }

        private void Clip(SKRect rect, XStyle style)
        {
            // 添加矩形
            if (style.Radius.HasSize)
            {
                var isCircular = style.Radius.All == 0.5f;
                var radius = style.Radius;
                if (isCircular)
                {
                    radius = new XSpace(Math.Min(rect.Width, rect.Height) / 2);
                }
                Canvas.ClipRoundRect(rect.ToSkRoundRect(style.Radius), SKClipOperation.Intersect, true);
            }
            else
            {
                Canvas.ClipRect(rect, SKClipOperation.Intersect, true);
            }
        }

        public void DrawBaseRect(XRect rect, XStyle style, XDrawCache cache, Action onDraw)
        {
            if (Canvas == null) return;
            if (style.Background.IsEmpty && style.Border.Color.IsEmpty && !style.IsClip)
            {
                onDraw?.Invoke();
                return;
            }
            var cRect = rect.ToSKRect();
            Canvas.Save();
            if (!style.IsOverDraw)
            {
                var brush = style.Background;
                var paint = PaintCache.GetBackgoundPaint(style.Background);
                if (!brush.StartColor.IsEmpty && style.Background.EndColor.IsEmpty)
                {
                    // 设置阴影
                    if (!style.Shadow.IsEmpty && cache != null && !cache.EnableCache && cache.CacheShadow)
                    {
                        PaintCache.DrawShadowRect(Canvas, style.Shadow, rect, skPath);
                    }
                    else if (!style.Shadow.IsEmpty && (cache == null || !cache.EnableCache || cache.CacheType == XCacheType.Bitmap))
                    {
                        paint = PaintCache.GetShadowPaint(brush, style.Shadow);
                        paint.ImageFilter = PaintCache.GetImageFilter(style.Shadow);
                    }
                    FillPaint(cRect, style, paint);
                }
                else if (!brush.StartColor.IsEmpty && !brush.EndColor.IsEmpty)
                {
                    // 设置阴影
                    if (!style.Shadow.IsEmpty && cache != null && !cache.EnableCache && cache.CacheShadow)
                    {
                        PaintCache.DrawShadowRect(Canvas, style.Shadow, rect, skPath);
                    }
                    else if (!style.Shadow.IsEmpty && (cache == null || !cache.EnableCache || cache.CacheType == XCacheType.Bitmap))
                    {
                        paint = PaintCache.GetShadowPaint(brush, style.Shadow);
                        paint.ImageFilter = PaintCache.GetImageFilter(style.Shadow);
                        FillPaint(cRect, style, paint);
                    }
                    paint = PaintCache.GetGradientPaint(rect, brush);
                    FillPaint(cRect, style, paint);
                }
            }
            Canvas.Save();
            if (style.IsClip)
            {
                Clip(cRect, style);
                if (style.IsClipPadding && style.ClipPadding.HasSize)
                {
                    rect.ScaleRevert(style.ClipPadding);
                    rect.Scale(2);
                    Canvas.ClipRect(rect.ToSKRect(), SKClipOperation.Intersect, true);
                }
            }
            onDraw?.Invoke();
            Canvas.Restore();

            if (!style.IsOverDraw)
            {
                // 添加边框
                var border = style.Border;
                if (border.Size.HasSize && !border.Color.IsEmpty)
                {
                    AddPath(cRect, style);
                    var paint = PaintCache.GetBorderPaint(border.Color, border.Size.All);
                    // 设置边框渐变
                    if (!border.Color.EndColor.IsEmpty)
                    {
                        paint = PaintCache.GetGradientBorderPaint(rect, border.Color, border.Size.All);
                    }
                    paint.PathEffect = PaintCache.GetPathEffect(border.DashType);
                    if (border.Size.All == 0)
                    {
                        DrawHelper.DrawBorder(Canvas, paint, cRect, border.Size);
                    }
                    else
                    {
                        FillPaint(cRect, style, paint);
                    }
                }
            }
            if (XTheme.EnableDebugRect)
            {
                if (debugPaint == null)
                {
                    debugPaint = new SKPaint
                    {
                        Color = SKColors.Red,
                        IsAntialias = true,
                        StrokeWidth = 0.5f,
                        IsStroke = true
                    };
                }
                Canvas.DrawRect(cRect, debugPaint);
            }
            Canvas.Restore();
        }
        
        public void DrawText(List<XChar> chars)
        {
            if (Canvas == null || chars == null || chars.Count == 0) return;
            var font = chars[0].Font;
            var count = chars.Count;
            skFont.Size = font.Size;
            skFont.Typeface = font.ToSKTypeface();
            var paint = PaintCache.GetBackgoundPaint(font.Color);
            var posRunBuffer = blobBuilder.AllocatePositionedRun(skFont, count);
            var lineStart = XPoint.Empty;
            var lineEnd = XPoint.Empty;
            for (int i = 0; i < count; i++)
            {
                var c = chars[i];
                var p = new SKPoint(c.X, c.Y + font.LineHeight / 2 - (skFont.Metrics.Top + skFont.Metrics.Bottom) / 2);
                posRunBuffer.Glyphs[i] = skFont.Typeface.GetGlyph(c.Value);
                if (c.IsNewLine)
                {
                    posRunBuffer.Glyphs[i] = skFont.Typeface.GetGlyph(' ');
                }
                posRunBuffer.Positions[i] = p;
                if (font.Underline || font.DeleteLine)
                {
                    var x = c.X;
                    var y = c.Y + (font.Underline ? font.LineHeight : font.LineHeight / 2);
                    if (!lineEnd.Equals(XPoint.Empty) && y != lineEnd.Y)
                    {
                        Canvas.DrawLine(lineStart.ToSKPoint(), lineEnd.ToSKPoint(), paint);
                    }
                    if (lineStart.Equals(XPoint.Empty))
                    {
                        lineStart.X = x;
                        lineStart.Y = y;
                    }
                    lineEnd.X = c.X + c.Width;
                    lineEnd.Y = y;
                    if (i == chars.Count - 1)
                    {
                        Canvas.DrawLine(lineStart.ToSKPoint(), lineEnd.ToSKPoint(), paint);
                    }
                }
            }
            var textBlob = blobBuilder.Build();
            Canvas.DrawText(textBlob, 0, 0, paint);
            //textBlob.Dispose();
        }

        public XRect MeasureText(string text, XFont font)
        {
            if (Canvas == null) new XRect();
            skFont.Size = font.Size;
            skFont.Typeface = font.ToSKTypeface();
            var bounds = new SKRect();
            float width = skFont.MeasureText(text, out bounds);
            var height = skFont.Metrics.Descent - skFont.Metrics.Ascent + skFont.Metrics.Leading;
            font.LineHeight = (int)height;
            return new XRect(0, 0, (int)width, (int)bounds.Height);
        }

        public void DrawImage(SKBitmap bitmap, XRect rect, XBrush color, XScaleType scaleType)
        {
            if (Canvas == null) return;
            var paint = PaintCache.GetBackgoundPaint(color);
            if (!color.IsEmpty)
            {
                var skColor = color.StartColor.ToSKColor();
                paint.ColorFilter = SKColorFilter.CreateBlendMode(skColor, SKBlendMode.SrcIn);
            }
            if (!color.EndColor.IsEmpty)
            {
                paint.Shader = DrawHelper.ToShader(rect, color);
            }
            SKRect destRect = rect.ToSKRect();
            float scale, x, y;
            switch (scaleType)
            {
                case XScaleType.Normal:
                    scale = Math.Min((float)rect.Width / bitmap.Width, (float)rect.Height / bitmap.Height);
                    x = rect.X + (rect.Width - scale * bitmap.Width) / 2;
                    y = rect.Y + (rect.Height - scale * bitmap.Height) / 2;
                    destRect = new SKRect(x, y, x + scale * bitmap.Width,
                                                       y + scale * bitmap.Height);
                    break;
                case XScaleType.FixCenter:
                    scale = Math.Max((float)rect.Width / bitmap.Width, (float)rect.Height / bitmap.Height);
                    x = rect.X + (rect.Width - scale * bitmap.Width) / 2;
                    y = rect.Y + (rect.Height - scale * bitmap.Height) / 2;
                    destRect = new SKRect(x, y, x + scale * bitmap.Width,
                                                       y + scale * bitmap.Height);
                    break;
            }
            if (!color.IsEmpty)
            {
                Canvas?.DrawBitmap(bitmap, destRect, paint);
            }
            else
            {
                Canvas?.DrawBitmap(bitmap, destRect);
            }
        }

        public void DrawImage(int resId, XRect rect, XBrush color, XScaleType scaleType)
        {
            var skRect = rect.ToSKRect();
            if(XTheme.ImgResources.ContainsKey(resId) && XTheme.ImgResources[resId] is XBitmap)
            {
                var xBitmap = (XBitmap)XTheme.ImgResources[resId];
                var bitmap = (SKBitmap)xBitmap.Cache;
                DrawImage(bitmap, rect, color, scaleType);
            }
        }

        public void DrawImage(byte[] images, XRect rect, XBrush color, XScaleType scaleType)
        {
            SKBitmap bitmap = SKBitmap.Decode(images);
            DrawImage(bitmap, rect, color, scaleType);
        }

        public void DrawSvg(int resId, XRect rect, XBrush color)
        {
            var name = resId.ToString();
            if (XTheme.SvgResources.ContainsKey(resId) && XTheme.SvgResources[resId] is SKPicture)
            {
                var picture = (SKPicture)XTheme.SvgResources[resId];
                var skRect = rect.ToSKRect();
                float scaleX = skRect.Width / picture.CullRect.Width;
                float scaleY = skRect.Height / picture.CullRect.Height;
                var matrix = SKMatrix.CreateScale(scaleX, scaleY);
                matrix.TransX = skRect.Left;
                matrix.TransY = skRect.Top;
                var paint = PaintCache.GetBackgoundPaint(color);
                if (!color.StartColor.IsEmpty && color.EndColor.IsEmpty)
                {
                    var skColor = color.StartColor.ToSKColor();
                    paint.ColorFilter = SKColorFilter.CreateBlendMode(skColor, SKBlendMode.SrcIn);
                    Canvas?.DrawPicture(picture, in matrix, paint);
                }
                else if (!color.IsEmpty)
                {
                    paint = PaintCache.GetGradientPaint(rect, color);
                    Canvas.SaveLayer(skRect, paint);
                    Canvas.DrawPicture(picture, in matrix);
                    paint.BlendMode = SKBlendMode.SrcIn;
                    Canvas.DrawRect(skRect, paint);
                    Canvas.Restore();
                }
            }
        }
        public object GetSvg(string svgContent)
        {
            var viewBoxMatch = ViewBoxRegex.Match(svgContent);
            SKRect bounds = new SKRect(0, 0, 1024, 1024);
            if (viewBoxMatch.Success)
            {
                string[] dims = viewBoxMatch.Groups[1].Value.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (dims.Length >= 4 &&
                    float.TryParse(dims[0], out float x) &&
                    float.TryParse(dims[1], out float y) &&
                    float.TryParse(dims[2], out float w) &&
                    float.TryParse(dims[3], out float h))
                {
                    bounds = new SKRect(x, y, x + w, y + h);
                }
            }
            var pathMatches = PathRegex.Matches(svgContent);
            if (pathMatches.Count == 0) return null;
            using (var recorder = new SKPictureRecorder())
            {
                SKCanvas canvas = recorder.BeginRecording(bounds);
                using(var mergedPath = new SKPath())
                {
                    foreach (Match match in pathMatches)
                    {
                        string pathData = match.Groups[1].Value;
                        using (var path = SKPath.ParseSvgPathData(pathData))
                        {
                            if (path != null)
                            {
                                mergedPath.AddPath(path);
                            }
                        }
                    }
                    var paint = PaintCache.GetBackgoundPaint(new XBrush() { StartColor = XColors.White });
                    canvas.DrawPath(mergedPath, paint);
                }
                return recorder.EndRecording();
            }
        }

        public XBitmap GetBitmap(string base64, bool hasBuffer)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            using (var stream = new MemoryStream(imageBytes))
            {
                var skBitmap =  SKBitmap.Decode(stream);
                int w = skBitmap.Width;
                int h = skBitmap.Height;
                int byteCount = w * h * 4;
                byte[] buffer = null;
                if (hasBuffer)
                {
                   buffer = new byte[byteCount];
                    Marshal.Copy(skBitmap.GetPixels(), buffer, 0, byteCount);
                }
                return new XBitmap()
                {
                    Width = w,
                    Height = h,
                    Buffers = buffer,
                    Cache = skBitmap
                };
            }
        }

        public void RefreshCache(bool isRefresh)
        {
            isRefreshCache = isRefresh;
        }

        public object GetCanvas()
        {
            return Canvas;
        }

        public void DrawArc(XRect rect, XStyle style, float startAngle, float sweepAngle, bool userCenter)
        {
            var paint = PaintCache.GetBackgoundPaint(style.Background);
            paint.Color = style.Background.StartColor.ToSKColor();
            var size = Math.Min(rect.Width, rect.Height);
            rect = new XRect(rect.Center.X - size / 2, rect.Center.Y - size / 2, size, size);
            if (!style.Background.EndColor.IsEmpty)
            {
                paint = PaintCache.GetGradientPaint(rect, style.Background);
            }
            Canvas?.DrawArc(rect.ToSKRect(), startAngle, sweepAngle, userCenter, paint);
            if (style.Border.Size.HasSize)
            {
                paint = PaintCache.GetBorderPaint(style.Border.Color, style.Border.Size.Left);
                if (!style.Border.Color.EndColor.IsEmpty)
                {
                    paint = PaintCache.GetGradientBorderPaint(rect, style.Background, style.Border.Size.Left);
                }
                Canvas?.DrawArc(rect.ToSKRect(), startAngle, sweepAngle, userCenter, paint);
            }
        }

        public void DrawPath(XRect rect, XStyle style, bool isCache, Action content)
        {
            if (Canvas == null) return;
            Canvas.Save();
            skPath.Reset();
            content();
            var paint = PaintCache.GetBackgoundPaint(style.Background);
            if (!style.Background.StartColor.IsEmpty && style.Background.EndColor.IsEmpty)
            {
                if (!style.Shadow.IsEmpty && !isCache)
                {
                    paint = PaintCache.GetShadowPaint(style.Background, style.Shadow);
                    paint.ImageFilter = PaintCache.GetImageFilter(style.Shadow);
                }
                Canvas.DrawPath(skPath, paint);
            }
            else if(!style.Background.StartColor.IsEmpty && !style.Background.EndColor.IsEmpty)
            {
                if (!style.Shadow.IsEmpty && !isCache)
                {
                    paint = PaintCache.GetShadowPaint(style.Background, style.Shadow);
                    paint.ImageFilter = PaintCache.GetImageFilter(style.Shadow);
                    Canvas.DrawPath(skPath, paint);
                }
                paint = PaintCache.GetGradientPaint(rect, style.Background);
                Canvas.DrawPath(skPath, paint);
            }
            if (style.Border.Size.HasSize && !style.Border.Color.IsEmpty)
            {
                paint = PaintCache.GetBorderPaint(style.Border.Color, style.Border.Size.All);
                if (!style.Border.Color.EndColor.IsEmpty)
                {
                    paint.Shader = DrawHelper.ToShader(rect, style.Border.Color);
                }
                Canvas.DrawPath(skPath, paint);
            }
            Canvas.Restore();
        }


        public void MoveTo(int x,int y)
        {
            skPath?.MoveTo(x, y);
        }
        public void LineTo(int x, int y)
        {
            skPath?.LineTo(x, y);
        }

        public void ArcTo(int x,int y, int radius)
        {
            skPath?.ArcTo(radius, radius, 0, SKPathArcSize.Small, SKPathDirection.Clockwise, x, y);
        }

        public void CubicTo(XPoint point1, XPoint point2, XPoint point3)
        {
            skPath?.CubicTo(point1.ToSKPoint(), point2.ToSKPoint(), point3.ToSKPoint());
        }

        public void AddRect(XRect rect, XColor[] colors, XGradientDirection direction)
        {
            skPath?.AddRect(rect.ToSKRect());
            var paint = PaintCache.GetGradientPaint(rect, new XBrush() { StartColor = colors[0] });
            var color = paint.Color;
            if (paint.Color.Alpha == 0)
            {
                paint.Color = SKColors.White;
            }
            paint.Shader = rect.GetShader(colors, direction);
            Canvas?.DrawPath(skPath, paint);
            skPath?.Reset();
        }
    }
}
