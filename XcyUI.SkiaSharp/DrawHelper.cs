using SkiaSharp;
using System;
using System.Collections.Generic;
using XcyUI.models;
using XcyUI.theme;

namespace XcyUI.SkiaSharp
{
    public static class DrawHelper
    {
        /// <summary>
        /// 缓存的字体信息
        /// </summary>
        public static Dictionary<int, SKTypeface> Typefaces = new Dictionary<int, SKTypeface>();

        /// <summary>
        /// 矩形转圆角
        /// </summary>
        /// <returns></returns>
        public static SKRoundRect ToSkRoundRect(this SKRect rect, XSpace radius)
        {
            SKRoundRect round = new SKRoundRect(rect, radius.All);
            if (!radius.IsFullSize)
            {
                round.SetRectRadii(
                rect,
                [
                    new SKPoint(radius.Left, radius.Left), // 左上角
                    new SKPoint(radius.Top, radius.Top), // 右上角
                    new SKPoint(radius.Right, radius.Right), // 右下角
                    new SKPoint(radius.Bottom, radius.Bottom),  // 左下角
                ]);
            }
            return round;
        }

        public static SKPath ToSkLargeRoundRect(this SKRect rect, XSpace radiusSpace, int smoothRadius)
        {
            var path = new SKPath();

            // 贝塞尔曲线控制点系数 (0.5522847498f)
            const float k = 0.5522848f;
            var radius = radiusSpace.Left;
            var kappa = 0f;
            if (radius > 0)
            {
                smoothRadius = Math.Min((int)radius / 2, smoothRadius);
                kappa = radius - smoothRadius * k;
                radius += smoothRadius;
            }
            path.MoveTo(rect.Left, rect.Top + radius);
            // 左上角
            if (radius > 0)
            {
                path.CubicTo(
                rect.Left, rect.Top + radius - kappa,
                rect.Left + radius - kappa, rect.Top,
                rect.Left + radius, rect.Top);
            }

            radius = radiusSpace.Top;
            if (radius > 0)
            {
                smoothRadius = Math.Min((int)radius / 2, smoothRadius);
                kappa = radius - smoothRadius * k;
                radius += smoothRadius;
            }

            // 上边
            path.LineTo(rect.Right - radius, rect.Top);
            // 右上角
            if (radius > 0)
            {
                path.CubicTo(
                rect.Right - radius + kappa, rect.Top,
                rect.Right, rect.Top + radius - kappa,
                rect.Right, rect.Top + radius);
            }

            radius = radiusSpace.Right;
            if (radius > 0)
            {
                smoothRadius = Math.Min((int)radius / 2, smoothRadius);
                kappa = radius - smoothRadius * k;
                radius += smoothRadius;
            }
            // 右边
            path.LineTo(rect.Right, rect.Bottom - radius);

            // 右下角
            if (radius > 0)
            {
                path.CubicTo(
                rect.Right, rect.Bottom - radius + kappa,
                rect.Right - radius + kappa, rect.Bottom,
                rect.Right - radius, rect.Bottom);
            }
            radius = radiusSpace.Bottom;
            if (radius > 0)
            {
                smoothRadius = Math.Min((int)radius / 2, smoothRadius);
                kappa = radius - smoothRadius * k;
                radius += smoothRadius;
            }
            // 下边
            path.LineTo(rect.Left + radius, rect.Bottom);

            // 左下角
            if (radius > 0)
            {
                path.CubicTo(
                rect.Left + radius - kappa, rect.Bottom,
                rect.Left, rect.Bottom - radius + kappa,
                rect.Left, rect.Bottom - radius);
            }

            path.Close();
            return path;
        }

        public static void DrawBorder(SKCanvas g, SKPaint paint, SKRect rect, XSpace border)
        {
            SKPoint start = rect.Location, end = rect.Location;
            if (border.Left > 0)
            {
                paint.StrokeWidth = border.Left;
                end.Offset(0, rect.Height);
                g.DrawLine(start, end, paint);
            }
            start = rect.Location;
            end = rect.Location;
            if (border.Top > 0)
            {
                paint.StrokeWidth = border.Top;
                end.Offset(rect.Width, 0);
                g.DrawLine(start, end, paint);
            }
            start = rect.Location;
            end = rect.Location;
            if (border.Right > 0)
            {
                paint.StrokeWidth = border.Right;
                start.Offset(rect.Width, 0);
                end.Offset(rect.Width, rect.Height);
                g.DrawLine(start, end, paint);
            }
            start = rect.Location;
            end = rect.Location;
            if (border.Bottom > 0)
            {
                paint.StrokeWidth = border.Bottom;
                start.Offset(0, rect.Height);
                end.Offset(rect.Width, rect.Height);
                g.DrawLine(start, end, paint);
            }
        }

        public static SKColor ToSKColor(this XColor color)
        {
            return new SKColor(color.Red, color.Green, color.Blue, color.Alpha);
        }

        public static SKRect ToSKRect(this XRect rect)
        {
            return SKRect.Create(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static SKPoint ToSKPoint(this XPoint point)
        {
            return new SKPoint(point.X, point.Y);
        }

        public static SKImageFilter ToImageFilter(this XShadow shadow)
        {
            return SKImageFilter.CreateDropShadow(shadow.Dx, shadow.Dy, shadow.Blur, shadow.Blur, shadow.Color.ToSKColor());
        }

        public static SKShader ToShader(this XRect rect, XBrush brush)
        {
            var startColor = brush.StartColor.ToSKColor();
            var endColor = brush.EndColor.ToSKColor();
            var colors = new SKColor[2] { startColor, endColor };
            var startPoint = rect.Point.ToSKPoint();
            var endPoint = new SKPoint(rect.Right, rect.Top);
            switch (brush.Direction)
            {
                case XGradientDirection.Vertical:
                    endPoint = new SKPoint(rect.Left, rect.Bottom);
                    break;
                case XGradientDirection.DiagonalBottom:
                    endPoint = new SKPoint(rect.Right, rect.Bottom);
                    break;
                case XGradientDirection.DiagonalTop:
                    startPoint = new SKPoint(rect.Left, rect.Bottom);
                    endPoint = new SKPoint(rect.Right, rect.Top);
                    break;
                case XGradientDirection.Round:
                    startPoint = rect.Center.ToSKPoint();
                    endPoint = startPoint;
                    break;
            }
            if (brush.Direction == XGradientDirection.Radial)
            {
                var centerPoint = rect.Center.ToSKPoint();
                var radius = (float)Math.Sqrt(rect.Width * rect.Width + rect.Height * rect.Height) / 2;
                return SKShader.CreateRadialGradient(centerPoint, radius, colors, null, SKShaderTileMode.Repeat);
            }
            else if (brush.Direction == XGradientDirection.Round)
            {
                float[] colorPositions = new[] { 0f, 1f };
                var radius = (float)Math.Sqrt(rect.Width * rect.Width + rect.Height * rect.Height) / 2;
                return SKShader.CreateSweepGradient(startPoint, colors, colorPositions);
            }
            else
            {
                return SKShader.CreateLinearGradient(startPoint, endPoint, colors, null, SKShaderTileMode.Repeat);
            }
        }

        public static SKShader GetShader(this XRect rect, XColor[] colors, XGradientDirection direction)
        {
            var startPoint = rect.Point.ToSKPoint();
            var endPoint = new SKPoint(rect.Right, rect.Top);
            var skColors = new SKColor[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                skColors[i] = colors[i].ToSKColor();
            }
            switch (direction)
            {
                case XGradientDirection.Vertical:
                    endPoint = new SKPoint(rect.Left, rect.Bottom);
                    break;
                case XGradientDirection.DiagonalBottom:
                    endPoint = new SKPoint(rect.Right, rect.Bottom);
                    break;
                case XGradientDirection.DiagonalTop:
                    startPoint = new SKPoint(rect.Left, rect.Bottom);
                    endPoint = new SKPoint(rect.Right, rect.Top);
                    break;
                case XGradientDirection.Round:
                    startPoint = rect.Center.ToSKPoint();
                    endPoint = startPoint;
                    break;
            }

            var shader = SKShader.CreateLinearGradient(
                startPoint, endPoint, skColors, null, SKShaderTileMode.Repeat);
            return shader;
        }

        public static SKTypeface ToSKTypeface(this XFont font)
        {
            if (string.IsNullOrEmpty(font.Name))
            {
                font.Name = "宋体";
            }
            var key = (font.Name, font.Weight, font.Italic).GetHashCode();
            if (!Typefaces.ContainsKey(key))
            {
                if (string.IsNullOrEmpty(font.Path))
                {

                    Typefaces[key] = SKTypeface.FromFamilyName(font.Name, new SKFontStyle((int)font.Weight, 6, font.Italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright));
                    if (Typefaces[key] == null)
                    {
                        Console.WriteLine("name:" + font.Name);
                        Typefaces[key] = SKTypeface.Default;
                    }
                }
                else
                {
                    Typefaces[key] = SKTypeface.FromFile(font.Path);
                }
            }
            return Typefaces[key];
        }

        public static SKPathEffect ToPathEffect(this XDashType type)
        {
            SKPathEffect pathEffect = null;
            switch (type)
            {
                case XDashType.Dash:
                    pathEffect = SKPathEffect.CreateDash(new float[2] { 12f, 6f }, 0);
                    break;
                case XDashType.Dot:
                    pathEffect = SKPathEffect.CreateDash(new
                        float[2] { 2f, 8f }, 0);
                    break;
                case XDashType.DashDot:
                    pathEffect = SKPathEffect.CreateDash(new
                        float[4] { 20f, 5f, 8f, 5f }, 0);
                    break;
            }

            return pathEffect;
        }
        
        /// <summary>
        /// Skia资源预热
        /// </summary>
        /// <param name="surface"></param>
        internal static void PreWarmSkia(SKSurface surface)
        {
            var warmupCanvas = surface.Canvas;
            var warmupPaint = new SKPaint();
            var warmupFont = new SKFont { Size = 32, Typeface = SKTypeface.Default };

            // 1. 清空画布
            warmupCanvas.Clear(SKColors.Transparent);

            // 2. 预热背景绘制（纯色和渐变）
            using (var bgPaint = new SKPaint())
            {
                // 纯色背景
                bgPaint.Color = SKColors.LightGray;
                warmupCanvas.DrawPaint(bgPaint);

                // 渐变背景
                bgPaint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(100, 100),
                    new SKColor[] { SKColors.Red, SKColors.Green, SKColors.Blue },
                    new float[] { 0, 0.5f, 1.0f },
                    SKShaderTileMode.Clamp);
                warmupCanvas.DrawPaint(bgPaint);
                bgPaint.Shader = null;
            }

            // 3. 预热边框（描边）
            using (var strokePaint = new SKPaint())
            {
                strokePaint.IsStroke = true;
                strokePaint.StrokeWidth = 2;
                strokePaint.Color = SKColors.Black;
                strokePaint.IsAntialias = true;

                // 矩形边框
                warmupCanvas.DrawRect(new SKRect(10, 10, 50, 50), strokePaint);

                // 圆角矩形边框
                warmupCanvas.DrawRoundRect(
                    new SKRoundRect(new SKRect(60, 10, 100, 50), 10, 10),
                    strokePaint);

                // 椭圆形边框
                warmupCanvas.DrawOval(150,30, 20, 15, strokePaint);
            }

            // 4. 预热填充（各种形状）
            using (var fillPaint = new SKPaint())
            {
                fillPaint.IsStroke = false;
                fillPaint.IsAntialias = true;

                // 纯色填充
                fillPaint.Color = SKColors.Blue;
                warmupCanvas.DrawRect(new SKRect(10, 60, 50, 100), fillPaint);

                // 渐变填充
                fillPaint.Shader = SKShader.CreateRadialGradient(
                    new SKPoint(80, 80), 30,
                    new SKColor[] { SKColors.Yellow, SKColors.Orange },
                    null,
                    SKShaderTileMode.Clamp);
                warmupCanvas.DrawCircle(new SKPoint(80, 80), 25, fillPaint);
                fillPaint.Shader = null;
            }

            // 5. 预热Path（复杂路径）
            using (var pathPaint = new SKPaint { IsAntialias = true })
            {
                // 空心Path
                pathPaint.IsStroke = true;
                pathPaint.StrokeWidth = 2;
                pathPaint.Color = SKColors.DarkBlue;

                var path = new SKPath();
                path.MoveTo(120, 60);
                path.CubicTo(140, 40, 160, 80, 180, 60);
                path.QuadTo(190, 40, 200, 60);
                path.Close();
                warmupCanvas.DrawPath(path, pathPaint);
                path.Dispose();

                // 实心Path（带渐变）
                pathPaint.IsStroke = false;
                pathPaint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(120, 60), new SKPoint(200, 100),
                    new SKColor[] { SKColors.Red, SKColors.Purple },
                    null, SKShaderTileMode.Clamp);

                path = new SKPath();
                path.AddPoly(new SKPoint[] {
                new SKPoint(120, 110),
                new SKPoint(140, 150),
                new SKPoint(120, 190),
                new SKPoint(160, 170),
                new SKPoint(200, 190),
                new SKPoint(180, 150),
                new SKPoint(200, 110)
            });
                warmupCanvas.DrawPath(path, pathPaint);
                path.Dispose();
                pathPaint.Shader = null;
            }

            // 6. 预热文本（各种字体样式）
            var builder = new SKTextBlobBuilder();
            using (var textPaint = new SKPaint { IsAntialias = true })
            {
                textPaint.Color = SKColors.Black;

                // 普通文本
                ushort[] glyphs = warmupFont.GetGlyphs("Warmup");
                var run = builder.AllocateHorizontalRun(warmupFont, glyphs.Length, 0);
                run.SetGlyphs(glyphs);
                var textBlob = builder.Build();
                warmupCanvas.DrawText(textBlob, 0, 0, warmupPaint);

                // 描边文本
                textPaint.IsStroke = true;
                textPaint.StrokeWidth = 1;
                textPaint.Color = SKColors.Red;
                glyphs = warmupFont.GetGlyphs("Stroke Text");
                run = builder.AllocateHorizontalRun(warmupFont, glyphs.Length, 0);
                run.SetGlyphs(glyphs);
                textBlob = builder.Build();
                warmupCanvas.DrawText(textBlob, 0, 0, warmupPaint);

                // 带阴影的文本
                textPaint.IsStroke = false;
                textPaint.Color = SKColors.Blue;
                textPaint.ImageFilter = SKImageFilter.CreateDropShadow(
                    3, 3, 3, 3, SKColors.Black.WithAlpha(0x80));
                glyphs = warmupFont.GetGlyphs("Shadow Text");
                run = builder.AllocateHorizontalRun(warmupFont, glyphs.Length, 0);
                run.SetGlyphs(glyphs);
                textBlob = builder.Build();
                warmupCanvas.DrawText(textBlob, 0, 0, warmupPaint);
                textPaint.ImageFilter = null;
            }

            // 7. 预热阴影效果
            using (var shadowPaint = new SKPaint())
            {
                shadowPaint.Color = SKColors.Green;
                shadowPaint.ImageFilter = SKImageFilter.CreateDropShadow(
                    5, 5, 5, 5, SKColors.Black.WithAlpha(0x80));

                // 矩形阴影
                warmupCanvas.DrawRect(new SKRect(10, 200, 60, 240), shadowPaint);

                // 圆形阴影
                warmupCanvas.DrawCircle(new SKPoint(100, 220), 20, shadowPaint);

                shadowPaint.ImageFilter = null;
            }

            // 8. 预热混合模式（Blend Modes）
            using (var blendPaint = new SKPaint())
            {
                blendPaint.Color = SKColors.Red.WithAlpha(0x80);
                blendPaint.BlendMode = SKBlendMode.Multiply;
                warmupCanvas.DrawRect(new SKRect(10, 250, 50, 290), blendPaint);

                blendPaint.Color = SKColors.Blue.WithAlpha(0x80);
                warmupCanvas.DrawRect(new SKRect(30, 260, 70, 300), blendPaint);
                blendPaint.BlendMode = SKBlendMode.SrcOver;
            }

            // 9. 预热抗锯齿和位图
            using (var bitmap = new SKBitmap(50, 50))
            using (var bitmapCanvas = new SKCanvas(bitmap))
            using (var bitmapPaint = new SKPaint { IsAntialias = true })
            {
                bitmapCanvas.Clear(SKColors.Transparent);
                bitmapCanvas.DrawCircle(25, 25, 20,
                    new SKPaint { Color = SKColors.Cyan, IsAntialias = true });

                // 绘制位图
                warmupCanvas.DrawBitmap(bitmap, 80, 250, bitmapPaint);

                // 拉伸位图（触发不同的着色器）
                warmupCanvas.DrawBitmap(bitmap, new SKRect(140, 250, 210, 300), bitmapPaint);
            }

            // 10. 多次Flush确保GPU编译完成
            for (int i = 0; i < 5; i++)
            {
                warmupCanvas.Clear(SKColors.Transparent);
                warmupCanvas.DrawPaint(new SKPaint { Color = SKColors.White });
                surface.Flush();
            }

            Console.WriteLine("SkiaSharp Warmup Completed!");
        }
    }
}
