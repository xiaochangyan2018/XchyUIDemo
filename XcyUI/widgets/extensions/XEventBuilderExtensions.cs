using System;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;

namespace XcyUI.widgets.extensions
{
    public static class XEventBuilderExtensions
    {
        public static XModify OnDispatchEvent(this XModify builder, Action<XModify, XEventInfo> function, string key = "OnDispatchEvent")
        {
            builder.View.AddEvent(XEventType.DispatchEvent, key, (v, info) => function(builder, info));
            return builder;
        }

        public static XModify OnDraw(this XModify builder, Action<XModify> function,bool isOver = true, string key = "OnDraw")
        {
            builder.View.AddEvent(isOver ? XEventType.DrawOver : XEventType.DrawUnder, key, (v, info) => function(builder));
            return builder;
        }

        public static XModify Draw(this XModify builder, Action<XModify> function, string key = "Draw")
        {
            builder.View.AddEvent(XEventType.Draw, key, (v, info) => function(builder));
            return builder;
        }

        public static XModify OnHover(this XModify builder, Action<XModify, XEventInfo> function, string key = "OnHover")
        {
            builder.View.AddEvent(XEventType.Hover, key, (v, info) => function(builder, info));
            return builder;
        }

        public static XModify OnDown(this XModify builder, Action<XModify, XEventInfo> function, string key = "OnDown")
        {
            builder.View.AddEvent(XEventType.Down, key, (v, info) => function?.Invoke(builder, info));
            return builder;
        }

        private static XModify CanDown(this XModify builder)
        {
            builder.View.EventParams.EventOrCreate(XEventType.Down);
            return builder;
        }

        public static XModify OnMove(this XModify builder, Action<XModify, XEventInfo> function, string key = "OnMove")
        {
            builder.View.AddEvent(XEventType.Move, key, (v, info) => function(builder, info));
            return builder.CanDown();
        }

        public static XModify OnUp(this XModify builder, Action<XModify, XEventInfo> function, string key = "OnUp")
        {
            builder.View.AddEvent(XEventType.Up, key, (v, info) => function(builder, info));
            return builder.CanDown();
        }

        public static XModify OnLeave(this XModify builder, Action<XModify, XEventInfo> function, string key = "OnLeave")
        {
            builder.View.AddEvent(XEventType.Leave, key, (v, info) => function(builder, info));
            return builder;
        }

        public static XModify OnCancel(this XModify builder, Action<XModify, XEventInfo> function, string key = "OnCancel")
        {
            builder.View.AddEvent(XEventType.Cancel, key, (v, info) => function(builder, info));
            return builder.CanDown();
        }
        
        public static XModify OnFocused(this XModify builder, Action<XModify> function, string key = "OnFocused")
        {
            builder.View.AddEvent(XEventType.Focused, key, (v, info) => function?.Invoke(builder));
            return builder.CanDown();
        }

        public static XModify OnLossFocused(this XModify builder, Action<XModify> function, string key = "OnLossFocused")
        {
            builder.View.AddEvent(XEventType.LossFocused, key, (v, info) =>
            {
                function?.Invoke(builder);
            });
            return builder.CanDown();
        }

        public static XModify Click(this XModify builder, Action<XModify, XEventInfo> function, bool defaultEffect = true, string eventKey = "default_click")
        {
            builder.View.AddEvent(XEventType.Click,eventKey, (v, info) => function(builder, info));
            if (defaultEffect)
            {
                builder.DefaultClickEffect();
            }
            else
            {
                builder.View.EventParams.Remove(XEventType.Draw);
            }
            return builder.CanDown();
        }

        public static XModify DoubleClick(this XModify builder, Action<XModify, XEventInfo> function, bool defaultEffect = true, string eventKey = "default_double_click")
        {
            builder.View.AddEvent(XEventType.DoubleClick, eventKey, (v, info) => function(builder, info));
            if (defaultEffect)
            {
                builder.DefaultClickEffect();
            }
            else
            {
                builder.View.EventParams.Remove(XEventType.Draw);
            }
            return builder.CanDown();
        }

        public static XModify InterceptEvent(this XModify builder,XEventType eventType,bool isInercept = true)
        {
            var view = builder.View;
            view.EventParams.EventOrCreate(eventType).IsIntercept = isInercept;
            return builder.CanDown();
        }
        
        public static XModify Click(this XModify builder, Action function, bool defaultEffect = true, string eventKey = "default_click")
        {
            if (function == null)
            {
                return builder;
            }
            builder.View.AddEvent(XEventType.Click, eventKey, function);
            if (defaultEffect)
            {
                builder.DefaultClickEffect();
            }
            else
            {
                builder.View.EventParams.Remove(XEventType.Draw);
            }
            return builder.CanDown().InterceptEvent(XEventType.Move);
        }

        public static XModify OnScrolled(this XModify setter, Action<XModify, XEventInfo> function, string key = "OnScrolled")
        {
            setter.View.AddEvent(XEventType.Scolled, key, (v, info) =>
            {
                function.Invoke(setter, info);
            });
            return setter;
        }

        public static XModify MeasureEnd(this XModify setter, Action<XModify> function)
        {
            setter.View.AddEvent(XEventType.MeasureEnd, (v, info) =>
            {
                function.Invoke(new XModify(v));
            });
            return setter;
        }

        public static XModify MeasureStart(this XModify setter, Action<XModify> function)
        {
            setter.View.AddEvent(XEventType.MeasureStart, (v, info) =>
            {
                function.Invoke(new XModify(v));
            });
            return setter;
        }

        public static XModify LayoutStart(this XModify setter, Action<XModify> function)
        {
            setter.View.AddEvent(XEventType.LayoutStart, (v, info) =>
            {
                function.Invoke(new XModify(v));
            });
            return setter;
        }

        public static XModify LayoutEnd(this XModify setter, Action<XModify> function)
        {
            setter.View.AddEvent(XEventType.LayoutEnd, (v, info) =>
            {
                function.Invoke(new XModify(v));
            });
            return setter;
        }

        public static XModify OnDispose(this XModify setter, Action<XModify> function, string key = "default_dispose")
        {
            setter.View.AddEvent(XEventType.Dispose, key, (v, info) =>
            {
                function.Invoke(new XModify(v));
            });
            return setter;
        }


        public static XModify BubbleEvent(this XModify builder,XEventType type, bool isMust = true)
        {
            builder.View.EventParams.EventOrCreate(type).IsMust = isMust;
            return builder;
        }

        internal static XModify DefaultClickEffect(this XModify builder)
        {
            var key = "DefaultClickEffect";
            var backgroundColor = builder.View.Style.Background;
            var hoverColor = backgroundColor.Copy(XTheme.Color.Hover);
            var pressedColor = backgroundColor.Copy(XTheme.Color.Pressed);
            var view = builder.View;
            var style = new XStyle();
            style.Radius = builder.View.Style.Radius;
            var state = -1;
            builder
               .OnHover((b, e) =>
               {
                   if (state == 0) return;
                   state = 0;
                   style.Background = hoverColor;
                   b.Draw(d =>
                   {
                       RenderImp.DrawRect(builder.View.RenderRect, style);
                   }, key);
                   view.Invalidate();
               }, key)
               .OnDown((b,e) =>
               {
                   if (state == 1) return;
                   state = 1;
                   style.Background = pressedColor;
                   
                   view.Invalidate();
               }, key)
               .OnUp((b, e) =>
               {
                   if (state == 2) return;
                   state = 2;
                   style.Background = hoverColor;
                   view.Invalidate();
               }, key)
               .OnLeave((b,e)=>
               {
                   if (state == 3) return;
                   state = 3;
                   view.EventParams.RemoveFunction(XEventType.Draw, key);
                   view.Invalidate();
               }, key);
            return builder;
        }

        public static XModify TextChanged(this XModify builder, Action<XModify,string> onChanged, string eventKey = "TextChanged")
        {
            builder.AsView<XText>()?.Also(n =>
            {
                n.AddEvent(XEventType.TextChanged, eventKey, () =>
                {
                    onChanged.Invoke(builder, n.Text);
                });
            });
            return builder;
        }

        public static XModify LocationChanged(this XModify builder, Action<XModify> function, string key = "LocationChanged")
        {
            builder.View.AddEvent(XEventType.LocationChanged, key, (v, info) => function(new XModify(v)));
            return builder;
        }       

        public static XModify KeyPress(this XModify builder, Action<XModify, XEventInfo> onKeyPress, string key = "KeyPress")
        {
            builder.AsView<XInput>()?.Also(n =>
            {
                n.AddEvent(XEventType.KeyPress, key, (info) =>
                {
                    onKeyPress.Invoke(builder, info);
                });
            });
            return builder;
        }
    }
}
