using System;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XModify VerticalBars(
            string title,
            int[] yAxis,
            string[] xAxis,
            float[] values,
            Action<XModify, int>? content = null)
        {
            return Row(() =>
            {
                for (int i = 0; i < values.Length; i++)
                {
                    var a = i;
                    var yAxisCount = yAxis.Length;
                    var yMax = yAxis.Max();
                    var yMin = yAxis.Min();
                    var color = XTheme.Color.Primary;
                    Spacer()
                    .Width(FILL)
                    .Margin(horizontal: 20)
                    .Background(color.Copy(0.5f))
                    .Border(color, 1)
                    .HoverBackgroundColor(color.Copy(0.6f))
                    .Alignment(XAlignment.BottomCenter)
                    .HoverChartInfoCard(new ChartInfo(i, color, xAxis[a], $"{title}:{values[a]}"))
                    .Also(n=> content?.Invoke(n, a))
                    .MeasureStart(builder =>
                    {
                        float height = builder.View.Parent.Height;
                        var itemHeight = height / yAxisCount;
                        height -= itemHeight;
                        float bl = (height) / (yMax - yMin);
                        var barHeight = bl * (values[a] - yMin) + builder.View.LayoutParams.Padding.VerticalSize;
                        barHeight = Math.Max(1, barHeight.AsDp());
                        builder.Height((int)barHeight)
                        .Margin(bottom: (int)(itemHeight / 2 - builder.View.LayoutParams.Padding.Bottom).AsDp() + 2);
                    });
                }
            })
            .Size(FILL)
            .Margin(horizontal: 10)
            .Alignment(XAlignment.LeftBottom)
            .HorizontalAlignment(XHorizontalAlignment.Bisect);
        }
        public static XModify VerticalBars(int[] yAxis, float[] values, Func<int, XModify> content)
        {
            return Row(() =>
            {
                for (int i = 0; i < values.Length; i++)
                {
                    var a = i;
                    var yAxisCount = yAxis.Length;
                    var yMax = yAxis.Max();
                    var yMin = yAxis.Min();
                    content(i)
                    .Width(FILL)
                    .Alignment(XAlignment.BottomCenter)
                    .MeasureStart(builder =>
                    {
                        float height = builder.View.Parent.Height;
                        var itemHeight = height / yAxisCount;
                        height -= itemHeight;
                        float bl = (height) / (yMax - yMin);
                        var barHeight = bl * (values[a] - yMin) + builder.View.LayoutParams.Padding.VerticalSize;
                        barHeight = Math.Max(1, barHeight.AsDp());
                        builder.Height((int)barHeight)
                        .Margin(bottom: (int)(itemHeight / 2 - builder.View.LayoutParams.Padding.Bottom).AsDp() + 2);
                    });
                }
            })
            .Size(FILL)
            .Clip()
            .Margin(horizontal: 10)
            .Alignment(XAlignment.LeftBottom)
            .HorizontalAlignment(XHorizontalAlignment.Bisect);
        }
    }
}
