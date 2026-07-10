using System;
using System.Collections.Generic;
using System.Threading;
using XcyUI.animation;
using XcyUI.events;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;
using static XcyUI.models.XFunctions;

namespace XcyUI.expansions
{
    public static class ViewExtensions
    {
        private static CancellationTokenSource invalidateTask = null;
        public static int AsPx(this int value)
        {
            return XThemeManager.Scale == 1? value: (int)(value * XThemeManager.Scale + 0.5f);
        }

        public static int AsDp(this int value)
        {
            return XThemeManager.Scale == 1 ? value : (int)(value / XThemeManager.Scale);
        }

        public static float AsDp(this float value)
        {
            return XThemeManager.Scale == 1 ? value : (int)(value / XThemeManager.Scale);
        }

        public static float AsPx(this float value)
        {
            return XThemeManager.Scale == 1 ? value : value * XThemeManager.Scale;
        }
        public static XView RootView(this XView view)
        {
            return view.Parent != null ? RootView(view.Parent) : view;
        }

        public static XView TopWrapParent(this XView view)
        {
            var parent1 = view.Parent;
            var parent2 = parent1?.Parent;
            var isWrap1 = parent1?.LayoutParams?.IsWrapWidth == true || parent1?.LayoutParams?.IsWrapHeight == true;

            return !isWrap1?view: isWrap1 && ((parent2?.LayoutParams?.IsWrapWidth ?? false) == false && (parent2?.LayoutParams?.IsWrapHeight ?? false) == false) ? parent1 : TopWrapParent(parent1);
        }

        public static int ParentWidth(this XView view)
        {
            var parentContentWidth = view.Parent?.ContentRect.Width ?? 0;
            return parentContentWidth;
        }

        public static int ParentHeight(this XView view)
        {
            var parentContentHeight = view.Parent?.ContentRect.Height ?? 0;
            return parentContentHeight;
        }

        public static void MeasureSize(this XView view)
        {
            var param = view.LayoutParams;
            view.Width = param.Width == XLayoutParams.Fill ? view.ParentWidth() -param.Margin.HorizontalSize : param.Width;
            view.Width = Math.Max(0, view.Width);
            view.Height = param.Height == XLayoutParams.Fill ? view.ParentHeight() - param.Margin.VerticalSize : param.Height;
            view.Height = Math.Max(0, view.Height);
            MeasureAspectRatio(view);
            MeasureMaxOrMin(view);
        }

        public static void MeasureAspectRatio(this XView view)
        {
            var param = view.LayoutParams;
            if (param.AspectRatio != 0)
            {
                if (view.Width > 0 && view.Height <= 0)
                {
                    view.Height = (int)(view.Width * param.AspectRatio);
                }
                else if (view.Width <= 0 && view.Height > 0)
                {
                    view.Width = (int)(view.Height * param.AspectRatio);
                }
            }
        }

        public static void MeasureMaxOrMin(this XView view)
        {
            var param = view.LayoutParams;
            if (param.MinWidth > 0) view.Width = Math.Max(param.MinWidth, view.Width);
            if (param.MaxWidth > 0) view.Width = Math.Min(param.MaxWidth, view.Width);
            if (param.MinHeight > 0) view.Height = Math.Max(param.MinHeight, view.Height);
            if (param.MaxHeight > 0) view.Height = Math.Min(param.MaxHeight, view.Height);
        }

        public static int ChildCount(this XView view)
        {
            return view is XGroup ? ((XGroup)view).Childs.Count : 0;
        }

        public static XView ChildElemnt(this XView view,int index)
        {
            return view is XGroup ? ((XGroup)view).Childs[index] : null;
        }

        public static List<XView> ChildElemnts(this XView view, int start,int count)
        {
            return view is XGroup ? ((XGroup)view).Childs.GetRange(start, count) : null;
        }

        public static void AddView(this XView view, XView childView)
        {
            if (view is XGroup)
            {
                ((XGroup)view).AddView(childView);
            }
        }

        public static void InsertView(this XView view, int index, XView childView)
        {
            if (view is XGroup)
            {
                ((XGroup)view).InsertView(index, childView);
            }
        }

        public static void RemoveView(this XView view, XView childView)
        {
            if (view is XGroup)
            {
                ((XGroup)view).RemoveView(childView);
            }
        }

        public static void Removed(this XView view)
        {
            if (view.Parent is XGroup)
            {
                ((XGroup)view.Parent).RemoveView(view);
                ((XGroup)view.Parent).Invalidate();
            }
        }

        public static void Visible(this XView view, XVisibleType visible)
        {
            view.LayoutParams.Visible = visible;
        }
        public static void Visible(this XView view, bool visible)
        {
            view.LayoutParams.Visible = visible ? XVisibleType.Visible : XVisibleType.Gone;
        }

        public static bool IsFocus(this XView view)
        {
            return view == XEvent.FocusView;
        }
        

        public static void InVisible(this XView view, bool visible)
        {
            view.LayoutParams.Visible = visible ? XVisibleType.Visible : XVisibleType.InVisible;
        }

        public static XFunction<XView,XEventInfo> RemoveEvent(this XView view, XEventType eventType, string key)
        {
            return view.EventParams.RemoveFunction(eventType, key);
        }

        public static void AddEvent(this XView view, XEventType eventType, string key, XFunction function)
        {
            view.EventParams.EventOrCreate(eventType).AddFunction(key, (v, info) =>
            {
                function.Invoke();
            });
        }

        public static void AddEvent(this XView view, XEventType eventType, XFunction<XView, XEventInfo> function)
        {
            view.EventParams.EventOrCreate(eventType).AddFunction(eventType.ToString(), function);
        }

        public static void AddEvent(this XView view, XEventType eventType, string key, XFunction<XView, XEventInfo> function)
        {
            view.EventParams.EventOrCreate(eventType).AddFunction(key, function);
        }

        public static void AddEvent(this XView view, XEventType eventType, string key, XFunction<XEventInfo> function)
        {
            view.EventParams.EventOrCreate(eventType).AddFunction(key, (v, info) =>
            {
                function.Invoke(info);
            });
        }

        public static void DebounceInvalidate(this XView view)
        {
            if (XAnimation.IsStart()) return;
            if(invalidateTask != null)
            {
                invalidateTask.Cancel();
                invalidateTask.Dispose();
            }
            invalidateTask = XTask.RunDelayed(() =>
            {
                RenderImp.Post(() =>
                {
                    RenderImp.Invalidate(view);
                });
            }, 200);
        }

        internal static bool HasWeightChild(this XView view)
        {
            for (int i = 0; i < view.ChildCount(); i++)
            {
                if (view is XColumn && view.ChildElemnt(i).LayoutParams.Weight > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static void BubbleUpLayout(this XView view)
        {
            if (view == null) return;
            var width = view.Width;
            var height = view.Height;
            view.StartLayout();
            view.IsMeasured = false;
            if ((width != view.Width || height != view.Height) && width > 0 && height > 0 && !(view.Parent?.Parent is XLazy))
            {
                BubbleUpLayout(view.Parent);
            }
            view.IsMeasured = true;
        }

        public static void NotifyLazy(this XView view)
        {
            if(view == null)
            {
                return;
            }
            if (view is XLazy)
            {
                (view as XLazy).Refresh();
                return;
            }
            NotifyLazy(view.Parent);
        }

        internal static void RefreshParentCache(this XView view)
        {
            view.DrawCache.IsRefreshCache = true;
            while (view.Parent != null)
            {
                view.Parent.DrawCache.IsRefreshCache = true;
                view = view.Parent;
            }
        }

        internal static bool IsParentCache(this XView view)
        {
            if (view == null)
            {
                return false;
            }
            if(view.Parent?.IsCache == true)
            {
                return true;
            }
            else
            {
                return IsParentCache(view.Parent);
            }
        }

        internal static void ResetParams(this XView view)
        {
            view.LayoutParams.Reset();
            view.Style.Reset();
            view.Accessibility.Reset();
            view.EventParams.Event(XEventType.Dispose)?.Invoke(view, null);
            view.EventParams.Clear();
            if(view is XGroup)
            {
                var group = (XGroup)view;
                foreach (var item in group.Childs)
                {
                    item.ResetParams();
                }
            }
        }

        public static bool IsScrolledToBottom(this XGroup group)
        {
            if (group != null)
            {
                var maxScollerHeight = group.ChildSize.Height - group.ContentRect.Height;
                return maxScollerHeight == -group.Scroller.ScrollerHeight;
            }
            return false;
        }
        public static bool IsScrolledToRight(this XGroup group)
        {
            if (group != null)
            {
                var maxScollerWidth = group.ChildSize.Width - group.ContentRect.Width;
                return maxScollerWidth< 0 ||( maxScollerWidth == -group.Scroller.ScrollerWidth && maxScollerWidth != 0);
            }
            return false;
        }

        public static void ModifyChild(this XView view, XFunction<XView> function)
        {
            function?.Invoke(view);
            for (int i = 0; i < view.ChildCount(); i++)
            {
                function?.Invoke(view.ChildElemnt(i));
                ModifyChild(view.ChildElemnt(i), function);
            }
        }
    }
}
