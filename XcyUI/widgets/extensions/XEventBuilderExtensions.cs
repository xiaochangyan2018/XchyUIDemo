using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;
using static XcyUI.models.XFunctions;

namespace XcyUI.widgets.extensions
{
    public static class XEventBuilderExtensions
    {
        public static XViewBuilder OnDispatchEvent(this XViewBuilder builder, XFunction<XViewBuilder, XEventInfo> function, string key = "OnDispatchEvent")
        {
            builder.View.AddEvent(XEventType.DispatchEvent, key, (v, info) => function(builder, info));
            return builder;
        }

        public static XViewBuilder OnDraw(this XViewBuilder builder, XFunction<XViewBuilder> function,bool isOver = true, string key = "OnDraw")
        {
            builder.View.AddEvent(isOver ? XEventType.DrawOver : XEventType.DrawUnder, key, (v, info) => function(builder));
            return builder;
        }

        public static XViewBuilder Draw(this XViewBuilder builder, XFunction<XViewBuilder> function, string key = "Draw")
        {
            builder.View.AddEvent(XEventType.Draw, key, (v, info) => function(builder));
            return builder;
        }

        public static XViewBuilder OnHover(this XViewBuilder builder, XFunction<XViewBuilder, XEventInfo> function, string key = "OnHover")
        {
            builder.View.AddEvent(XEventType.Hover, key, (v, info) => function(builder, info));
            return builder;
        }

        public static XViewBuilder OnDown(this XViewBuilder builder, XFunction<XViewBuilder, XEventInfo> function, string key = "OnDown")
        {
            builder.View.AddEvent(XEventType.Down, key, (v, info) => function?.Invoke(builder, info));
            return builder;
        }

        private static XViewBuilder CanDown(this XViewBuilder builder)
        {
            builder.View.EventParams.EventOrCreate(XEventType.Down);
            return builder;
        }

        private static XViewBuilder ClickableAccessibility(this XViewBuilder builder)
        {
            builder.View.EventParams.Focusable = true;
            if (builder.View.Accessibility.Role == XAccessibilityRole.None)
            {
                builder.View.Accessibility.Role = XAccessibilityRole.Button;
            }
            builder.View.Accessibility.MergeDescendants = true;
            return builder;
        }

        public static XViewBuilder OnMove(this XViewBuilder builder, XFunction<XViewBuilder, XEventInfo> function, string key = "OnMove")
        {
            builder.View.AddEvent(XEventType.Move, key, (v, info) => function(builder, info));
            return builder.CanDown();
        }

        public static XViewBuilder OnUp(this XViewBuilder builder, XFunction<XViewBuilder, XEventInfo> function, string key = "OnUp")
        {
            builder.View.AddEvent(XEventType.Up, key, (v, info) => function(builder, info));
            return builder.CanDown();
        }

        public static XViewBuilder OnLeave(this XViewBuilder builder, XFunction<XViewBuilder, XEventInfo> function, string key = "OnLeave")
        {
            builder.View.AddEvent(XEventType.Leave, key, (v, info) => function(builder, info));
            return builder;
        }

        public static XViewBuilder OnCancel(this XViewBuilder builder, XFunction<XViewBuilder, XEventInfo> function, string key = "OnCancel")
        {
            builder.View.AddEvent(XEventType.Cancel, key, (v, info) => function(builder, info));
            return builder.CanDown();
        }
        
        public static XViewBuilder OnFocused(this XViewBuilder builder, XFunction<XViewBuilder> function, string key = "OnFocused")
        {
            builder.View.AddEvent(XEventType.Focused, key, (v, info) => function?.Invoke(builder));
            return builder.CanDown();
        }

        public static XViewBuilder OnLossFocused(this XViewBuilder builder, XFunction<XViewBuilder> function, string key = "OnLossFocused")
        {
            builder.View.AddEvent(XEventType.LossFocused, key, (v, info) =>
            {
                function?.Invoke(builder);
            });
            return builder.CanDown();
        }

        public static XViewBuilder Click(this XViewBuilder builder, XFunction<XViewBuilder, XEventInfo> function, bool defaultEffect = true, string eventKey = "default_click")
        {
            builder.View.AddEvent(XEventType.Click,eventKey, (v, info) => function(builder, info));
            builder.ClickableAccessibility();
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

        public static XViewBuilder DoubleClick(this XViewBuilder builder, XFunction<XViewBuilder, XEventInfo> function, bool defaultEffect = true, string eventKey = "default_double_click")
        {
            builder.View.AddEvent(XEventType.DoubleClick, eventKey, (v, info) => function(builder, info));
            builder.ClickableAccessibility();
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

        public static XViewBuilder InterceptEvent(this XViewBuilder builder,XEventType eventType,bool isInercept = true)
        {
            var view = builder.View;
            view.EventParams.EventOrCreate(eventType).IsIntercept = isInercept;
            return builder;
        }
        
        public static XViewBuilder Click(this XViewBuilder builder,XFunction function, bool defaultEffect = true, string eventKey = "default_click")
        {
            if (function == null)
            {
                return builder;
            }
            builder.View.AddEvent(XEventType.Click, eventKey, function);
            builder.ClickableAccessibility();
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

        public static XViewBuilder OnScrolled(this XViewBuilder setter, XFunction<XViewBuilder, XEventInfo> function, string key = "OnScrolled")
        {
            setter.View.AddEvent(XEventType.Scolled, key, (v, info) =>
            {
                function.Invoke(setter, info);
            });
            return setter;
        }

        public static XViewBuilder MeasureEnd(this XViewBuilder setter, XFunction<XViewBuilder> function)
        {
            setter.View.AddEvent(XEventType.MeasureEnd, (v, info) =>
            {
                function.Invoke(new XViewBuilder(v));
            });
            return setter;
        }

        public static XViewBuilder MeasureStart(this XViewBuilder setter, XFunction<XViewBuilder> function)
        {
            setter.View.AddEvent(XEventType.MeasureStart, (v, info) =>
            {
                function.Invoke(new XViewBuilder(v));
            });
            return setter;
        }

        public static XViewBuilder LayoutStart(this XViewBuilder setter, XFunction<XViewBuilder> function)
        {
            setter.View.AddEvent(XEventType.LayoutStart, (v, info) =>
            {
                function.Invoke(new XViewBuilder(v));
            });
            return setter;
        }

        public static XViewBuilder LayoutEnd(this XViewBuilder setter, XFunction<XViewBuilder> function)
        {
            setter.View.AddEvent(XEventType.LayoutEnd, (v, info) =>
            {
                function.Invoke(new XViewBuilder(v));
            });
            return setter;
        }

        public static XViewBuilder OnDispose(this XViewBuilder setter, XFunction<XViewBuilder> function, string key = "default_dispose")
        {
            setter.View.AddEvent(XEventType.Dispose, key, (v, info) =>
            {
                function.Invoke(new XViewBuilder(v));
            });
            return setter;
        }


        public static XViewBuilder BubbleEvent(this XViewBuilder builder,XEventType type, bool isMust = true)
        {
            builder.View.EventParams.EventOrCreate(type).IsMust = isMust;
            return builder;
        }

        internal static XViewBuilder DefaultClickEffect(this XViewBuilder builder)
        {
            var theme = XThemeManager.Theme;
            var key = "DefaultClickEffect";
            var backgroundColor = builder.View.Style.Background;
            var hoverColor = backgroundColor.Copy(theme.Colors.Hover);
            var pressedColor = backgroundColor.Copy(theme.Colors.Pressed);
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
                   builder.Draw(d => RenderImp.DrawRect(builder.View.RenderRect, style), key);
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

        public static XViewBuilder TextChanged(this XViewBuilder builder,XFunction<XViewBuilder,string> onChanged, string eventKey = "TextChanged")
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

        public static XViewBuilder LocationChanged(this XViewBuilder builder, XFunction<XViewBuilder> function, string key = "LocationChanged")
        {
            builder.View.AddEvent(XEventType.LocationChanged, key, (v, info) => function(new XViewBuilder(v)));
            return builder;
        }       

        public static XViewBuilder KeyPress(this XViewBuilder builder, XFunction<XViewBuilder, XEventInfo> onKeyPress, string key = "KeyPress")
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
