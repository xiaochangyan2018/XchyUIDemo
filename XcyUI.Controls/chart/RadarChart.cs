using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        private static List<XPoint> CalculateRadarPoints(XRect rect, float[] values, float maxValue, float minValue, bool isValue)
        {
            XPoint center = rect.Center;
            float maxRadius = Math.Min(rect.Width, rect.Height) / 2;
            List<XPoint> points = new List<XPoint>();
            float radius = maxRadius;
            for (int i = 0; i < values.Length; i++)
            {
                float angleDegrees = -90 + i * (360f / values.Length);
                float radians = angleDegrees * (float)Math.PI / 180;
                if (isValue)
                {
                    float ratio = Math.Min(1.0f, Math.Max(0, (values[i] - minValue) / (maxValue - minValue)));
                    radius = maxRadius * ratio;
                }
                float x = center.X + radius * (float)Math.Cos(radians);
                float y = center.Y + radius * (float)Math.Sin(radians);
                points.Add(new XPoint((int)x, (int)y));
            }
            return points;
        }

        public static XModify RadarChart(
            string title,
            int[] yAxis,
            string[] xAxis,
            float[] values)
        {
            return Box(() =>
            {
                // 雷达图背景
                RadarBackground(values);
                // 刻度
                RadarYAxis(yAxis);
                // 雷达图值
                RadarLineArea(yAxis, values);
                // 画点
                RadarCircles(title, yAxis, xAxis, values);
                // 标签
                RadarXAxis(xAxis);
            });
        }

        public static XModify RadarBackground(float[] values)
        {
            return Spacer(FILL)
                .EnableOverDraw(true)
                .DefaultBorder()
                .OnDraw(builder =>
                {
                    var rect = builder.View.RenderRect.ToCircle();
                    var style = builder.View.Style;
                    var item = (rect.Width / 2) / (values.Length - 1);
                    for (int i = 0; i < values.Length - 1; i++)
                    {
                        if (i > 0)
                        {
                            rect.Scale(-item);
                        }
                        DrawRander(rect, style, values, 0, 0, false);
                    }
                });
        }

        public static XModify RadarLineArea(int[] yAxis, float[] values)
        {
            return Spacer(FILL)
                .EnableOverDraw(true)
                .Background(XTheme.Color.Primary.Copy(0.3f))
                .Border(XTheme.Color.Primary, 2)
                .OnDraw(builder =>
                {
                    var rect = builder.View.RenderRect.ToCircle();
                    var style = builder.View.Style;
                    DrawRander(rect, style, values, yAxis.Max(), yAxis.Min(), true);
                });
        }

        public static XModify RadarCircles(string title, int[] yAxis, string[] xAxis, float[] values)
        {
            return Box(() =>
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        Spacer(12)
                        .Background(XTheme.Color.Primary)
                        .Border(XTheme.Color.Background, 2)
                        .HoverBackgroundColor(XTheme.Color.PrimaryDark)
                        .Circle()
                        .HoverChartInfoCard(new ChartInfo(i, XTheme.Color.Primary, xAxis[i], $"{title}:{values[i]}"));
                    }
                })
                .ContentAlignment(XAlignment.LeftTop)
                
                .MeasureStart(builder=>
                {
                    var renderRect = builder.View.RenderRect;
                    int size = Math.Min(renderRect.Width, renderRect.Height);
                    var rect = new XRect(size, size);
                    var points = CalculateRadarPoints(rect, values, yAxis.Max(), yAxis.Min(),true);
                    for (int i = 0; i < points.Count; i++)
                    {
                        var view = builder.View.ChildElemnt(i);
                        var x = points[i].X + (renderRect.Width - size) / 2;
                        var y = points[i].Y + (renderRect.Height - size) / 2;
                        view.Measure();
                        x -= view.Width / 2;
                        y -= view.Height / 2;
                        XModify.With(view).Margin(left: x.AsDp(), top: y.AsDp());
                    }
                });
        }

        private static XModify RadarYAxis(int[] axis)
        {
            return Box(() =>
            {
                for (int i = 0; i < axis.Length-1; i++)
                {
                    var num = i;
                    Text(axis[i].ToString())
                    .Background(XTheme.Color.Background)
                    .MeasureStart(builder =>
                    {
                        var rect = builder.View.Parent.RenderRect;
                        int radius = Math.Min(rect.Width, rect.Height) / 2;
                        var item = radius / (axis.Length-1);
                        var topMarin = item * num - builder.AsView<XText>().Font.Size;
                        builder.Margin(topMarin.AsDp());
                    });
                }
            }).ContentAlignment(XAlignment.TopCenter);
        }

        private static XModify RadarXAxis(string[] axis)
        {
            return Box(() =>
            {
                foreach (var item in axis)
                {
                    Text(item.ToString());
                }
            })
                .ContentAlignment(XAlignment.LeftTop)
                
                .MeasureStart(builder =>
                {
                    var renderRect = builder.View.RenderRect;
                    int size = Math.Min(renderRect.Width, renderRect.Height);
                    var rect = new XRect(size, size);
                    var points = CalculateRadarPoints(rect, new float[axis.Length], 0, 0, false);
                    for (int i = 0; i < points.Count; i++)
                    {
                        var view = builder.View.ChildElemnt(i);
                        var x = points[i].X + (renderRect.Width - size) / 2;
                        var y = points[i].Y + (renderRect.Height - size) / 2;
                        var margin = 30;
                        if (Math.Abs(points[i].X - rect.Center.X) <= 2)
                        {
                            view.Measure();
                            x -= view.Width / 2;
                            if (points[i].Y < rect.Center.Y)
                            {
                                y -= view.Height + margin;
                            }
                            else
                            {
                                y += margin;
                            }
                        }
                        else if (points[i].X < rect.Center.X)
                        {
                            x -= view.Width + margin;
                        }
                        else
                        {
                            x += margin;
                        }
                        XModify.With(view).Margin(left: x.AsDp(), top: y.AsDp());
                    }
                });
        }

        private static void DrawRander(XRect rect, XStyle style, float[] values, float maxValue, float minValue, bool isValue)
        {
            var points = CalculateRadarPoints(rect, values, maxValue, minValue, isValue);
            RenderImp.DrawPath(rect, style, false, () =>
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (i == 0)
                    {
                        RenderImp.MoveTo(points[i].X, points[i].Y);
                    }
                    else
                    {
                        RenderImp.LineTo(points[i].X, points[i].Y);
                        if (i == points.Count - 1)
                            RenderImp.LineTo(points[0].X, points[0].Y);
                    }
                }
            });
            if (isValue) return;
            for (int i = 0; i < points.Count; i++)
            {

                RenderImp.DrawPath(rect, style, false, () =>
                {
                    RenderImp.MoveTo(points[i].X, points[i].Y);
                    RenderImp.LineTo(rect.Center.X, rect.Center.Y);
                });
            }
        }
    }
}
