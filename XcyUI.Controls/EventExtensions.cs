using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.utils;
using XcyUI.views;
using XcyUI.widgets;
using XcyUI.widgets.extensions;

namespace XcyUI.Controls
{
    public static class EventExtensions
    {
        private static Dictionary<XAlignment, XRect> ToResizeRect(XView view, bool left = false, bool top = false, bool right = false, bool bottom = false)
        {
            var renderRect = view.RenderRect;
            var dist = 4.AsPx();
            var leftRect = new XRect(renderRect.X, renderRect.Y, dist, renderRect.Height);
            var topRect = new XRect(renderRect.X, renderRect.Y, renderRect.Width, dist);
            var rightRect = new XRect(renderRect.Right - dist, renderRect.Y, dist, renderRect.Height);
            var bottomRect = new XRect(renderRect.X, renderRect.Bottom - dist, renderRect.Width, dist);

            var rects = new Dictionary<XAlignment, XRect>();
            if (left && top) rects.Add(XAlignment.LeftTop, leftRect.TopIntersectRect(topRect));
            if (left && bottom) rects.Add(XAlignment.LeftBottom, leftRect.TopIntersectRect(bottomRect));
            if (right && top) rects.Add(XAlignment.RightTop, rightRect.TopIntersectRect(topRect));
            if (right && bottom) rects.Add(XAlignment.RightBottom, rightRect.TopIntersectRect(bottomRect));

            if (left) rects.Add(XAlignment.LeftCenter, leftRect);
            if (top) rects.Add(XAlignment.TopCenter, topRect);
            if (right) rects.Add(XAlignment.RightCenter, rightRect);
            if (bottom) rects.Add(XAlignment.BottomCenter, bottomRect);
            return rects;
        }

        public static XModify Resize(this XModify builder, bool left = false, bool top = false, bool right = false, bool bottom = false, bool layout = true)
        {
            string eventKey = "Resize_key";
            var align = XAlignment.None;
            XPoint point = new XPoint();
            var view = builder.View;
            var isDefaultCursor = true;
            builder
                .OnHover((_, info) =>
                {
                    var rects = ToResizeRect(view, left, top, right, bottom);
                    var value = rects.FirstOrDefault(n => n.Value.Contain(info.Point));
                    align = value.Key;
                    if (value.Value.Contain(info.Point))
                    {
                        RenderImp.SetCursor(value.Key);
                        isDefaultCursor = false;
                    }
                    else if (!isDefaultCursor)
                    {
                        RenderImp.SetCursor(value.Key);
                        isDefaultCursor = true;
                    }
                    builder
                    .InterceptEvent(XEventType.Move, value.Key != XAlignment.None)
                    .InterceptEvent(XEventType.Hover, value.Key != XAlignment.None)
                    .InterceptEvent(XEventType.Down, value.Key != XAlignment.None);
                }, key: eventKey)
                .OnLeave((_, info) =>
                {
                    RenderImp.SetCursor(XAlignment.None);
                }, key: eventKey)
                .OnDown((_, info) =>
                {
                    point = info.Point;
                }, key: eventKey)
                .BubbleEvent(XEventType.Hover)
                .BubbleEvent(XEventType.Leave);

            var fun = builder.View.EventParams.Event(XEventType.Move);
            if (fun?.Value != null)
            {
                var size = (XSize)fun.Value;
                if (size.Width > 0)
                {
                    builder.Width(size.Width.AsDp());
                }
                if (size.Height > 0)
                {
                    builder.Height(size.Height.AsDp());
                }
            }

            builder.View.AddEvent(XEventType.Move, "resize_move", (v, info) =>
            {
                if (align == XAlignment.None) return;
                var x_dist = info.Point.X - point.X;
                var y_dist = info.Point.Y - point.Y;
                switch (align)
                {
                    case XAlignment.RightCenter:
                        x_dist = -x_dist;
                        y_dist = 0;
                        break;
                    case XAlignment.LeftCenter:
                        y_dist = 0;
                        break;
                    case XAlignment.BottomCenter:
                        y_dist = -y_dist;
                        x_dist = 0;
                        break;
                    case XAlignment.TopCenter:
                        x_dist = 0;
                        break;
                    case XAlignment.RightBottom:
                        y_dist = -y_dist;
                        x_dist = -x_dist;
                        break;
                    case XAlignment.LeftBottom:
                        y_dist = -y_dist;
                        break;
                    case XAlignment.RightTop:
                        x_dist = -x_dist;
                        break;
                }
                var height = v.Height;
                var width = v.Width;
                height -= y_dist;
                width -= x_dist;
                if (layout)
                {
                    width = Math.Max(width, 0);
                    height = Math.Max(height, 0);

                    var size = new XSize(0, 0);
                    if (y_dist != 0)
                    {
                        view.LayoutParams.Height = height;
                        size.Height = height;
                    }
                    if (x_dist != 0)
                    {
                        v.LayoutParams.Width = width;
                        size.Width = width;
                    }
                    v.EventParams.Event(XEventType.Move).Value = size;
                    v.BubbleUpLayout();
                }
                info.Value = (x_dist, y_dist, align);
                v.EventParams.Event(XEventType.Resize)?.Invoke(v, info);
                point = info.Point;
                (v.Parent ?? v).Invalidate();
            });
            return builder;
        }


        public static XModify HoverBackgroundAllColor(this XModify builder, XColor color, string key = "HoverBackgroundAllColor")
        {
            var style = builder.View.Style;
            var old = style.Background.StartColor;
            builder.ToggleHover((b,isHover)=>
            {
                b.BackgroundAll(isHover ? color : old).View.Invalidate();
            });
            return builder;
        }

        public static XModify HoverBackgroundColor(this XModify builder, XColor color, bool isFadeIn = false, string key = "HoverBackgroundColor")
        {
            var style = builder.View.Style;
            var old = style.Background;
            var backgound = style.Background.Copy(color);
            XState<bool> visibleState = null;
            XState<float> animateValue = null;
            if (isFadeIn)
            {
                visibleState = XCompose.StateValueOf(false);
                animateValue = XCompose.AnimateFloatOf(visibleState, animate =>
                {
                    animate.OnCancel = () =>
                    {
                        animateValue.Value = 0;
                    };
                }, false);
            }
            builder
                .OnHover((_, info) =>
                {
                    if (isFadeIn)
                    {
                        visibleState.Value = true;
                    }
                    else
                    {
                        builder.View.Style.Background = backgound;
                    }
                    builder.View.Invalidate();
                }, key)
                .OnLeave((_, info) =>
                {
                    if (isFadeIn)
                    {
                        visibleState.Value = false;
                    }
                    builder.View.Style.Background = old;
                    builder.View.Invalidate();
                }, key)
                .BubbleEvent(XEventType.Hover)
                .BubbleEvent(XEventType.Leave);
            if (isFadeIn)
            {
                builder.Bind(animateValue, (b, value) =>
                {
                    if (visibleState.Value)
                    {
                        b.Background(backgound.Copy(value)).RefreshParentCache();
                    }
                });
            }
            return builder;
        }
        public static XModify HoverBorderColor(this XModify builder, XColor color)
        {
            string eventKey = "HoverBorderColor";
            if (color.IsEmpty)
            {
                builder.ToggleHover(isHover => { }, eventKey);
                return builder;
            }
            var oldBorder = builder.View.Style.Border;
            var oldColor = builder.View.Style.Border.Color;
            return builder.ToggleHover(isHover =>
            {
                var border = isHover ? oldBorder.Copy(color) : oldBorder;
                builder.Border(border).View.Invalidate();
            }, eventKey);
        }

        public static XModify Drag(this XModify builder, XDragType type = XDragType.Vertical, Action<XModify, XEventInfo> onDrag = null, XRect? dragRangRect = null, [CallerLineNumber] int key = 0)
        {
            var point = XCompose.StateValueOf(new XPoint());
            var zIndex = builder.View.LayoutParams.ZIndex;
            builder
                .OnDown((b, info) =>
                {
                    point.Value = info.Point;
                    builder.View.LayoutParams.ZIndex = int.MaxValue;
                    ((XGroup)builder.View.Parent).UpdateDrawViews();
                }, "dragDown")
                .OnUp((b, info) =>
                {
                    b.View.LayoutParams.ZIndex = zIndex;
                    ((XGroup)builder.View.Parent).UpdateDrawViews();
                }, "dragUp")
                .OnMove((b, info) =>
                {
                    var x = type == XDragType.Vertical ? 0 : info.X - point.Value.X;
                    var y = type == XDragType.Horizontal ? 0 : info.Y - point.Value.Y;
                    builder.View.Translation(x, y);
                    var rangRect = dragRangRect;
                    rangRect = rangRect ?? builder.View.Parent.RenderRect;
                    var dragPoint = builder.View.Location;
                    if (type == XDragType.All || type == XDragType.Horizontal)
                    {
                        dragPoint.X = Math.Max(rangRect.Value.X, dragPoint.X);
                        dragPoint.X = Math.Min(rangRect.Value.Right - builder.View.Width, dragPoint.X);
                    }
                    if (type == XDragType.All || type == XDragType.Vertical)
                    {
                        dragPoint.Y = Math.Max(rangRect.Value.Y, dragPoint.Y);
                        dragPoint.Y = Math.Min(rangRect.Value.Bottom - builder.View.Height, dragPoint.Y);
                    }
                    builder.View.Location = dragPoint;
                    point.Value = info.Point;
                    onDrag?.Invoke(builder, info);
                    builder.View.Invalidate();

                }, "dragMove");
            return builder.InterceptEvent(XEventType.Move);
        }

        public static XModify HoverCursor(this XModify builder, XCursorType cursorType, string eventKey = "hoverCursor", [CallerLineNumber] int key = 0)
        {
            return builder.OnHover((b, info) =>
            {
                RenderImp.SetCursor(cursorType);
            }, eventKey)
            .OnLeave((b, info) =>
            {
                RenderImp.SetCursor(XCursorType.Arrow);
            }, eventKey)
            .BubbleEvent(XEventType.Hover)
            .BubbleEvent(XEventType.Leave)
            .OnDispose(b =>
            {
                if(b.View == XEvent.HoverView)
                {
                    RenderImp.SetCursor(XCursorType.Arrow);
                }
            }, eventKey);
        }

        public static XModify OnResize(this XModify builder, Action<XModify> function, string key = "OnResize")
        {
            builder.View.AddEvent(XEventType.Resize, key, (v, info) => function(new XModify(v)));
            return builder;
        }

        public static XModify OnResize(this XModify builder, Action<XModify,XEventInfo> function, string key = "OnResize")
        {
            builder.View.AddEvent(XEventType.Resize, key, (v, info) => function(new XModify(v),info));
            return builder;
        }

        public static XModify HoverColor(this XModify builder, XColor color, string key = "HoverColor")
        {
            XBrush oldColor = default;
            builder.View.ModifyChild(n =>
            {
                if(n is XText)
                {
                    oldColor = ((XText)n).Font.Color;
                }

                if(n is XIcon)
                {
                    oldColor = ((XIcon)n).Color;
                }
            });
            builder.ToggleHover(isHover =>
            {
                builder.View.ModifyChild(n =>
                {
                    if (n is XText)
                    {
                        ((XText)n).Font.Color = isHover ? ((XText)n).Font.Color.Copy(color) : oldColor;
                    }

                    if (n is XIcon)
                    {
                        ((XIcon)n).Color = isHover ? new XBrush(color) : oldColor;
                    }
                });
                builder.View.Invalidate();
            }, "HoverColor");
            return builder;
        }

        public static XModify OnEnter(this XModify builder, Action<string> funtion, string key = "OnEnter")
        {
            return builder.FirstInput(n =>
            {
                n.KeyPress((b, info) =>
                 {
                     if (info.KeyValue == XKeyValue.Enter)
                     {
                         funtion?.Invoke(n.Content());
                     }
                 }, key);
            });
        }
    }
}
