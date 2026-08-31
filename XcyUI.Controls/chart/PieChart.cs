using System;
using System.Linq;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.utils;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static bool IsPointInSector(XPoint point, XRect rect, float startAngle, float sweepAngle)
        {
            var size = Math.Min(rect.Width, rect.Height);
            return IsPointInSector(rect.Center, size / 2, startAngle, sweepAngle, point);
        }

        public static bool IsPointInSector(XPoint center, float radius,
                                      float startAngle, float sweepAngle,
                                      XPoint point)
        {
            float dx = point.X - center.X;
            float dy = point.Y - center.Y;
            if (dx * dx + dy * dy > radius * radius)
                return false;
            float pointAngle = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
            float start = startAngle;
            float end = startAngle + sweepAngle;
            while (pointAngle - start > 180) pointAngle -= 360;
            while (pointAngle - start < -180) pointAngle += 360;
            while (end - start > 180) end -= 360;
            while (end - start < -180) end += 360;
            float tolerance = 5f;
            if (sweepAngle > 0)
            {
                return pointAngle >= start - tolerance && pointAngle <= end + tolerance;
            }
            else
            {
                return pointAngle <= start + tolerance && pointAngle >= end - tolerance;
            }
        }

        public static XModify Arc(XColor color, float startAngle, float sweepAngle, bool userCenter = true)
        {
            return Spacer(FILL)
                .EnableOverDraw(true)
                .Background(color.Copy(0.6f))
                .Border(color, 8)
                .Draw(builder =>
            {
                RenderImp.DrawArc(builder.View.RenderRect, builder.View.Style, startAngle, sweepAngle, userCenter);
            });
        }

        private static void CalculateArc(float[] values, Action<float, float,int> content)
        {
            var sum = values.Sum();
            var startAngle = -90f;
            for (int i = 0; i < values.Length; i++)
            {
                var sweepAngle = 360 * values[i] / sum;
                var a = i;
                content(startAngle, sweepAngle, a);
                startAngle += sweepAngle;
            }
        }

        public static XModify PieChart(string title, string[] xAxis, float[] values, XColor[] colors)
        {
            var hoverViewVisible = StateValueOf(false);
            var sourceRect = StateValueOf(XRect.Empty);
            var chartInfoState = StateValueOf(new ChartInfo());
            return Box(() =>
            {
                HoverChartInfoCardView(hoverViewVisible, sourceRect, chartInfoState);
                CalculateArc(values, (start, sweep, i) =>
                {
                    Arc(colors[i], start, sweep).Bind(chartInfoState,(builder,info)=>
                    {
                        builder.Border(colors[i], info.Index == i ? 3 : 2);
                    });
                });
            })
            .OnHover((builder, info) =>
            {
                var rect = new XRect(XEvent.X + 30.AsPx(), XEvent.Y - 2, 2, 2);
                sourceRect.Value = rect;
                var point = info.Point;
                CalculateArc(values, (start, sweep, i) =>
                {
                    if (IsPointInSector(point, builder.View.RenderRect, start, sweep))
                    {
                        hoverViewVisible.Value = true;
                        chartInfoState.Value = new(i,colors[i], xAxis[i], $"{title}:{values[i]}");
                    }
                });
            })
            .OnLeave((_, _) =>
            {
                hoverViewVisible.Value = false;
                chartInfoState.Value = new ChartInfo() { Index = -1 };
            }) ;
        }
    }
}
