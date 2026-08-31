using System;
using System.Runtime.CompilerServices;
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
        public static XModify Slider(
            float value,
            Action<float> onChanged = null,
            int trackSize = 10,
            int thumbSize = 28,
            [CallerLineNumber] int key = 0)
        {
            // 状态对象，便于重组UI
            var paramState = StateValueOf((0, 0, 0), true, key: key);

            // 计算位置
            (int, int, int) CalculationParams(int width, float progress)
            {
                var thumbSizePx = thumbSize.AsPx();
                var trackWidth = (width - thumbSizePx) * progress;
                var left = (int)(trackWidth - thumbSizePx / 2);
                return (width, (int)trackWidth.AsDp(), left.AsDp());
            }

            // 根据指定位置设置值
            void setParam(XModify builder, int x)
            {
                // 计算UI值
                var width = builder.View.Parent.Width.AsDp();
                var trackWidth = (x - builder.View.Parent.X).AsDp();
                var left = trackWidth - thumbSize / 2;
                var progress = (float)trackWidth / (width - thumbSize);
                // 重组UI
                paramState.Value = (paramState.Value.Item1, trackWidth, left);
                // 透出给函数外的
                onChanged?.Invoke(value);
            }
            // 先描绘一个静态UI，再通过paramState状态对象重组
            return Box(paramState, param =>
            {
                // 背景
                Spacer(trackSize).Width(FILL)
                .Radius(trackSize / 2)
                .Background(XTheme.Color.BaseBorder)
                .HoverCursor(XCursorType.Hand)
                .Click((builder, info)=> setParam(builder,info.X), defaultEffect: false);

                // trackBar
                Spacer(trackSize)
                .Width(param.Item2) // 宽度是动态的
                .Radius(trackSize / 2)
                .Background(XTheme.Color.Primary);

                // thumbBar
                var visibleState = StateValueOf(false);
                var animateValue = AnimateFloatOf(visibleState);
                var isMaxScale = StateValueOf(false);
                Spacer(thumbSize)
                .Margin(left: param.Item3) // marginLeft是动态的
                .Background(XTheme.Color.White)
                .Border(XTheme.Color.Primary, 2)
                .HoverCursor(XCursorType.Arrow)
                .Circle()
                // 滑动的时候改变paramState对象让UI重组实现参数动态画
                .Drag(XDragType.Horizontal, (builder, info) => setParam(builder,builder.View.X))
                // 悬浮开启动画
                .ToggleHover(isHover =>
                {
                    isMaxScale.Value = !isHover;
                    visibleState.Value = true;
                })
                // 绑定动画
                .Bind(animateValue, (builder, progress) =>
                {
                    if (visibleState.Value)
                    {
                        var maxScale = 1.2f;
                        var start = 1f;
                        var end = maxScale;
                        if (isMaxScale.Value)
                        {
                            start = maxScale;
                            end = 1f;
                        }
                        var currentValue = start + (end - start) * progress;
                        builder.Scale(currentValue);
                    }
                });
            })
            .Size(500, WRAP)
            .Padding(horizontal: thumbSize / 2)
            .ContentAlignment(XAlignment.LeftCenter)
            .MeasureStart(builder =>
            {
                var width = builder.View.Width;
                if (width != paramState.Value.Item1)
                {
                    paramState.Value = CalculationParams(width, value);
                }
            });
        }
    }
}
