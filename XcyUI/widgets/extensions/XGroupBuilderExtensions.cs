using System;
using System.Runtime.CompilerServices;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.utils;
using XcyUI.views;
using static XcyUI.models.XFunctions;

namespace XcyUI.widgets.extensions
{
    public static class XGroupBuilderExtensions
    {
        public static XViewBuilder Space(this XViewBuilder builder,int space)
        {
            if (builder.View is XGroup)
            {
                ((XGroup)builder.View).Space = space.AsPx();
            }
            return builder;
        }

        public static XViewBuilder HorizontalAlignment(this XViewBuilder builder, XHorizontalAlignment alignment)
        {
            if (builder.View is XColumn)
            {
                ((XColumn)builder.View).HorizontalAlign = alignment;
            }
            return builder;
        }

        public static XViewBuilder VerticalAlignment(this XViewBuilder builder, XVerticalAlignment alignment)
        {
            if (builder.View is XColumn)
            {
                ((XColumn)builder.View).VerticalAlign = alignment;
            }
            return builder;
        }

        public static XViewBuilder FixedItem(this XViewBuilder builder, bool isFixed)
        {
            if (builder.View is XLazy)
            {
                ((XLazy)builder.View).IsFixedItem = isFixed;
            }
            return builder;
        }

        public static XViewBuilder ToggleHover(this XViewBuilder builder, XFunction<bool> function, string eventKey = "ToggleHover", [CallerLineNumber] int key = 0)
        {
            return ToggleHover(builder, (b, isHover) => function.Invoke(isHover), eventKey, key);
        }
        public static XViewBuilder ToggleHover(this XViewBuilder builder, XFunction<XViewBuilder, bool> function, string eventKey = "ToggleHover", [CallerLineNumber] int key = 0)
        {
            if (function == null)
            {
                builder.View.RemoveEvent(XEventType.Hover, eventKey);
                builder.View.RemoveEvent(XEventType.Leave, eventKey);
                return builder;
            }
            var isHover = XWidget.StateValueOf(false, key: key);
            return builder
                .OnHover((b, _) =>
                {
                    if (!isHover.Value)
                    {
                        function?.Invoke(b, true);
                    }
                    isHover.Value = true;

                }, eventKey)
                .OnLeave((v, _) =>
                {
                    if (isHover.Value)
                    {
                        function?.Invoke(v, false);
                    }
                    isHover.Value = false;
                }, eventKey)
                .BubbleEvent(XEventType.Hover)
                .BubbleEvent(XEventType.Leave);
        }        


        public static XViewBuilder FadeIn(this XViewBuilder builder, bool isIn = true, int delay = 0, [CallerLineNumber] int key = 0)
        {           
            var visibleState = XWidget.StateValueOf(true, true, key: key);            
            var animateValue = XWidget.AnimateFloatOf(visibleState, animate=> animate.Delay = delay, key: key);
            var isInState = XWidget.StateValueOf(isIn, true, key: key);
            builder.Bind(animateValue, (b, value) =>
            {
                value = isInState.Value ? value : (1 - value);
                builder.Alpha(value);
            });
            return builder;
        }


        public static XViewBuilder Scrollable(this XViewBuilder builder, bool isVertical = true, bool enableScollerBar = true, bool enableWheel = true)
        {
            
            if (builder.View is XGroup && builder.View.EventParams.Event(XEventType.Wheel) == null && builder.AsView<XGroup>()?.Scroller == null)
            {
                var view = builder.AsView<XGroup>();
                builder.Focusable(true);
                if (builder.View.Accessibility.Role == XAccessibilityRole.None)
                {
                    builder.AccessibilityRole(XAccessibilityRole.Group);
                }
                if (string.IsNullOrEmpty(builder.View.Accessibility.Name))
                {
                    builder.AccessibilityName("滚动区域");
                }
                builder.KeyScroll(isVertical);
                if (enableWheel)
                {
                    var wheel = view.EventParams.EventOrCreate(XEventType.Wheel);
                    wheel.AddFunction("default_whell", (v, info) => {
                        view.OnScolled(isVertical?info.IsVerticalWheel:isVertical, info.WheelSize);                        
                    });
                }
                builder.Clip();
                view.Scroller = new XScroller();
                view.Scroller.Init(view);
                var vBarBuilder = new XViewBuilder(view.Scroller.VerticalScollerBar);
                var hBarBuilder = new XViewBuilder(view.Scroller.HorizontalScollerBar);
                vBarBuilder.DefaultClickEffect().InterceptEvent(XEventType.Move)
                    .InterceptEvent(XEventType.Hover).EnableCache(true).InVisible(false);
                hBarBuilder.DefaultClickEffect().EnableCache(true)
                    .InterceptEvent(XEventType.Move)
                    .InterceptEvent(XEventType.Hover).InVisible(false);
                if (enableScollerBar)
                {
                    builder.ToggleHover(isHover =>
                    {
                        if (isHover)
                        {
                            XWidget.SetCurrentView(vBarBuilder.View);
                            vBarBuilder.Visible(true).FadeIn(true);
                            XWidget.SetCurrentView(hBarBuilder.View);
                            hBarBuilder.Visible(true).FadeIn(true);
                        }
                        else
                        {
                            XWidget.SetCurrentView(vBarBuilder.View);
                            vBarBuilder.FadeIn(false);
                            XWidget.SetCurrentView(hBarBuilder.View);
                            hBarBuilder.FadeIn(false);
                        }
                    });
                }
            }
            return builder;
        }

        private static XViewBuilder KeyScroll(this XViewBuilder builder, bool isVertical)
        {
            var view = builder.AsView<XGroup>();
            if (view == null)
            {
                return builder;
            }

            builder.View.EventParams.EventOrCreate(XEventType.KeyPress).AddFunction("default_key_scroll", (v, info) =>
            {
                var size = 50.AsPx();
                switch (info.KeyValue)
                {
                    case XKeyValue.Up:
                        view.OnScolled(true, size);
                        break;
                    case XKeyValue.Down:
                        view.OnScolled(true, -size);
                        break;
                    case XKeyValue.Left:
                        view.OnScolled(false, size);
                        break;
                    case XKeyValue.Right:
                        view.OnScolled(false, -size);
                        break;
                    case XKeyValue.Home:
                        view.OnScolled(isVertical, int.MaxValue);
                        break;
                    case XKeyValue.End:
                        view.OnScolled(isVertical, -int.MaxValue);
                        break;
                }
            });
            return builder;
        }

        public static XViewBuilder ScrolledTo(this XViewBuilder builder, bool isVertical, int size)
        {
            builder.AsView<XGroup>()?.Also(n =>
            {
                if (isVertical)
                {
                    n.Scroller.ScrollerHeight = size;
                }
                else
                {
                    n.Scroller.ScrollerWidth = size;
                }
            });
            return builder;
        }

        public static XViewBuilder Scrolled(this XViewBuilder builder, bool isVertical, int size)
        {
            builder.AsView<XGroup>()?.Also(n => n.OnScolled(isVertical, size));
            return builder;
        }

        public static XViewBuilder TranslationChilds(this XViewBuilder builder, int x, int y)
        {
            builder.AsView<XGroup>()?.Also(n =>
            {
                n.ScolledChilds(x, y);
                if (x != 0)
                {
                    n.Scroller.ScrollerWidth += x;
                }
                if(y!=0)
                {
                    n.Scroller.ScrollerHeight += y;
                }
                
            });
            return builder;
        }


        public static XViewBuilder ScrolledToIndex(this XViewBuilder builder, int index, int templateIndex= 0, bool isSmooth = false )
        {
            RenderImp.PostToQueue(() =>
            {
                ((XLazy)builder.View)?.Also(n => n.ScrolledToIndex(templateIndex, index,isSmooth));
            });
            return builder;
        }

        public static XViewBuilder FixedCells(this XViewBuilder builder, int cells)
        {
            ((XLazyGrid)builder.View)?.Also(n => n.Cells = cells);
            return builder;
        }

        public static XViewBuilder ContentAlignment(this XViewBuilder builder, XAlignment alignment)
        {

            builder.AsView<XBox>()?.Also(n => n.ContentAlign = alignment);
            return builder;
        }

        public static void Close(this XView view)
        {
            var removedEvent = view.EventParams.Event(XEventType.Removed);
            if (removedEvent != null)
            {
                removedEvent.Invoke(view, null);
            }
            else
            {
                view.Removed();
            }
        }

        public static XViewBuilder Cells(this XViewBuilder builder, int cells)
        {
            builder.AsView<XFlow>()?.Also(n => n.Cells = cells);
            return builder;
        }

        public static XViewBuilder Colspan(this XViewBuilder builder, int span)
        {
            builder.View.LayoutParams.Colspan = span;
            return builder;
        }

        public static XViewBuilder NotifyLazy(this XViewBuilder builder)
        {
            builder.View.NotifyLazy();
            builder.View.Invalidate();
            return builder;
        }

        public static XViewBuilder ResetParams(this XViewBuilder builder)
        {
            builder.View.LayoutParams.Reset();
            builder.View.Style.Reset();
            return builder;
        }
    }
}
