using System;
using System.Diagnostics;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;

namespace XcyUI.views
{
    public class XScroller
    {
        public XView HorizontalScollerBar { get; private set; }
        public XView VerticalScollerBar { get; private set; }
        public int Thickness { get; internal set; }
        public int MinDepth { get; internal set; }
        public int ScrollerHeight { get; internal set; }
        public int ScrollerWidth { get; internal set; }
        public bool EnableScrolled { get; set; }

        private XPoint downPoint;
        public XScroller()
        {
            Thickness = XThemeManager.Theme.Sizes.ScrollbarWidth.AsPx();
            MinDepth = XThemeManager.Theme.Sizes.ScrollbarMinHeight.AsPx();
            EnableScrolled = true;
        }

        public void Init(XView view)
        {
            HorizontalScollerBar = CreateScollerView(view);
            VerticalScollerBar = CreateScollerView(view);
            HorizontalScollerBar.Accessibility.Role = XAccessibilityRole.ScrollBar;
            HorizontalScollerBar.Accessibility.Name = "水平滚动条";
            VerticalScollerBar.Accessibility.Role = XAccessibilityRole.ScrollBar;
            VerticalScollerBar.Accessibility.Name = "垂直滚动条";
            HorizontalScollerBar.LayoutParams.Height = Thickness;
            HorizontalScollerBar.Height = Thickness;
            HorizontalScollerBar.LayoutParams.Margin = new XSpace(0, 0, 0, XThemeManager.Theme.Sizes.ScrollbarMargin.AsPx());
            VerticalScollerBar.LayoutParams.Width = Thickness;
            VerticalScollerBar.Width = Thickness;
            VerticalScollerBar.LayoutParams.Margin = new XSpace(0, 0, XThemeManager.Theme.Sizes.ScrollbarMargin.AsPx(), 0);
        }

        public void UpdateScollerSize(XRect contentRect, XSize childSize)
        {            
            var maxScollerHeight = childSize.Height - contentRect.Height;
            maxScollerHeight = Math.Max(0, maxScollerHeight);
            ScrollerHeight = Math.Min(0, ScrollerHeight);
            ScrollerHeight = Math.Max(ScrollerHeight, -maxScollerHeight);

            var maxScollerWidth = childSize.Width - contentRect.Width;
            maxScollerWidth = Math.Max(0, maxScollerWidth);
            ScrollerWidth = Math.Min(0, ScrollerWidth);
            ScrollerWidth = Math.Max(ScrollerWidth, -maxScollerWidth);
        }

        public void Layout(XGroup view)
        {
            var contentRect = view.ContentRect;
            var renderRect = view.RenderRect;
            var childSize = view.ChildSize;
            var maxScollerHeight = childSize.Height - contentRect.Height;
            VerticalScollerBar.Height = 0;
            if (maxScollerHeight > 0)
            {
                VerticalScollerBar.Height = Math.Max(MinDepth, contentRect.Height - maxScollerHeight);
                VerticalScollerBar.X = renderRect.Right - VerticalScollerBar.Width - (int)VerticalScollerBar.LayoutParams.Margin.Right;
                VerticalScollerBar.Y = contentRect.Y - (int)(ScrollerHeight * ((float)contentRect.Height - VerticalScollerBar.Height) / (childSize.Height - contentRect.Height));
            }

            var maxScollerWidth = childSize.Width - contentRect.Width;
            HorizontalScollerBar.Width = 0;
            if (maxScollerWidth > 0)
            {
                HorizontalScollerBar.Width = Math.Max(MinDepth, contentRect.Width - maxScollerWidth);
                HorizontalScollerBar.Y = contentRect.Bottom - HorizontalScollerBar.Height - (int)HorizontalScollerBar.LayoutParams.Margin.Bottom;
                HorizontalScollerBar.X = contentRect.X - (int)(ScrollerWidth * ((float)contentRect.Width - HorizontalScollerBar.Width) / (childSize.Width - contentRect.Width));
            }
            VerticalScollerBar.DrawCache.IsRefreshCache = true;
            HorizontalScollerBar.DrawCache.IsRefreshCache = true;
        }

        public void Draw()
        {
            HorizontalScollerBar?.Draw();
            VerticalScollerBar?.Draw();
        }

        private XView CreateScollerView(XView parent)
        {
            var view = new XView();
            view.Parent = parent;
            view.Style.Background = new XBrush()
            {
                StartColor = XThemeManager.Theme.Colors.InfoLight2
            };
            view.Style.Radius = new XSpace(Thickness / 2);
            var defaultEventKey = "default_scoller";
            view.EventParams.EventOrCreate(XEventType.Click).AddFunction(defaultEventKey, OnClick);
            view.EventParams.EventOrCreate(XEventType.Down).AddFunction(defaultEventKey, OnClick);
            view.EventParams.EventOrCreate(XEventType.Move).AddFunction(defaultEventKey, OnMove);
            return view;
        }

        private void OnClick(XView view, XEventInfo info)
        {
            downPoint = info.Point;
        }

        private readonly Stopwatch stopwatch = new Stopwatch();
        private void OnMove(XView view, XEventInfo info)
        {
            stopwatch.Restart();
            if (view == VerticalScollerBar)
            {
                OnVerticalMove((XGroup)view.Parent, info);
            }
            else
            {
                OnHorizontalMove((XGroup)view.Parent, info);
            }
            stopwatch.Stop();
            Console.WriteLine($"VerticalScolled times:{stopwatch.ElapsedMilliseconds}");
        }

        private void OnVerticalMove(XGroup view, XEventInfo info)
        {
            var contentRect = view.ContentRect;
            var childSize = view.ChildSize;
            var size = info.Point.Y - downPoint.Y;
            downPoint = info.Point;
            var wheelSize = size * ((float)childSize.Height - contentRect.Height) / (contentRect.Height - VerticalScollerBar.Height);
            view.OnScolled(true, -(int)Math.Round(wheelSize));
        }

        private void OnHorizontalMove(XGroup view, XEventInfo info)
        {
            var contentRect = view.ContentRect;
            var childSize = view.ChildSize;
            var size = info.Point.X - downPoint.X;
            downPoint = info.Point;
            var wheelSize = size * ((float)childSize.Width - contentRect.Width) / (contentRect.Width - HorizontalScollerBar.Width);
            view.OnScolled(false, -(int)Math.Round(wheelSize));
        }

        public void OnScolled(XGroup view, bool isVertical, int scolledSize)
        {
            var isCanScolled = false;
            RenderImp.lockInvalidate = true;
            if (isVertical)
            {
                isCanScolled = VerticalScolled(view, scolledSize);
            }
            else
            {
                isCanScolled = HorizontalScolled(view, scolledSize);
            }
            if (!isCanScolled)
            {
                XEvent.PopPreviousWheelEvent(view.Parent, new XEventInfo()
                {
                    EventType = XEventType.Wheel,
                    WheelSize = scolledSize,
                    IsVerticalWheel = isVertical
                });
            }            
        }
        
        public bool VerticalScolled(XGroup view, int wheelSize)
        {
            
            var oldHeight = ScrollerHeight;
            ScrollerHeight += wheelSize;
            UpdateScollerSize(view.ContentRect, view.ChildSize);
            var size = ScrollerHeight - oldHeight;
            TranslationVerticalScoller(view, size);
            view.ScolledChilds(0, size);
            view.ScolledEnd(true);
            view.EventParams.Event(XEventType.Scolled)?.Invoke(view, new XEventInfo()
            {
                X = 0,
                Y = size,
                Value = ScrollerHeight
            });
            RenderImp.lockInvalidate = false;
            view.Invalidate();       
            return size != 0;
        }
        private void TranslationVerticalScoller(XGroup view, int size)
        {
            var childSize = view.ChildSize;
            var renderRect = view.RenderRect;
            var y = ((float)renderRect.Height - VerticalScollerBar.Height) / (childSize.Height - view.ContentRect.Height) * -ScrollerHeight;
            VerticalScollerBar.Y = (int)y + renderRect.Y;
        }

        public bool HorizontalScolled(XGroup view, int wheelSize)
        {
            var oldWidth = ScrollerWidth;
            ScrollerWidth += wheelSize;
            UpdateScollerSize(view.ContentRect, view.ChildSize);
            var size = ScrollerWidth - oldWidth;
            TranslationHorizontalScoller(view, size);
            view.ScolledChilds(size, 0);
            view.ScolledEnd(false);
            view.EventParams.Event(XEventType.Scolled)?.Invoke(view, new XEventInfo()
            {
                X = size,
                Y = 0,
                Value = ScrollerWidth
            });
            RenderImp.lockInvalidate = false;
            view.Invalidate();
            return size != 0;
        }

        private void TranslationHorizontalScoller(XGroup view, int size)
        {
            var childSize = view.ChildSize;
            var renderRect = view.RenderRect;
            float x = ((float)renderRect.Width - HorizontalScollerBar.Width) / (childSize.Width - view.ContentRect.Width) * -ScrollerWidth;
            HorizontalScollerBar.X = (int)x + renderRect.X;
        }
    }
}
