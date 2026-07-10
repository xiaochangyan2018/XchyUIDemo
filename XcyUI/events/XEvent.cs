using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
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
        public static XView FocusView { get; private set; }
        public static event Action<XView> AccessibilityFocusChanged;

        public static int X { get; private set; }
        public static int Y { get; private set; }
        public static void ClearTargetView()
        {
            TargetView = null;
        }
        public static void ClearFocusView()
        {
            var oldFocusView = FocusView;
            FocusView = null;
            if (oldFocusView != null)
            {
                AccessibilityFocusChanged?.Invoke(null);
            }
        }
        public static void Clear()
        {
            var oldFocusView = FocusView;
            TargetView = null;
            HoverView = null;
            FocusView = null;
            if (oldFocusView != null)
            {
                AccessibilityFocusChanged?.Invoke(null);
            }
        }
        private static LinkedList<XView> views = new LinkedList<XView>();

        private static void AddEventViews(LinkedList<XView> views, XView view, XEventInfo info)
        {
            var rect = view.RenderRect;
            rect.Scale(-1);
            if (view.LayoutParams.Visible == XVisibleType.Visible && rect.Contain(info.Point))
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
                    foreach (var item in viewGroup.DrawViews)
                    {
                        if (item != null) AddEventViews(views, item, info);
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
            DoDispatch(root, info);
        }

        private static void DoDispatch(XView root, XEventInfo info)
        {
            if (IsInputEvent(info.EventType) && HandleKeyboard(root, info))
            {
                return;
            }

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
                SetFocus(null, info);
            }
            // 查找可以响应事件的view
            views.Clear();
            AddEventViews(views, root, info);

            var list = new List<XView>();
            foreach (var item in PreHoverViews)
            {
                if (!item.RenderRect.Contain(info.Point))
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
                    if(firstHandlerView == null && HoverView != view && info.EventType == XEventType.Hover)
                    {
                        HandlerLeave(view, info);
                    }
                    HandleEvent(view, info);
                    if (firstHandlerView == null)
                    {
                        firstHandlerView = view;
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

        private static void HandlerLeave(XView firstHandlerView,XEventInfo info)
        {
            var lastView = views.Last?.Value;
            // 处理leave
            if (info.EventType == XEventType.Hover)
            {
                var rect = HoverView?.RenderRect ?? new XRect();
                rect.Scale(-1);
                if ((HoverView != null && !rect.Contain(info.Point)) || (firstHandlerView != null && HoverView != null && HoverView != firstHandlerView && HoverView.EventParams?.Event(XEventType.Hover)?.IsMust == false))
                {
                    var leaveEventInfo = info.Copy(XEventType.Leave);
                    DoEvent(HoverView, leaveEventInfo, true);
                }
                else if(HoverView!=null)
                {
                    PreHoverViews.Add(HoverView);
                }
                HoverView = firstHandlerView;
            }
        }

        private static bool IsInputEvent(XEventType type)
        {
            return type == XEventType.KeyDown || type == XEventType.KeyPress;
        }

        private static void HandleEvent(XView view, XEventInfo info)
        {
            DoEvent(view, info, true);
            switch (info.EventType)
            {
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
                    TargetView = view;
                    SetFocus(view, info);
                    break;
            }
            
        }

        public static bool SetFocus(XView view)
        {
            return SetFocus(view, null);
        }

        public static bool SetFocus(XView view, XEventInfo sourceInfo)
        {
            if (view == FocusView)
            {
                return true;
            }

            if (view != null && !CanReceiveFocus(view))
            {
                return false;
            }

            var oldFocusView = FocusView;
            if (oldFocusView != null)
            {
                var lossInfo = sourceInfo?.Copy(XEventType.LossFocused) ?? new XEventInfo() { EventType = XEventType.LossFocused };
                DoEvent(oldFocusView, lossInfo, true);
                oldFocusView.DrawCache.IsRefreshCache = true;
            }

            FocusView = view;
            if (FocusView != null)
            {
                var focusInfo = sourceInfo?.Copy(XEventType.Focused) ?? new XEventInfo() { EventType = XEventType.Focused };
                focusInfo.X = FocusView.RenderRect.Center.X;
                focusInfo.Y = FocusView.RenderRect.Center.Y;
                DoEvent(FocusView, focusInfo, true);
                FocusView.DrawCache.IsRefreshCache = true;
                RenderImp.Invalidate(FocusView);
            }

            if (oldFocusView != null)
            {
                RenderImp.Invalidate(oldFocusView);
            }

            AccessibilityFocusChanged?.Invoke(FocusView);

            return true;
        }

        public static bool MoveFocus(XView root, bool forward = true)
        {
            var focusableViews = XAccessibility.GetFocusableViews(root);
            if (focusableViews.Count == 0)
            {
                return false;
            }

            var currentIndex = FocusView == null ? -1 : focusableViews.IndexOf(FocusView);
            int nextIndex;
            if (currentIndex < 0)
            {
                nextIndex = forward ? 0 : focusableViews.Count - 1;
            }
            else
            {
                nextIndex = forward ? currentIndex + 1 : currentIndex - 1;
                if (nextIndex >= focusableViews.Count)
                {
                    nextIndex = 0;
                }
                else if (nextIndex < 0)
                {
                    nextIndex = focusableViews.Count - 1;
                }
            }

            return SetFocus(focusableViews[nextIndex]);
        }

        public static bool Activate(XView view)
        {
            return Activate(view, null);
        }

        public static bool Activate(XView view, XEventInfo sourceInfo)
        {
            if (!XAccessibility.IsKeyboardActivatable(view))
            {
                return false;
            }

            var info = sourceInfo?.Copy(XEventType.Click) ?? new XEventInfo() { EventType = XEventType.Click };
            info.X = view.RenderRect.Center.X;
            info.Y = view.RenderRect.Center.Y;
            info.IsLeft = true;
            DoEvent(view, info, true);
            RenderImp.Invalidate(view);
            return true;
        }

        private static bool HandleKeyboard(XView root, XEventInfo info)
        {
            if (info.KeyValue == XKeyValue.Tab)
            {
                var isShift = (info.KeyModify & KeyModify.Shift) == KeyModify.Shift;
                return MoveFocus(root, !isShift);
            }

            if ((info.KeyValue == XKeyValue.Enter || info.KeyValue == XKeyValue.Space)
                && Activate(FocusView, info))
            {
                return true;
            }

            return false;
        }

        private static bool CanReceiveFocus(XView view)
        {
            return view != null
                && view.EventParams.Focusable
                && XAccessibility.IsEnabled(view)
                && XAccessibility.IsVisibleToAccessibility(view);
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
                if (isFocus)
                {
                    AccessibilityFocusChanged?.Invoke(FocusView);
                }
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
            view.OnEvent(info);
            var mEvent = view.EventParams?.Event(info.EventType);
            if (mEvent != null)
            {
                XWidget.SetCurrentView(view);
                mEvent.Invoke(view, info);
            }
            if (isPop && (mEvent == null || !mEvent.IsIntercept))
            {
                PopEvent(view.Parent, info);
            }
        }
    }
}
