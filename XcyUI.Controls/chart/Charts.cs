using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;


namespace XcyUI.Controls
{
    public struct XPointF
    {
        public float X { get; set; }
        public float Y { get; set; }
        public XPointF(float x,float y)
        {
            X = x;
            Y = y;
        }
    }

    public struct ChartInfo
    {
        public int Index { get; set; }
        public XColor Color { get; set; }
        public string Label1 { get; set; }
        public string Label2 { get; set; }
        public ChartInfo(int index,XColor color, string label1, string label2)
        {
            Index = index;
            Color = color;
            Label1 = label1;
            Label2 = label2;
        }
    }
    public static partial class Controls
    {
        public static List<float> LTTBReduceValues(List<float> values, int threshold)
        {
            int dataLength = values.Count;

            // 👇 点数少于目标数量 → 直接返回，不稀释
            if (dataLength <= threshold)
                return values;

            List<float> sampled =
            [
                values[0], // 保留第一个点
            ];

            int bucketSize = (dataLength - 2) / (threshold - 2);
            int aIndex = 0;

            for (int i = 1; i < threshold - 1; i++)
            {
                int bucketStart = i * bucketSize + 1;
                int bucketEnd = (i + 1) * bucketSize + 1;
                bucketEnd = Math.Min(bucketEnd, dataLength - 1);

                // 上一个点 A
                float aY = values[aIndex];
                XPointF a = new XPointF(aIndex, aY);

                // 下一个区间的终点 B（虚拟）
                int bRealIndex = Math.Min((i + 1) * bucketSize + 1, dataLength - 1);
                XPointF b = new XPointF(bRealIndex, values[bRealIndex]);

                float maxArea = -1;
                int bestIndex = bucketStart;

                // 遍历当前分段，找【面积最大 = 最能代表波形】的点
                for (int j = bucketStart; j < bucketEnd; j++)
                {
                    XPointF p = new XPointF(j, values[j]);
                    // 三角形面积公式（LTTB核心）
                    float area = Math.Abs((a.X - p.X) * (b.Y - a.Y) - (a.X - b.X) * (p.Y - a.Y));

                    if (area > maxArea)
                    {
                        maxArea = area;
                        bestIndex = j;
                    }
                }

                sampled.Add(values[bestIndex]);
                aIndex = bestIndex;
            }

            sampled.Add(values[dataLength - 1]); // 保留最后一个点
            return sampled;
        }

        public static XModify YAxis(int[] yAxis)
        {
            return Column(() =>
            {
                var font = new XFont()
                {
                    Size = XTheme.Size.Body,
                    Weight = XTheme.Weight.Middle
                };
                var lableWidth = RenderImp.MeasureText(yAxis.Max().ToString(), font).Width+2;
                for (int i = 0; i < yAxis.Length; i++)
                {
                    Row(() =>
                    {
                        Text(yAxis[i].ToString()).Width(lableWidth).TextAlignment(XAlignment.RightCenter);
                        Spacer(1).Weight(1).BottomBorder();
                    })
                    .Height(100)
                    .Width(FILL)
                    .Space(5);
                }
            }).Size(FILL).VerticalAlignment(XVerticalAlignment.Bisect);
        }

        public static XModify XAxis(object[] xAxis)
        {
            return Row(() =>
            {
                for (int i = 0; i < xAxis.Length; i++)
                {
                    Text(xAxis[i].ToString()).TextAlignment(XAlignment.Center);
                }
            }).Size(FILL, WRAP)
            .Margin(horizontal: 10)
            .Alignment(XAlignment.LeftBottom)
            .Margin(bottom: -5)
            .HorizontalAlignment(XHorizontalAlignment.Bisect);
        }

        public static XModify HoverChartInfoCard(this XModify builder, ChartInfo info)
        {
            var visible = StateValueOf(false);
            var sourceRect = StateValueOf(new XRect());
            var infoState = StateValueOf(info);
            HoverChartInfoCardView(visible, sourceRect, infoState);
            builder
                .OnHover((builder, eventInfo) =>
                {
                    var rect = new XRect(XEvent.X + 30.AsPx(), XEvent.Y - 2, 2, 2);
                    sourceRect.Value = rect;
                    infoState.Value = info;
                    visible.Value = true;
                    builder.RefreshParentCache();
                }, "HoverChartInfoCard")
                .OnLeave((_, _) =>
                {
                    visible.Value = false;
                }, "HoverChartInfoCard");
            return builder;
        }

        public static void HoverChartInfoCardView(XState<bool> visible, XState<XRect> rectState, XState<ChartInfo> infoState)
        {
            PopupCard(visible, builder =>
            {
                Column(infoState, info =>
                {
                    Text(info.Label1).Color(XTheme.Color.White);
                    Row(() =>
                    {
                        Spacer(20).Background(info.Color.Copy(0.5f)).Border(info.Color, 1);
                        Text(info.Label2).Color(XTheme.Color.White);
                    }).Space(10);
                })
                .Size(WRAP)
                .Padding(10).Space(10)
                .Background(XTheme.Color.Black.Copy(0.8f))
                .Radius(XTheme.Radius.Low)
                .HorizontalAlignment(XHorizontalAlignment.Left)
                .Alignment(XAlignment.LeftTop)
                .Bind(rectState, (builder, sourceRect) =>
                {
                    var width = builder.View.RootView().Width;
                    var height = builder.View.RootView().Height;
                    var rect = builder.View.RenderRect;
                    var point = PopoverUtils.GetLocation(rect, sourceRect, width, height, true, 0);
                    if (point.X != sourceRect.X)
                    {
                        point.X -= 35.AsPx();
                    }
                    builder.Margin(left: point.X.AsDp(), top: point.Y.AsDp());
                },needLayout: true).FadeIn();
            },
            disableOutClick: false,
            outSideClick: (_, _) => visible.Value = false
            );
        }
    }
}
