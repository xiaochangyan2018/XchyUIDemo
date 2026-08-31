using System;
using System.Runtime.CompilerServices;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;

namespace XcyUI.widgets.extensions
{
    public static class XGroupBuilderExtensions
    {
        public static XModify Space(this XModify builder,int space)
        {
            if (builder.View is XGroup)
            {
                ((XGroup)builder.View).Space = space.AsPx();
            }
            return builder;
        }

        public static XModify HorizontalAlignment(this XModify builder, XHorizontalAlignment alignment)
        {
            if (builder.View is XColumn)
            {
                ((XColumn)builder.View).HorizontalAlignment = alignment;
            }
            return builder;
        }

        public static XModify VerticalAlignment(this XModify builder, XVerticalAlignment alignment)
        {
            if (builder.View is XColumn)
            {
                ((XColumn)builder.View).VerticalAlignment = alignment;
            }
            return builder;
        }

        public static XModify FixedItem(this XModify builder, bool isFixed = true)
        {
            if (builder.View is XLazy)
            {
                ((XLazy)builder.View).IsFixedItem = isFixed;
            }
            return builder;
        }

        public static XModify ToggleHover(this XModify builder, Action<bool> function, string eventKey = "ToggleHover", [CallerLineNumber] int key = 0)
        {
            return ToggleHover(builder, (b, isHover) => function.Invoke(isHover), eventKey, key);
        }
        public static XModify ToggleHover(this XModify builder, Action<XModify, bool> function, string eventKey = "ToggleHover", [CallerLineNumber] int key = 0)
        {
            if (function == null)
            {
                builder.View.RemoveEvent(XEventType.Hover, eventKey);
                builder.View.RemoveEvent(XEventType.Leave, eventKey);
                return builder;
            }
            var isHover = XCompose.StateValueOf(false, key: key);
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


        public static XModify FadeIn(this XModify builder, bool isIn = true,int duration = 500, int delay = 0, float startValue = 0, Action finished = null, [CallerLineNumber] int key = 0)
        {           
            var visibleState = XCompose.StateValueOf(true, true, key: key);
            var enableCache = builder.View.DrawCache.EnableCache;
            var animateValue = XCompose.AnimateFloatOf(visibleState, animate =>
            {
                animate.Delay = delay;
                animate.Duration = duration;
                animate.OnFinished = () =>
                {
                    finished?.Invoke();
                    if (!enableCache)
                    {
                        builder.EnableCache(enableCache);
                    }
                };
            }, key: key);
            builder.Bind(animateValue, (b, value) =>
            {
                value = startValue + (1 - startValue) * value;
                value = isIn ? value : (1 - value);
                builder.Alpha(value);
            });
            return builder;
        }


        public static XModify Scrollable(this XModify builder, bool isVertical = true, bool enableScollerBar = true, bool enableWheel = true)
        {
            
            if (builder.View is XGroup && builder.View.EventParams.Event(XEventType.Wheel) == null && builder.AsView<XGroup>()?.Scroller == null)
            {
                var view = builder.AsView<XGroup>();
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
                var vBarBuilder = new XModify(view.Scroller.VerticalScollerBar);
                var hBarBuilder = new XModify(view.Scroller.HorizontalScollerBar);
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
                            vBarBuilder.Background(XTheme.Color.InfoLight2);
                            hBarBuilder.Background(XTheme.Color.InfoLight2);
                            vBarBuilder.View.RefreshCache();
                            hBarBuilder.View.RefreshCache();
                            XCompose.SetCurrentView(vBarBuilder.View);
                            vBarBuilder.Visible(true).FadeIn(true, 250);
                            XCompose.SetCurrentView(hBarBuilder.View);
                            hBarBuilder.Visible(true).FadeIn(true, 250);
                        }
                        else
                        {
                            XCompose.SetCurrentView(vBarBuilder.View);
                            vBarBuilder.FadeIn(false,250);
                            XCompose.SetCurrentView(hBarBuilder.View);
                            hBarBuilder.FadeIn(false,250);
                        }
                    });
                }
            }
            return builder;
        }

        public static XModify ScrolledTo(this XModify builder, bool isVertical, int size)
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

        public static XModify Scrolled(this XModify builder, bool isVertical, int size)
        {
            builder.AsView<XGroup>()?.Also(n => n.OnScolled(isVertical, size));
            return builder;
        }

        public static XModify TranslationChilds(this XModify builder, int x, int y)
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


        public static XModify ScrolledToIndex(this XModify builder, int index, int templateIndex= 0, bool isSmooth = false )
        {
            RenderImp.PostToQueue(() =>
            {
                ((XLazy)builder.View)?.Also(n => n.ScrolledToIndex(templateIndex, index,isSmooth));
            });
            return builder;
        }

        public static XModify FixedCells(this XModify builder, int cells)
        {
            ((XLazyGrid)builder.View)?.Also(n => n.Cells = cells);
            return builder;
        }

        public static XModify ContentAlignment(this XModify builder, XAlignment alignment)
        {

            builder.AsView<XBox>()?.Also(n => n.ContentAlignment = alignment);
            return builder;
        }

        public static void Close(this XView view)
        {
            var removedEvent = view.EventParams.Event(XEventType.Removed);
            if (removedEvent != null)
            {
                removedEvent.Invoke(view, XEventInfo.Empty);
            }
            else
            {
                view.Removed();
            }
        }

        public static XModify Cells(this XModify builder, int cells)
        {
            builder.AsView<XFlow>()?.Also(n => n.Cells = cells);
            return builder;
        }

        public static XModify Colspan(this XModify builder, int span)
        {
            builder.View.LayoutParams.Colspan = span;
            return builder;
        }

        public static XModify NotifyLazy(this XModify builder)
        {
            builder.View.NotifyLazy();
            builder.View.Invalidate();
            return builder;
        }

        public static XModify ResetParams(this XModify builder)
        {
            builder.View.LayoutParams.Reset();
            builder.View.Style.Reset();
            return builder;
        }
    }
}
