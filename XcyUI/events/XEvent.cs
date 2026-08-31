using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.navigation;
using XcyUI.utils;
using XcyUI.views;
using XcyUI.widgets;

namespace XcyUI.events
{
    public class XEvent
    {
        // 正在操作的view
        public static XView TargetView { get; private set; }
        // 鼠标悬浮的view
        public static XView HoverView { get; private set; }
        internal static List<XView> PreHoverViews = new List<XView>();
        //获取焦点的view
        public static XView FocusView { get; internal set; }

        public static int X { get; private set; }
        public static int Y { get; private set; }
        public static XPoint DownPoint;
        public static XPoint Point => new XPoint(X, Y);
        public static bool EnableTabindex { get; set; } = true;
        public static void ClearTargetView()
        {
            TargetView = null;
        }
        /// <summary>
        /// 清除焦点view
        /// </summary>
        public static void ClearFocusView()
        {
            FocusView = null;
        }
        /// <summary>
        /// 清除目标view，悬浮view，焦点view
        /// </summary>
        public static void Clear()
        {
            TargetView = null;
            HoverView = null;
            FocusView = null;
        }
        private static LinkedList<XView> views = new LinkedList<XView>();

        /// <summary>
        /// 添加鼠标所在位置的有监听相关事件的view
        /// </summary>
        /// <param name="views">当前view的集合</param>
        /// <param name="view">当前遍历的view</param>
        /// <param name="info">事件信息</param>
        private static void AddEventViews(LinkedList<XView> views, XView view, XEventInfo info)
        {
            var rect = view.RenderRect;
            rect.Scale(-1);
            if (view.LayoutParams.Visible == XVisibleType.Visible && rect.Contain(info.Point) && view.EventParams.Enable)
            {
                view.EventParams?.Event(XEventType.DispatchEvent)?.Invoke(view, info);
                views.AddLast(view);
                if (view.EventParams?.Event(info.EventType)?.IsIntercept == true)
                {
                    return;
                }
                if (view is XGroup && view.ChildCount() > 0)
                {
                    var viewGroup = (XGroup)view;
                    for (int i = 0; i < viewGroup.DrawViews.Count; i++)
                    {
                        AddEventViews(views, viewGroup.DrawViews[i], info);
                    }
                    if (viewGroup.Scroller != null)
                    {
                        AddEventViews(views, viewGroup.Scroller.VerticalScollerBar, info);
                        AddEventViews(views, viewGroup.Scroller.HorizontalScollerBar, info);
                    }
                }
            }
        }

        /// <summary>
        /// 分发事件
        /// </summary>
        /// <param name="root">根view</param>
        /// <param name="info">事件信息</param>
        public static void Dispatch(XView root, XEventInfo info)
        {
            X = info.X;
            Y = info.Y;
            if (info.EventType == XEventType.KeyPress && root.Parent == null)
            {
                TabIndexHandler.Handler(root as XGroup, info);
            }
            else
            {
                if (info.EventType == XEventType.Down)
                {
                    TabIndexHandler.Clear();
                }
                DoDispatch(root, info);
            }
        }

        private static void DoDispatch(XView root, XEventInfo info)
        {
            if (TargetView != null)
            {
                HandleEvent(TargetView, info);
                return;
            }
            else if (IsInputEvent(info.EventType))
            {
                DoEvent(FocusView, info, false);
                return;
            }
            if (info.EventType == XEventType.Down && FocusView!=null && !FocusView.RenderRect.Contain(info.Point))
            {
                DoEvent(FocusView, info.Copy(XEventType.LossFocused), true);
                RenderImp.Invalidate(FocusView);
                FocusView = null;
            }
            // 查找可以响应事件的view
            views.Clear();
            AddEventViews(views, root, info);
            var list = new List<XView>();
            foreach (var item in PreHoverViews)
            {
                var rect = item.RenderRect;
                var isMusit = item.EventParams.Event(XEventType.Hover)?.IsMust ?? false;
                if (item != HoverView && !(isMusit && rect.Contain(info.Point)))
                {
                    var leaveEventInfo = info.Copy(XEventType.Leave);
                    DoEvent(item, leaveEventInfo, true);
                    list.Add(item);
                }
            }
            list.ForEach(n => PreHoverViews.Remove(n));

            var node = views.Last;
            XView firstHandlerView = null;
            while (node != null)
            {
                var view = node.Value;
                var eventFunction = view.EventParams?.Event(info.EventType);
                if (view.EventParams?.Enable == true && eventFunction != null && firstHandlerView == null)
                {
                    HandleEvent(view, info);
                    if (firstHandlerView == null)
                    {
                        firstHandlerView = view;
                        break;
                    }
                }
                node = node.Previous;
            }
            if (HoverView != null && !HoverView.RenderRect.Contain(info.Point))
            {
                var leaveEventInfo = info.Copy(XEventType.Leave);
                DoEvent(HoverView, leaveEventInfo, true);
                HoverView = null;
            }
        }

        private static bool IsInputEvent(XEventType type)
        {
            return type == XEventType.KeyDown || type == XEventType.KeyPress;
        }

        private static void HandleEvent(XView view, XEventInfo info)
        {
            if (info.EventType == XEventType.Hover)
            {
                if (PreHoverViews.IndexOf(view) < 0)
                {
                    PreHoverViews.Add(view);
                }
            }
            DoEvent(view, info, true);
            switch (info.EventType)
            {
                case XEventType.Hover:
                    HoverView = view;
                    break;
                case XEventType.Up:

                    if (HoverView != null && !HoverView.RenderRect.Contain(info.Point))
                    {
                        var leaveEventInfo = info.Copy(XEventType.Leave);
                        DoEvent(HoverView, leaveEventInfo, true);
                    }
                    if (!view.RenderRect.Contain(info.Point))
                    {
                        DoEvent(view, info.Copy(XEventType.Cancel), false);
                    }
                    TargetView = null;
                    break;
                case XEventType.Down:
                    DownPoint = info.Point;
                    TargetView = view;
                    if (view != FocusView && view.EventParams.Focusable)
                    {
                        if (FocusView != null)
                        {
                            DoEvent(FocusView, info.Copy(XEventType.LossFocused), true);
                            FocusView.DrawCache.IsRefreshCache = true;
                        }
                        DoEvent(view, info.Copy(XEventType.Focused), true);
                        FocusView = view;
                    }
                    break;
            }
            
        }

        private static void PopEvent(XView view, XEventInfo info)
        {
            while (view != null)
            {
                if (view.EventParams?.Event(info.EventType)?.IsMust == true && (info.EventType!= XEventType.Leave || (info.EventType == XEventType.Leave && !view.RenderRect.Contain(info.Point))))
                {
                    DoEvent(view, info, true);
                }
                view = view.Parent;
            }
        }

        internal static void PopPreviousWheelEvent(XView view, XEventInfo info)
        {
            while (view != null)
            {
                if (view.EventParams.Contains(XEventType.Wheel))
                {
                    DoEvent(view, info, true);
                }
                view = view.Parent;
            }
        }

        public static void FocusChanged(bool isFocus)
        {
            if (FocusView != null)
            {
                var info = new XEventInfo();
                info.EventType = isFocus ? XEventType.Focused : XEventType.LossFocused;
                DoEvent(FocusView, info, true);
                FocusView.DrawCache.IsRefreshCache = true;
            }
        }

        private static void DoEvent(XView view, XEventInfo info, bool isPop)
        {
            if (view == null) return;
            var isClickable = info.EventType == XEventType.Click || info.EventType == XEventType.DoubleClick;
            if (isClickable && !view.RenderRect.Contain(info.Point))
            {
                return;
            }
            var mEvent = view.EventParams?.Event(info.EventType);
            if (mEvent != null)
            {
                XCompose.SetCurrentView(view);
                mEvent.Invoke(view, info);
            }
            view.OnEvent(info);
            if (isPop && (mEvent == null || !mEvent.IsIntercept))
            {
                PopEvent(view.Parent, info);
            }
        }
    }
}
