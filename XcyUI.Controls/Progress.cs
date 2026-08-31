using System;
using XcyUI.animation;
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
        public static XModify CircleAnim(this XModify builder, bool isStart = true)
        {
            var visibleState = StateValueOf(isStart, isReset: true);
            var animateValue = AnimateFloatOf(visibleState, animate =>
            {
                animate.Times = int.MaxValue;
                animate.Duration = 600;
                animate.Interpolator = XAnimationInterpolator.Uniform;
            });
            builder.Bind(animateValue, (b, value) =>
            {
                b.Rotate(value * 360);
            });
            return builder;
        }
        public static XModify ColorLoading(XColor color, int size, int borderSize)
        {
            var borderBursh = new XBrush() { StartColor = color, EndColor = color.Copy(0f), Direction = XGradientDirection.Round };
            return Spacer(size).Circle()
               .EnableCache(true)
               .EnableOverDraw(true)
               .Border(new XBorder() { Color = borderBursh, Size = new XSpace(borderSize.AsPx()) })
               .OnDraw(builder =>
               {
                   var style = builder.View.Style;
                   var rect = builder.View.ContentRect;
                   var startAngle = Math.Max(borderSize.AsPx(), 10);
                   RenderImp.DrawArc(builder.View.ContentRect, style, startAngle, 360 - startAngle * 2);
               }).CircleAnim();
        }

        public static XModify CircleProgress(XColor color, int size, int borderSize, XState<float> progress)
        {
            return Spacer(size)
                .Circle()
                .Border(XTheme.Color.BaseBorder, borderSize)
                .Bind(progress, (builder, value) =>
                {
                    builder.View.Invalidate();
                })
                .Draw(builder =>
                {
                    var style = builder.View.Style.Copy();
                    style.Border = new XBorder(new XBrush() { StartColor = color }, new XSpace(borderSize.AsPx()), XDashType.Solid);
                    RenderImp.DrawArc(builder.View.ContentRect, style, -90, 360 * progress.Value);
                });
        }
    }
}
