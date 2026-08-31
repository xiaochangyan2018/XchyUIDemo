using XcyUI.Core.Utils;
using XcyUI.models;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyDemo.Animate
{
    public static class Animates
    {
        public static XModify AnimateWidthTo(this XModify modify, XState<bool> animateStart, XState<XRect> targetRect, XAlignment alignment)
        {
            var animateValue = AnimateFloatOf(animateStart, a=> a.Duration = 500);
            var endState = StateValueOf(XRect.Empty);
            var startState = StateValueOf(XRect.Empty);
            return modify
                .LayoutEnd(n =>
                {
                    n.View.LayoutParams.Width = targetRect.Value.Width;
                    n.View.Width = targetRect.Value.Width;
                    n.View.Location = targetRect.Value.LeftBottom;
                })
                .Bind(targetRect, (modify, rect) =>
                {
                    modify.View.LayoutParams.Width = rect.Width;
                    if (modify.View.Width == 0)
                    {
                        modify.View.Width = rect.Width;
                    }
                    var childRect = modify.View.RenderRect;
                    childRect.Width = rect.Width;
                    var point = XRectUtils.GetDockedPoint(rect, childRect, alignment);
                    var isFirst = startState.Value.Equals(XRect.Empty);
                    startState.Value = modify.View.RenderRect;
                    endState.Value = new XRect(point.X, point.Y, rect.Width, modify.View.Height);
                })
                .Bind(animateValue, (modify, value) =>
                {
                    if (animateStart.Value)
                    {
                        modify.View.X = (int)(startState.Value.X + (endState.Value.X - startState.Value.X) * value);
                        modify.View.Y = (int)(startState.Value.Y + (endState.Value.Y - startState.Value.Y) * value);
                        modify.View.Width = (int)(startState.Value.Width + (endState.Value.Width - startState.Value.Width) * value);
                        modify.View.LayoutParams.Margin = new XSpace(endState.Value.X - modify.View.Parent.ContentRect.X, 0, 0, 0);
                    }
                });
        }
    }
}
