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
        protected List<XView> _childs;
        public XScroller Scroller;
        public int Space;
        protected List<XView> _layoutChilds = new();
        protected List<XView> _drawViews = new();
        protected List<XView> _retryMeasureChilds = new();
        protected int ChildRectWidth;
        protected int ChildRectHeight;

        public List<XView> Childs => _childs ??= new();
        internal List<XView> DrawViews => _drawViews;
        public XSize ChildSize => new XSize(ChildRectWidth, ChildRectHeight);
        

        protected void FillVisibleViews()
        {
            _layoutChilds.Clear();
            if (_childs!= null)
            {
                foreach (var view in _childs)
                {
                    if (view != null && view.LayoutParams.Visible != XVisibleType.Gone)
                    {
                        _layoutChilds.Add(view);
                    }
                }
            }
        }

        public void AddView(XView view)
        {
            view.Parent = this;
            Childs.Add(view);
        }

        public void InsertView(int index,XView view)
        {
            if(index >=0 && index <= Childs.Count)
            {
                view.Parent = this;
                Childs.Insert(index, view);
            }
        }

        public void RemoveView(XView view)
        {
            view.Dispose();
            _childs?.Remove(view);
            UpdateDrawViews();
        }

        public override void Translation(int x, int y)
        {
            base.Translation(x, y);
            ScolledChilds(x, y);
        }

        public virtual void ScolledChilds(int x, int y)
        {
            foreach (var child in _layoutChilds)
            {
                child.Translation(x, y);
            }
        }

        internal virtual void ScolledEnd(bool isVertical)
        {

        }


        public virtual void OnScolled(bool isVertical, int scolledSize, bool isScollerBar = false)
        {
            if (scolledSize == 0) return;
            Scroller?.OnScolled(this, isVertical, scolledSize, isScollerBar);
        }

        public override void Measure()
        {
            FillVisibleViews();
            base.Measure();
        }

        protected override void OnMeasure()
        {
            if (_layoutChilds.Count == 0) return;
            ChildRectHeight = ChildRectWidth = 0;
            MeasureChilds();
            if (Scroller?.EnableScrolled == true)
            {
                UpdateScollerSize();
            }
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
        }

        public virtual void UpdateDrawViews()
        {
            if (_childs == null) return;
            _drawViews.Clear();
            foreach (var child in _childs)
            {
                if (child.LayoutParams.Visible == XVisibleType.Visible)
                {
                    var index = _drawViews.Count;
                    for (int i = 0; i < _drawViews.Count; i++)
                    {
                        if (_drawViews[i].LayoutParams.ZIndex > child.LayoutParams.ZIndex)
                        {
                            index = i;
                            break;
                        }
                    }
                    _drawViews.Insert(index, child);
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            Scroller?.Draw();
        }

        protected override void DrawContent()
        {
            _drawViews.ForEach(n => n.Draw());
        }

        public override void Dispose()
        {
            _childs?.ForEach(n => n.Dispose());
            base.Dispose();
        }
    }
}
