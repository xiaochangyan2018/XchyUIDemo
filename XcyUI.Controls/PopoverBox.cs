using System;
using System.Collections.Generic;
using System.Text;
using XcyUI.animation;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.models.XFunctions;
using static XcyUI.widgets.XWidget;
using static XcyUI.Controls.PopoverUtils;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XViewBuilder Popover(this XViewBuilder builder,
            XState<bool> visibleState = null,
            XFunction content = null,
            bool enablePopover = true,
            bool isAlignLeft = false,
            bool isSameWidth = false,
            bool defaultEffect = true)
        {
            var rect = StateValueOf(new XRect());
            visibleState = visibleState ?? StateValueOf(false);
            PopContentView(visibleState ?? StateValueOf(false), content, rect, enablePopover, isAlignLeft, isSameWidth);

            builder
            .LocationChanged(b =>
            {
                visibleState.Value = false;
            })
            .AccessibilityExpanded(visibleState.Value)
            .Bind(visibleState, (b, visible) => b.AccessibilityExpanded(visible))
            .BubbleEvent(XEventType.Click)
            .Click(() =>
            {
                rect.Value = builder.View.RenderRect;
                visibleState.Value = !visibleState.Value;
            }, defaultEffect: defaultEffect, "Popover_click");
            return builder;
        }
        public static void PopContentView(XState<bool> visible, XFunction content, XState<XRect> rectState, bool enablePopover = true, bool isAlignLeft = false, bool isSameWidth = false)
        {
            PopupCard(visible, builder =>
            {
                var visisbleState = StateValueOf(true);
                var isOut = StateValueOf(false);
                var animateValue = AnimateFloatOf(visisbleState, animate =>
                {
                    animate.Interpolator = XAnimationInterpolator.Accelerate;
                });
                var point = StateValueOf(new XPoint(), true);
                PopoverCard(content, rectState, enablePopover, isAlignLeft, isSameWidth)
                .LayoutEnd(b =>
                {
                    var padding = 16.AsPx();
                    var rect = b.View.ContentRect;
                    var sourceRect = rectState.Value;
                    var width = b.View.RootView().Width;
                    var height = b.View.RootView().Height;
                    var direction = GetArrowDirection(rect, sourceRect, width, height, isAlignLeft, 10);
                    if (direction == ArrowDirection.Top)
                    {
                        point.Value = rect.TopCenter;
                    }

                    if (direction == ArrowDirection.Bottom)
                    {
                        point.Value = rect.BottomCenter;
                    }
                    if (direction == ArrowDirection.Right) point.Value = rect.RightCenter;
                    if (direction == ArrowDirection.Left) point.Value = rect.LeftCenter;
                })
                .Bind(animateValue, (b, value) =>
                {
                    b.Scale(1, value, point.Value);
                });
            },
            disableOutClick: false,
            outSideClick: (_, info) =>
            {
                if (!rectState.Value.Contain(info.Point))
                {
                    visible.Value = false;
                }
            });
        }

        public static XViewBuilder PopoverCard(XFunction content, XState<XRect> rectState, bool enablePopover = true, bool isAlignLeft = false, bool isSameWidth = false)
        {
            var padding = 16.AsPx();
            var builder = Box(content)
                .Size(WRAP)
                .Width(isSameWidth ? (enablePopover ? ((rectState.Value.Width + padding * 2).AsDp()) : rectState.Value.Width.AsDp()) : WRAP)
                .Card()
                .Padding(enablePopover ? 16 : 0)
                .Radius(xTheme.Radius.Low)
                .Alignment(XAlignment.LeftTop)
                .MeasureEnd(b =>
                {
                    var width = b.View.RootView().Width;
                    var height = b.View.RootView().Height;
                    var rect = b.View.ContentRect;
                    var sourceRect = rectState.Value;
                    var point = GetLocation(rect, sourceRect, width, height, isAlignLeft, enablePopover ? 5 : 10);
                    if (enablePopover)
                    {
                        var direction = GetArrowDirection(rect, sourceRect, width, height, isAlignLeft, 10);
                        if (direction == ArrowDirection.Top) point.X -= padding;
                        if (direction == ArrowDirection.Bottom)
                        {
                            point.X -= padding;
                            point.Y -= padding * 2;
                        }
                        if (direction == ArrowDirection.Right) point.X -= padding * 2;
                    }
                    b.View.Location = point;
                    b.Margin(left: point.X.AsDp(), top: point.Y.AsDp());
                });
            if (enablePopover)
            {
                builder
                    .EnableOverDraw(true)
                    .OnDraw(b =>
                    {
                        var sourceRect = rectState.Value;
                        var view = b.View;
                        var rect = view.ContentRect;
                        var width = view.RootView().Width;
                        var height = view.RootView().Height;
                        var arrowSize = 10.AsPx();
                        var direction = GetArrowDirection(rect, sourceRect, width, height, isAlignLeft, 10);
                        DrawRoundedArrowBubble(rect, view.Style, view.IsCache, sourceRect, (int)view.Style.Radius.All, arrowSize, direction);
                    }, isOver: false);
            }
            return builder;
        }
    }
}
