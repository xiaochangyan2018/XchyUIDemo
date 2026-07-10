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
        public static XViewBuilder Tooltip(this XViewBuilder builder, string tips)
        {
            var tipsState = StateValueOf(tips, true);
            builder.AccessibilityDescription(tips);
            builder.ToggleHover(isHover =>
            {
                if (string.IsNullOrEmpty(tipsState.Value)) return;
                var rect = StateValueOf(builder.View.RenderRect);
                var visible = StateValueOf(false);
                TooltipView(visible, tipsState, rect);
                visible.Value = isHover;
            });
            return builder;
        }
        public static void TooltipView(XState<bool> visible, XState<string> textState, XState<XRect> rectState)
        {
            PopupCard(visible, builder =>
            {
                Text(textState.Value)
                .Alignment(XAlignment.LeftTop)
                .AccessibilityRole(XAccessibilityRole.Tooltip)
                .MiniCard()
                .Background(xTheme.Colors.Black)
                .Color(xTheme.Colors.White)
                .MeasureEnd(b =>
                {
                    var width = b.View.RootView().Width;
                    var height = b.View.RootView().Height;
                    var rect = b.View.RenderRect;
                    var sourceRect = rectState.Value;
                    var point = PopoverUtils.GetLocation(rect, sourceRect, width, height, false);
                    b.Margin(left: point.X.AsDp(), top: point.Y.AsDp());
                })
                .FadeIn(delay:200);
            },
            disableOutClick: false,
            outSideClick: (_, info) => visible.Value = false
            );
        }
    }
}
