using System;
using XcyUI.Components.Utils;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.utils;
using XcyUI.views;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        /// <summary>
        /// 颜色选择
        /// </summary>
        /// <param name="color">初始颜色</param>
        /// <param name="onSelected">选择时函数回调</param>
        /// <returns></returns>
        public static XModify ColorPicker(XColor color, Action<XColor> onSelected)
        {
            return Column(() =>
            {
                float h = 0;
                float s = 0;
                float v = 0;
                ColorUtils.ColorToHsv(color, out h, out s, out v);
                var pointer = StateValueOf(XPoint.Empty);
                var huePoint = StateValueOf(XPoint.Empty);
                var alphaPoint = StateValueOf(XPoint.Empty);
                var panelRect = StateValueOf(XRect.Empty);
                var hueRect = StateValueOf(XRect.Empty);
                var alphaRect = StateValueOf(XRect.Empty);
                var selectColorState = StateValueOf(color);
                panelRect.Join(pointer);
                hueRect.Join(huePoint);
                alphaRect.Join(alphaPoint);
                var shadow = new XShadow()
                {
                    Color = XColors.Black,
                    Blur = 2
                };
                var panelWidth = 300;
                var panelHeight = 200;
                var pointerSize = 8;
                Row(() =>
                {
                    // 渐变面板
                    Box(() =>
                    {
                        Spacer()
                        .Size(panelWidth, panelHeight)
                        .Click((builder, info) =>
                        {
                            var rect = builder.View.RenderRect;
                            ColorUtils.PointToSV(info.X - rect.Left, info.Y - rect.Top, rect.Width, rect.Height,
               out s, out v);
                            pointer.Value = info.Point;
                            var selectColor = ColorUtils.HsvToColor(h, s, v);
                            var alpha = selectColorState.Value.Alpha;
                            selectColorState.Value = selectColor.Copy(alpha);
                        }, false)
                        .LayoutEnd(builder =>
                        {
                            panelRect.Value = builder.View.RenderRect;
                        })
                        .OnDraw(builder =>
                        {
                            // 绘制渐变
                            var style = builder.View.Style;
                            var rect = builder.View.ContentRect;
                            RenderImp.DrawPath(rect, style, false, () =>
                            {
                                //RenderImp.AddRect(rect);
                                var hColor = ColorUtils.HsvToColor(h, 1, 1);
                                RenderImp.AddRect(rect, [XColors.White, hColor], XGradientDirection.Horizontal);
                                RenderImp.AddRect(rect, [XColors.Transparent, XColors.Black], XGradientDirection.Vertical);
                            });
                        });

                        // 选点
                        Spacer(pointerSize)
                        .Circle()
                        .Bind(panelRect, (builder, rect) =>
                        {
                            if (rect.Width != 0)
                            {
                                var dragRect = rect;
                                var p = ColorUtils.SVToPoint(s, v, rect);
                                var radius = pointerSize.AsPx() / 2;
                                var marginX = p.X - rect.X;
                                var marginY = p.Y - rect.Y;
                                builder.Margin(marginX.AsDp(), marginY.AsDp());
                                builder.View.Location = new XPoint(p.X, p.Y);
                                builder.View.Invalidate();
                            }
                        })
                        .Bind(selectColorState, (builder, color) =>
                        {
                            onSelected?.Invoke(color);
                        })
                        .Drag(XDragType.All, (builder, info) =>
                        {
                            var rect = panelRect.Value;
                            var center = builder.View.RenderRect.Center;
                            ColorUtils.PointToSV(center.X - rect.Left, center.Y - rect.Top, rect.Width, rect.Height,
               out s, out v);
                            var selectColor = ColorUtils.HsvToColor(h, s, v);
                            var alpha = selectColorState.Value.Alpha;
                            selectColorState.Value = selectColor.Copy(alpha);
                        })
                        .Border(XColors.White, 2)
                        .Background(XColors.Gray)
                        .Shadow(shadow)
                        .Alignment(XAlignment.LeftTop);

                    })
                    .Size(panelWidth + pointerSize, panelHeight + pointerSize);

                    Spacer(10);

                    // 彩色条
                    var hueWidth = 18;
                    var hueHeight = 200;
                    var hueBarWidth = 20;
                    var hueBarHeight = 6;
                    Box(() =>
                    {
                        Spacer()
                        .Size(hueWidth, hueHeight)
                        .Click((builder, info) =>
                        {
                            h = ColorUtils.YToHue(info.Y - builder.View.Y, builder.View.Height);
                            huePoint.Value = info.Point;
                            var alpha = selectColorState.Value.Alpha;
                            selectColorState.Value = ColorUtils.HsvToColor(h, s, v).Copy(alpha);
                        }, false)
                        .LayoutEnd((builder) =>
                        {
                            hueRect.Value = builder.View.RenderRect;
                        })
                        .Draw(builder =>
                        {
                            var style = builder.View.Style;
                            var rect = builder.View.ContentRect;
                            RenderImp.DrawPath(rect, style, false, () =>
                            {
                                //RenderImp.AddRect(rect);
                                var hColor = ColorUtils.HsvToColor(h, 1, 1);
                                RenderImp.AddRect(rect, [XColors.Red, XColors.Orange, XColors.Yellow,
                                XColors.Green, XColors.Cyan, XColors.Blue, XColors.Magenta, XColors.Red], XGradientDirection.Vertical);
                            });
                        });
                        // hue 条
                        Spacer().Size(hueBarWidth, hueBarHeight)
                        .Background(XColors.White)
                        .Alignment(XAlignment.TopCenter)
                        .Bind(hueRect, (builder, rect) =>
                        {
                            if (rect.Width != 0)
                            {
                                float hy = ColorUtils.HueToY(h, rect.Height);
                                int y = (int)(hy + rect.Y);
                                builder.Margin(top: (y - rect.Y).AsDp());
                                builder.View.Y = y;
                                builder.View.Invalidate();
                            }
                        })
                        .Drag(XDragType.Vertical, (builder, info) =>
                        {
                            var rect = hueRect.Value;
                            var y = builder.View.RenderRect.Center.Y;
                            h = ColorUtils.YToHue(y - rect.Y, rect.Height);
                            var alpha = selectColorState.Value.Alpha;
                            selectColorState.Value = ColorUtils.HsvToColor(h, s, v).Copy(alpha);
                        })
                        .Radius(2).DefaultBorder().Shadow(shadow);
                    })
                    .Size(hueBarWidth, hueHeight + hueBarHeight);
                });

                Spacer(10);

                // 透明度条
                var alphaBarWidth = 6;
                Box(() =>
                {
                    Spacer()
                    .Size(FILL, 18)
                    .Margin(horizontal: alphaBarWidth / 2)
                    .Click((builder, info) =>
                    {
                        var left = info.X - builder.View.X;
                        var alpha = ((float)left / builder.View.Width) * 255;
                        selectColorState.Value = selectColorState.Value.Copy((byte)alpha);
                        alphaPoint.Value = info.Point;
                    }, defaultEffect: false)
                    .LayoutEnd(buidler =>
                    {
                        alphaRect.Value = buidler.View.RenderRect;
                    })
                    .OnDraw(builder =>
                    {
                        var style = new XStyle();
                        style.Reset();
                        var rect = builder.View.ContentRect;
                        var gridSize = rect.Height / 2;
                        for (int y = 0; y < rect.Height; y += gridSize)
                        {
                            for (int x = 0; x < rect.Width; x += gridSize)
                            {
                                bool isEven = (x / gridSize + y / gridSize) % 2 == 0;
                                var color = isEven ? XColors.Gray : XColors.White;
                                style.Background = new XBrush() { StartColor = color };
                                var cell = new XRect(x + rect.X, y + rect.Y, gridSize, gridSize);
                                RenderImp.DrawRect(cell, style);
                            }
                        }
                        var hColor = ColorUtils.HsvToColor(h, 1, 1);
                        style.Background = new XBrush()
                        {
                            StartColor = XColors.Transparent,
                            EndColor = hColor
                        };
                        RenderImp.DrawRect(rect, style);
                    });

                    Spacer().Size(alphaBarWidth, 22)
                        .Background(XColors.White)
                        .Alignment(XAlignment.LeftCenter)
                        .Bind(alphaRect, (builder, rect) =>
                        {
                            if (rect.Width != 0)
                            {
                                float hx = (selectColorState.Value.Alpha / 255f) * rect.Width;
                                int x = (int)(hx + rect.X);
                                builder.Margin(left: (x - rect.X).AsDp());
                                builder.View.X = x;
                                builder.View.Invalidate();
                            }
                        })
                        .Drag(XDragType.Horizontal, (builder, info) =>
                        {
                            var rect = alphaRect.Value;
                            var center = builder.View.RenderRect.Center;
                            var left = center.X - rect.X;
                            left = Math.Min(left, rect.Width);
                            left = Math.Max(0, left);
                            var alpha = ((float)left / rect.Width) * 255;
                            selectColorState.Value = selectColorState.Value.Copy((byte)alpha);
                        })
                        .Radius(2).DefaultBorder().Shadow(shadow);
                })
                .Size(FILL, 18);

                Spacer(10);
                Row(() =>
                {
                    Input()
                    .Width(80)
                    .TextAlignment(XAlignment.Center)
                    .KeyPress((builder, info) =>
                    {
                        var text = builder.AsView<XText>().Text;
                        byte.TryParse(text, out byte alpha);
                        if (alpha >= 0 && alpha <= 255)
                        {
                            selectColorState.Value = selectColorState.Value.Copy(alpha);
                            alphaPoint.Send(alphaPoint.Value);
                        }
                    })
                    .Bind(selectColorState, (builder, color) =>
                    {
                        builder.Content(color.Alpha.ToString());
                    }, true)
                    .PrimaryInput();
                    Spacer(10);

                    Input().PrimaryInput().Weight(1)
                    .KeyPress((builder, info) =>
                    {
                        var text = builder.AsView<XText>().Text;
                        try
                        {
                            if (text.Length < 9) return;
                            var color = XColors.FromHex(text);
                            selectColorState.Value = color;
                            ColorUtils.ColorToHsv(color, out h, out s, out v);
                            pointer.Send(pointer.Value);
                            huePoint.Send(huePoint.Value);
                            alphaPoint.Send(alphaPoint.Value);
                        }
                        catch
                        {
                        }
                    })
                    .Bind(selectColorState, (builder, color) =>
                    {
                        builder.Content(color.Hex);
                    }, true);
                }).Width(FILL);
            }).Size(WRAP).Padding(6);
        }
    }
}
