using System.Collections.Generic;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static void SmoothLinePath(List<XPoint> points, float smoothness = 0.3f)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                var current = points[i];
                var next = points[i + 1];
                XPoint controlPoint1, controlPoint2;
                if (i == 0)
                {
                    controlPoint1 = current;
                    controlPoint2 = new XPoint(
                        (current.X + next.X) / 2,
                        (current.Y + next.Y) / 2
                    );
                }
                else if (i == points.Count - 2)
                {
                    controlPoint1 = new XPoint(
                        (current.X + points[i - 1].X) / 2,
                        (current.Y + points[i - 1].Y) / 2
                    );
                    controlPoint2 = next;
                }
                else
                {
                    XPoint prev = points[i - 1];
                    XPoint nextNext = points[i + 1];
                    int dx1 = (int)((next.X - prev.X) * smoothness);
                    int dy1 = (int)((next.Y - prev.Y) * smoothness);
                    int dx2 = (int)((current.X - nextNext.X) * smoothness);
                    int dy2 = (int)((current.Y - nextNext.Y) * smoothness);
                    controlPoint1 = new XPoint(current.X + dx1, current.Y + dy1);
                    controlPoint2 = new XPoint(next.X + dx2, next.Y + dy2);
                }
                RenderImp.CubicTo(controlPoint1, controlPoint2, next);
            }
        }

        public static XModify Circels(
            string title,
            int[] yAxis,
            string[] xAxis,
            float[] values)
        {
            return VerticalBars(yAxis, values, i =>
            {
                var builder = Box(() =>
                {
                    Spacer(10)
                    .Background(XTheme.Color.Success.Copy(0.5f))
                    .HoverBackgroundColor(XTheme.Color.Success)
                    .Border(XTheme.Color.Success, 2).Circle()
                    .HoverChartInfoCard(new ChartInfo(i, XTheme.Color.Success, xAxis[i], $"{title}:{values[i]}"))
                    .Margin(top: -5)
                    .Alignment(XAlignment.TopCenter);
                })
                .Width(FILL)
                .Height(0)
                .Padding(top: 5, bottom: 8)
                .Margin(horizontal: 20);
                return builder;
            });
        }

        public static XModify LineArea(int[] yAxis, float[] values)
        {
            return VerticalBars(yAxis, values, i => Spacer().Width(FILL).Margin(horizontal: 20))
                .Background(new XBrush() { StartColor = XTheme.Color.Success, EndColor = XTheme.Color.Success.Copy(0.1f), Direction = XGradientDirection.Vertical })
                .EnableOverDraw(true).OnDraw(builder =>
                {
                    var rect = builder.View.RenderRect;
                    var style = builder.View.Style;
                    if (builder.View.ChildCount() < 3)
                    {
                        return;
                    }
                    RenderImp.DrawPath(rect, style, false, () =>
                    {
                        var first = builder.View.ChildElemnt(0);
                        for (int i = 0; i < builder.View.ChildCount(); i++)
                        {
                            var renderRect = builder.View.ChildElemnt(i).RenderRect;
                            var x = renderRect.Center.X;
                            var y = renderRect.Y;
                            if (i == 0)
                            {
                                RenderImp.MoveTo(x, renderRect.Bottom);
                                RenderImp.LineTo(x, y);
                            }
                            else if (i == builder.View.ChildCount() - 1)
                            {
                                RenderImp.LineTo(x, y);
                                RenderImp.LineTo(x, renderRect.Bottom);
                            }
                            else
                            {
                                RenderImp.LineTo(x, y);
                            }
                        }
                    });
                });
        }

        public static XModify Lines(int[] yAxis, float[] values)
        {
            return VerticalBars(yAxis, values, i => Spacer().Width(FILL).Margin(horizontal: 20))
                .Border(XTheme.Color.Success, 3)
                .EnableOverDraw(true).OnDraw(builder =>
                {
                    var rect = builder.View.RenderRect;
                    var style = builder.View.Style;
                    RenderImp.DrawPath(rect, style, false, () =>
                    {
                        for (int i = 0; i < builder.View.ChildCount(); i++)
                        {
                            var renderRect = builder.View.ChildElemnt(i).RenderRect;
                            var x = renderRect.Center.X;
                            var y = renderRect.Y;
                            if (i == 0)
                            {
                                RenderImp.MoveTo(x, y);
                            }
                            else
                            {
                                RenderImp.LineTo(x, y);
                            }
                        }
                    });
                });
        }
    }
}
