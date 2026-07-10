using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.utils;

namespace XcyUI.views
{
    public class XGroup: XView
    {
        internal List<XView> Childs { get; private set; }
        public XScroller Scroller { get; set; }
        public int Space { get; set; }
        protected List<XView> visibleChilds = new List<XView>();
        protected List<XView> drawViews;
        internal List<XView> DrawViews { get => drawViews; }
        protected int ChildRectWidth;
        protected int ChildRectHeight;
        public XSize ChildSize => new XSize(ChildRectWidth, ChildRectHeight);
        public XGroup()
        {
            Childs = new List<XView>();
        }        

        protected void FillVisibleViews()
        {
            visibleChilds.Clear();
            foreach (var view in Childs)
            {
                if (view != null && view.LayoutParams.Visible != XVisibleType.Gone)
                {
                    visibleChilds.Add(view);
                }
            }
        }

        public void AddView(XView view)
        {
            view.Parent = this;
            Childs.Add(view);
            XAccessibility.NotifyStructureChanged(this);
        }

        public void InsertView(int index,XView view)
        {
            if(index >=0 && index <= Childs.Count)
            {
                view.Parent = this;
                Childs.Insert(index, view);
                XAccessibility.NotifyStructureChanged(this);
            }
        }

        public void RemoveView(XView view)
        {
            view.Dispose();
            var childs = new List<XView>(Childs);
            childs.Remove(view);
            Childs = childs;
            UpdateDrawViews();
            XAccessibility.NotifyStructureChanged(this);
        }

        public override void Translation(int x, int y)
        {
            base.Translation(x, y);
            Childs?.ForEach(n => n.Translation(x, y));
        }

        public virtual void ScolledChilds(int x, int y)
        {
            Childs?.ForEach(n => n.Translation(x, y));
        }

        internal virtual void ScolledEnd(bool isVertical)
        {

        }


        public virtual void OnScolled(bool isVertical, int scolledSize)
        {
            if (scolledSize == 0) return;
            Scroller?.OnScolled(this, isVertical, scolledSize);
        }

        public override void Measure()
        {
            if (!IsMeasured) return;
            FillVisibleViews();
            OnMeasure();
            if (Scroller?.EnableScrolled == true)
            {
                UpdateScollerSize();
            }
            
            measureHashCode = LayoutParams.MeasureHashCode();
            EventParams.Event(XEventType.MeasureEnd)?.Invoke(this, null);
        }

        protected override void OnMeasure()
        {
            base.OnMeasure();
            EventParams.Event(XEventType.MeasureStart)?.Invoke(this, null);
            this.MeasureSize();
            if (Childs.Count == 0) return;

            if (LayoutParams.IsWrapWidth)
            {
                //Width = this.ParentWidth();
            }

            if (LayoutParams.IsWrapHeight)
            {
                //Height = this.ParentHeight();
            }

            this.MeasureMaxOrMin();

            MeasureChilds();
        }

        protected virtual void MeasureWrapSize()
        {
            if (LayoutParams.IsWrapWidth && LayoutParams.AspectRatio == 0)
            {
                Width = ChildRectWidth + LayoutParams.Padding.HorizontalSize;
            }

            if (LayoutParams.IsWrapHeight && LayoutParams.AspectRatio == 0)
            {
                Height = ChildRectHeight + LayoutParams.Padding.VerticalSize;
            }
            
            this.MeasureMaxOrMin();
        }

        protected virtual void MeasureChilds() { }

        protected virtual void UpdateScollerSize()
        {
            Scroller?.UpdateScollerSize(ContentRect, ChildSize);
        }

        public override void Layout()
        {
            base.Layout();
            UpdateDrawViews();
            Scroller?.Layout(this);
            //Style.IsClipContent = ChildRectHeight > Height || ChildRectWidth > Width;
        }

        public virtual void UpdateDrawViews()
        {
            drawViews = Childs.Where(n => n.LayoutParams.Visible == XVisibleType.Visible).OrderBy(n => n.LayoutParams.ZIndex).ToList();
        }

        public override void Draw()
        {
            base.Draw();
            Scroller?.Draw();
        }

        protected override void DrawContent()
        {
            if (drawViews?.Count > 0)
            {
                drawViews.ForEach(n => n.Draw());
            }
        }

        public override void Invalidate()
        {
            RenderImp.Invalidate(this);
        }

        public override void Dispose()
        {
            Childs.ForEach(n => n.Dispose());
            base.Dispose();
            //Childs.Clear();
        }
    }
}
