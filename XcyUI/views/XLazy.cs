using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using XcyUI.animation;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.templates;

namespace XcyUI.views
{
    public abstract class XLazy : XGroup
    {
        public List<XLazyTemplate> Templates { get; private set; }
        public List<XLazyItem> Items { get; private set; }

        public bool IsFixedItem { get; set; }
        internal XLazyItem FirstVisibleItem { get; private set; }
        protected bool isScolled;
        protected bool isScollForward;
        protected XRect childRect;
        internal bool IsAnimate { get; set; }
        internal List<XLazyItem> AnimateItems { get; private set; }
        internal XAnimate Animate { get; private set; }
        internal XLazyAnimateInfo AnimateInfo { get; set; }
        internal int LayoutNum { get; set; }

        // 快速滑动的动画
        protected XAnimate _smoothScolledAniate = XAnimation.AnimateFloatOf();

        public XLazy()
        {
            Items = new List<XLazyItem>();
            Templates = new List<XLazyTemplate>();
            Animate = XAnimation.AnimateFloatOf();
            AnimateInfo = new XLazyAnimateInfo();
            initSmoothScolledAniate();
        }

        protected virtual void MeasureItems()
        {
            LayoutNum = 0;
            if (IsFixedItem)
            {
                MeausreFixedItems();
            }
            else
            {
                MeasureDynamicItems();
            }
            Scroller?.UpdateScollerSize(ContentRect, ChildSize);
        }

        protected virtual void MeausreFixedItems()
        {
            Templates[0].MeasureFixed(this);
            ChildRectHeight = Templates[0].SumHeight;
            ChildRectWidth = Templates[0].Width;
        }

        public void Refresh()
        {
            for (int i = 0; i < Templates.Count; i++)
            {
                Templates[i].CacheSize.Clear();
                Templates[i].Clear();
                Templates[i].Height = 0;
                Templates[i].IsNotifyChanged = true;
            }
            StartLayout();
        }
        protected virtual void MeasureDynamicItems()
        {
            ChildRectHeight = 0;
            ChildRectWidth = 0;
            for (int i = 0; i < Templates.Count; i++)
            {
                Templates[i].CacheSize.Clear();
                Templates[i].MeasureDynamic(this);
                ChildRectHeight += Templates[i].SumHeight;
            }
            ChildRectHeight += Space * (Templates.Count - 1);
            ChildRectWidth = Templates.Max(n => n.SumWidth);
        }


        private void initSmoothScolledAniate()
        {
            _smoothScolledAniate.Duration = 20;
            _smoothScolledAniate.Interpolator = XAnimationInterpolator.Uniform;
            _smoothScolledAniate.OnCallback = (value, index) =>
            {
                OnScolledItems();
            };
        }

        protected virtual void OnScolledItems()
        {

        }

        protected virtual void LayoutItems()
        {
            AnimateItems = null;
            if (IsAnimate && Items.Count > 0)
            {
                AnimateItems = Items.ToList();
            }
            childRect = new XRect();
            if (IsFixedItem)
            {
                LayoutFixedItems();
            }
            else
            {
                LayoutDynamicItems();
            }
            var views = Items.Select(n => n.View);
            Childs.Clear();
            Childs.AddRange(views);
            UpdateDrawViews();
            FirstVisibleItem = Items.FirstOrDefault();            
        }
        
        protected abstract void LayoutFixedItems();
        protected abstract void LayoutDynamicItems();

        protected override void OnLayout()
        {
            var contentRect = ContentRect;
            var contentX = contentRect.X - childRect.X;
            var contentY = contentRect.Y - childRect.Y;
            childRect = contentRect;
            Childs.ForEach(n =>
            {
                n.X += contentX;
                n.Y += contentY;
                ((XGroup)n).Childs.ForEach(a =>
                {
                    a.Translation(contentX + (a.LayoutParams.Freeze ? 0 : Scroller.ScrollerWidth), contentY);
                });
            });
            Templates.ForEach(n => n.IsNotifyChanged = false);
            DoAnimate();
        }

        protected virtual void DoAnimate()
        {
            if (AnimateInfo.Enable && AnimateItems != null && Items.Count > 0)
            {
                Animate.Stop();
                // 先找到删除
                if (AnimateInfo.IsAdd)
                {
                    AddAnimate();
                }
                else
                {
                    DeleteAnimate();
                }
                var animateInfo = AnimateInfo;
                animateInfo.Enable = false;
                AnimateInfo = animateInfo;
            }
        }

        protected abstract void DeleteAnimate();
        protected abstract void AddAnimate();

        public override void Translation(int x, int y)
        {
            base.Translation(x, y);
            for (int i = 0; i < Childs.Count; i++)
            {
                Childs[i].DrawCache.IsRefreshCache = true;
            }
            Scroller?.VerticalScollerBar?.Translation(0, y);
            Scroller?.HorizontalScollerBar?.Translation(x, 0);
        }

        public abstract void ScrolledToIndex(int templeateIndex, int index, bool isSmooth);

        public int Count()
        {
            var count = 0;
            for (int i = 0; i < Templates.Count; i++)
            {
                count += Templates[i].Datas.Count;
            }
            return count;
        }
        public List<object> Datas
        {
            get
            {
                var list = new List<object>();
                Templates.ForEach(n => list.AddRange(n.Datas));
                return list;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            Templates.ForEach(n =>
            {
                n.Clear();
            });
        }
    }

    public struct XLazyAnimateInfo
    {
        internal bool Enable { get; set; }
        internal bool IsAdd { get; set; }
        internal int StartIndex { get; set; }
        internal int EndIndex { get; set; }
    }
}
