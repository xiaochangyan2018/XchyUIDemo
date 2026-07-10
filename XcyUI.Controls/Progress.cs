using System;
using XcyUI.animation;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.utils;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XWidget;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XViewBuilder CircleAnim(this XViewBuilder builder, bool isStart = true)
        {
            var visibleState = XWidget.StateValueOf(isStart, isReset: true);
            var animateValue = XWidget.AnimateFloatOf(visibleState, animate =>
            {
                animate.Times = int.MaxValue;
                animate.Duration = 800;
                animate.Interpolator = XAnimationInterpolator.Uniform;
            });
            builder.Bind(animateValue, (b, value) =>
            {
                b.Rotate(value * 360);
            });
            return builder;
        }
        public static XViewBuilder ColorLoading(XColor color, int size, int borderSize)
        {
            var borderBursh = new XBrush() { StartColor = color, EndColor = color.Copy(0f), Direction = XGradientDirection.Round };
            return Spacer(size).Circle()
               .AccessibilityRole(XAccessibilityRole.ProgressBar)
               .AccessibilityValue("loading")
               .EnableCache(true)
               .EnableOverDraw(true)
               .Border(new XBorder() { Color = borderBursh, Size = new XSpace(borderSize.AsPx()) })
               .OnDraw(builder =>
               {
                   var style = builder.View.Style;
                   var rect = builder.View.RenderRect;
                   var startAngle = Math.Max(borderSize.AsPx(), 10);
                   RenderImp.DrawArc(builder.View.RenderRect, style, startAngle, 360 - startAngle * 2);
               }).CircleAnim();
        }

        public static XViewBuilder CircleProgress(XColor color, int size, int borderSize, XState<float> progress)
        {
            return Spacer(size)
                .Circle()
                .AccessibilityRole(XAccessibilityRole.ProgressBar)
                .AccessibilityValue(progress.Value.ToString("P0"))
                .Border(xTheme.Colors.BaseBorder, borderSize)
                .Bind(progress, (builder, value) =>
                {
                    builder.AccessibilityValue(value.ToString("P0"));
                    builder.View.Invalidate();
                })
                .OnDraw(builder =>
                {
                    var style = builder.View.Style.Copy();
                    style.Border = new XBorder(new XBrush() { StartColor = color }, new XSpace(borderSize.AsPx()), XDashType.Solid);
                    RenderImp.DrawArc(builder.View.RenderRect, style, -90, 360 * progress.Value);
                });
        }
    }
}
