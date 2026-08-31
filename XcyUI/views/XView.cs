using System;
using System.Reflection;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;

namespace XcyUI.views
{
    public class XView
    {
        public string Key;
        public int Tabindex = -1;
        public int X;
        public int Y;
        public int Width;
        public int Height;
        protected XStyle _style;
        protected XLayoutParams _layoutParams;
        protected XEventParams _eventParams;
        public XDrawCache Cache;

        public XView Parent;
        protected int measureHashCode;

        public XStyle Style => _style ??= new();
        public XLayoutParams LayoutParams => _layoutParams ??= new();
        public XEventParams EventParams => _eventParams ??= new();
        internal XDrawCache DrawCache => Cache ??= new();

        public XRect ContentRect => RenderRect.InsetSpace(LayoutParams.Padding);

        public XRect RenderRect => new XRect(X, Y, Width, Height);

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
                    InvokeEvent(XEventType.LocationChanged);
                }
            }
        }

        public void StartLayout()
        {
            Measure();
            Layout();
        }

        public virtual void Layout()
        {
            InvokeEvent(XEventType.LayoutStart);
            _eventParams?.Event(XEventType.LayoutStart)?.Invoke(this, XEventInfo.Empty);
            OnLayout();
            InvokeEvent(XEventType.LayoutEnd);
            if (Cache != null)
            {
                Cache.IsRefreshCache = true;
            }
        }

        public virtual bool IsNeedMeasure()
        {
            return ((LayoutParams.IsFillWidth || LayoutParams.Weight != 0) && Width != Parent?.Width) || ((LayoutParams.IsFillHeight || LayoutParams.Weight != 0) && Height != Parent.Height) || measureHashCode != LayoutParams.MeasureHashCode();
        }

        public virtual void Measure()
        {
            if (_eventParams?.Contains(XEventType.MeasureStart) == true)
            {
                this.MeasureSize();
                InvokeEvent(XEventType.MeasureStart);
            }
            this.MeasureSize();
            OnMeasure();
            measureHashCode = LayoutParams.MeasureHashCode();
            InvokeEvent(XEventType.MeasureEnd);
        }

        protected virtual void OnMeasure()
        {
            this.MeasureMaxOrMin();
        }
        protected virtual void OnLayout() { }

        public void MoveTo(int x, int y)
        {
            Translation(x - X, y - Y);
        }

        public virtual void Translation(int x, int y)
        {
            Location = new XPoint(X + x, Y + y);
        }
        public void InvokeEvent(XEventType type)
        {
            _eventParams?.Event(type)?.Invoke(this, XEventInfo.Empty.Copy(type));
        }
        public virtual void OnEvent(XEventInfo info)
        {
        }

        public virtual void Draw()
        {
            var rect = RenderRect;
            var rootRect = this.RootView().RenderRect;
            if (LayoutParams.Visible != XVisibleType.Visible || (!rootRect.Intersect(rect) && !this.IsParentCache()))
            {
                return;
            }
            if (_style == null)
            {
                OnDraw();
                return;
            }
            _style.ClipPadding = LayoutParams.Padding;           
            if (_style.IsOverDraw)
            {
                InvokeEvent(XEventType.Draw);
                RenderImp.Draw(RenderRect, _style, DrawCache, OnDraw);
            }
            else
            {
                RenderImp.Draw(RenderRect, _style, DrawCache, OnDraw);
                InvokeEvent(XEventType.Draw);
            }
        }

        protected virtual void OnDraw()
        {
            InvokeEvent(XEventType.DrawUnder);
            DrawContent();
            InvokeEvent(XEventType.DrawOver);
        }

        protected virtual void DrawContent()
        {

        }
        public bool IsCache => Cache?.EnableCache == true;

        public void SetBlurSigma(int blurSigma)
        {
            DrawCache.BlurSigma = blurSigma;
        }
        public void EnableCache(bool enable, XCacheType type = XCacheType.Pictrue)
        {
            if (DrawCache.EnableCache != enable)
            {
                Cache.EnableCache = enable;
                Cache.CacheType = type;
                if (!Cache.EnableCache)
                {
                    Cache.Clear();
                }
            }
        }

        public virtual void Invalidate()
        {
            RenderImp.Invalidate(this);
        }

        public virtual void Dispose()
        {
            InvokeEvent(XEventType.Dispose);
            _eventParams?.Clear();
            Cache?.Clear();
            if (this == XEvent.FocusView)
            {
                XEvent.ClearFocusView();
            }
        }
    }
}
