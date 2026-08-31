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
        private int maxScrollerHeight;
        private XEventInfo _scolledEventInfo = new XEventInfo();
        public XScroller()
        {
            Thickness = XTheme.Size.ScrollbarWidth.AsPx();
            MinDepth = XTheme.Size.ScrollbarMinHeight.AsPx();
            EnableScrolled = true;
        }

        public void Init(XView view)
        {
            HorizontalScollerBar = CreateScollerView(view);
            VerticalScollerBar = CreateScollerView(view);
            HorizontalScollerBar.LayoutParams.Height = Thickness;
            HorizontalScollerBar.Height = Thickness;
            HorizontalScollerBar.LayoutParams.Margin = new XSpace(0, 0, 0, XTheme.Size.ScrollbarMargin.AsPx());
            VerticalScollerBar.LayoutParams.Width = Thickness;
            VerticalScollerBar.Width = Thickness;
            VerticalScollerBar.LayoutParams.Margin = new XSpace(0, 0, XTheme.Size.ScrollbarMargin.AsPx(), 0);
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
                StartColor = XTheme.Color.InfoLight2
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

        private void OnMove(XView view, XEventInfo info)
        {
            if (view == VerticalScollerBar)
            {
                OnVerticalMove((XGroup)view.Parent, info);
            }
            else
            {
                OnHorizontalMove((XGroup)view.Parent, info);
            }
        }

        private void OnVerticalMove(XGroup view, XEventInfo info)
        {
            var contentRect = view.ContentRect;
            var childSize = view.ChildSize;
            var size = info.Point.Y - downPoint.Y;
            downPoint = info.Point;
            var wheelSize = size * ((float)childSize.Height - contentRect.Height) / (contentRect.Height - VerticalScollerBar.Height);
            var roundSize = -(int)Math.Round(wheelSize);
            var scolledSize = GetVerticalScolledWheelSize(view, roundSize);
            view.Invalidate();
            view.OnScolled(true, scolledSize, true);
        }

        private void OnHorizontalMove(XGroup view, XEventInfo info)
        {
            var contentRect = view.ContentRect;
            var childSize = view.ChildSize;
            var size = info.Point.X - downPoint.X;
            downPoint = info.Point;
            var wheelSize = size * ((float)childSize.Width - contentRect.Width) / (contentRect.Width - HorizontalScollerBar.Width);
            var roundSize = -(int)Math.Round(wheelSize);
            var scolledSize = GetHorizontalScolledWheelSize(view, roundSize);
            view.Invalidate();
            view.OnScolled(false, scolledSize, true);
        }

        public void OnScolled(XGroup view, bool isVertical, int scolledSize,bool isScollerBar)
        {
            var isCanScolled = false;
            if (isVertical)
            {
                isCanScolled = VerticalScolled(view, scolledSize,isScollerBar);
            }
            else
            {
                isCanScolled = HorizontalScolled(view, scolledSize,isScollerBar);
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

        public int GetVerticalScolledWheelSize(XGroup view, int wheelSize)
        {
            var oldHeight = ScrollerHeight;
            ScrollerHeight += wheelSize;
            UpdateScollerSize(view.ContentRect, view.ChildSize);
            var size = ScrollerHeight - oldHeight;
            TranslationVerticalScoller(view, size);
            return size;
        }
        
        public bool VerticalScolled(XGroup view, int wheelSize, bool isScollerBar)
        {
            var size = isScollerBar ? wheelSize : GetVerticalScolledWheelSize(view, wheelSize);
            view.ScolledChilds(0, size);
            view.ScolledEnd(true);
            _scolledEventInfo.X = 0;
            _scolledEventInfo.Y = size;
            _scolledEventInfo.Value = ScrollerHeight;
            view.EventParams.Event(XEventType.Scolled)?.Invoke(view, _scolledEventInfo);
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

        public int GetHorizontalScolledWheelSize(XGroup view, int wheelSize)
        {
            var oldWidth = ScrollerWidth;
            ScrollerWidth += wheelSize;
            UpdateScollerSize(view.ContentRect, view.ChildSize);
            var size = ScrollerWidth - oldWidth;
            TranslationHorizontalScoller(view, size);
            return size;
        }

        public bool HorizontalScolled(XGroup view, int wheelSize, bool isScollerBar)
        {
            var size = isScollerBar ? wheelSize : GetHorizontalScolledWheelSize(view, wheelSize);
            RenderImp.lockInvalidate = true;
            view.ScolledChilds(size, 0);
            view.ScolledEnd(false);
            _scolledEventInfo.X = size;
            _scolledEventInfo.Y = 0;
            _scolledEventInfo.Value = ScrollerWidth;
            view.EventParams.Event(XEventType.Scolled)?.Invoke(view, _scolledEventInfo);
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
