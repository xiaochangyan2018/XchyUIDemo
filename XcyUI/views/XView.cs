using System;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;

namespace XcyUI.views
{
    public class XView : XRenderRect
    {
        public int Key { get; set; }
        public XLayoutParams LayoutParams { get; private set; }
        public XEventParams EventParams { get; private set; }
        public XAccessibilityParams Accessibility { get; private set; }
        internal XDrawCache DrawCache { get; private set; }
        public XView Parent { get; set; }

        protected int measureHashCode;
        protected XPoint lastPoint;
        internal bool IsMeasured{ get; set; }
        public XRect ContentRect => getContentRect();

        protected virtual XRect getContentRect()
        {
            var rect = RenderRect;
            rect.ScaleRevert(LayoutParams.Padding);
            return rect;
        }

        public XPoint Location
        {
            get => new XPoint(X, Y);
            set
            {
                var isChanged = X != value.X || Y != value.Y;
                X = value.X;
                Y = value.Y;
                if (isChanged)
                {
                    EventParams.Event(XEventType.LocationChanged)?.Invoke(this, null);
                }
            }
        }
        public XView()
        {
            LayoutParams = new XLayoutParams();
            EventParams = new XEventParams();
            Accessibility = new XAccessibilityParams();
            DrawCache = new XDrawCache();
            IsMeasured = true;
        }

        public void StartLayout()
        {
            Measure();
            Layout();
        }

        public virtual void Layout()
        {
            EventParams.Event(XEventType.LayoutStart)?.Invoke(this, null);
            OnLayout();
            EventParams.Event(XEventType.LayoutEnd)?.Invoke(this, null);
            if (DrawCache != null)
            {
                DrawCache.IsRefreshCache = true;
            }
            EventParams.Remove(XEventType.FirstLayout)?.Invoke(this, null);
            if (lastPoint.X!=0 && lastPoint.Y != 0 && X!=0 && Y!=0)
            {
                if (lastPoint.X != X || lastPoint.Y != Y)
                {
                    EventParams.Event(XEventType.LocationChanged)?.Invoke(this, null);
                }
            }
            lastPoint = Location;
        }

        public virtual bool IsNeedMeasure()
        {
            return ((LayoutParams.IsFillWidth || LayoutParams.Weight != 0) && Width != Parent?.Width) || ((LayoutParams.IsFillHeight || LayoutParams.Weight != 0) && Height != Parent.Height) || measureHashCode != LayoutParams.MeasureHashCode();
        }

        public virtual void Measure()
        {
            if (!IsMeasured) return;
            EventParams.Event(XEventType.MeasureStart)?.Invoke(this, null);
            OnMeasure();
            measureHashCode = LayoutParams.MeasureHashCode();
            EventParams.Event(XEventType.MeasureEnd)?.Invoke(this, null);
        }

        protected virtual void OnMeasure()
        {
            this.MeasureSize();
        }
        protected virtual void OnLayout() { }

        public override void Translation(int x, int y)
        {
            base.Translation(x, y);
            if (lastPoint.X != 0 && lastPoint.Y != 0 && X != 0 && Y != 0)
            {
                if (lastPoint.X != X || lastPoint.Y != Y)
                {
                    EventParams.Event(XEventType.LocationChanged)?.Invoke(this, null);
                }
            }
            lastPoint = Location;
        }

        public virtual void OnEvent(XEventInfo info)
        {

        }

        public override void Draw()
        {
            var rect = RenderRect;
            var rootRect = this.RootView().RenderRect;
            if (LayoutParams.Visible != XVisibleType.Visible || (!rootRect.Intersect(rect) && !this.IsParentCache()))
            {
                return;
            }
            Style.ClipPadding = LayoutParams.Padding;
            if (Style.IsOverDraw)
            {
                EventParams?.Event(XEventType.Draw)?.Invoke(this, null);
                RenderImp.Draw(RenderRect, Style, DrawCache, OnDraw);
            }
            else
            {
                RenderImp.Draw(RenderRect, Style, DrawCache, OnDraw);
                EventParams?.Event(XEventType.Draw)?.Invoke(this, null);
            }
        }

        protected virtual void OnDraw()
        {
            EventParams?.Event(XEventType.DrawUnder)?.Invoke(this, null);
            DrawContent();
            EventParams?.Event(XEventType.DrawOver)?.Invoke(this, null);
        }

        protected virtual void DrawContent()
        {

        }

        public bool IsCache => DrawCache.EnableCache;
        public void EnableCache( bool enable, XCacheType type = XCacheType.Pictrue)
        {
            if (enable != DrawCache.EnableCache)
            {
                DrawCache.EnableCache = enable;
                DrawCache.CacheType = type;
            }
        }

        public virtual void Invalidate()
        {
            RenderImp.Invalidate(this);
        }

        public virtual void Dispose()
        {
            EventParams?.Event(XEventType.Dispose)?.Invoke(this, null);
            EventParams?.Clear();
            DrawCache?.Clear();
            if (this == XEvent.FocusView)
            {
                XEvent.ClearFocusView();
            }
        }
    }
}
