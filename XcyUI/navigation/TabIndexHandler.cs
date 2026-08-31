using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.views;
using XcyUI.widgets;

namespace XcyUI.navigation
{
    public static class TabIndexHandler
    {
        private static XView currentTabindexView = null;
        private static int currentSelectTabindex = 0;
        private static XView currentLayerView = null;
        private static XBrush preBrush = new XBrush();
        private static XShadow preShadow = new XShadow();
        private static XShadow tabShadow = new XShadow(0, 0, XTheme.Color.Primary, 5);
        public static void Handler(XGroup rootView, XEventInfo eventInfo)
        {
            if (eventInfo.EventType == XEventType.KeyPress)
            {
                if (eventInfo.KeyValue == XKeyValue.Tab)
                {
                    HandlerTab(rootView, eventInfo);
                }
                else if (eventInfo.KeyValue == XKeyValue.Enter)
                {
                    HandlerEnter(rootView, eventInfo);
                }
            }
        }

        public static void Clear()
        {
            if (currentTabindexView != null && !(currentTabindexView is XInput))
            {
                new XModify(currentTabindexView)
                .Background(preBrush)
                .Shadow(preShadow);
                currentTabindexView = null;
            }
        }

        private static void HandlerTab(XGroup rootView, XEventInfo eventInfo)
        {
            rootView.EventParams.EventOrCreate(XEventType.Dispose).AddFunction("tabindex_handler", (v, info) =>
            {
                currentTabindexView = null;
                currentLayerView = null;
                currentSelectTabindex = -1;
            });
            var lastPopCard = rootView.Childs.LastOrDefault(n => n.Key.StartsWith(XCompose.POPCARD_PRE_KEY));
            var view = lastPopCard ?? rootView;
            if (view != null)
            {
                if (view != currentLayerView)
                {
                    currentTabindexView = null;
                    currentSelectTabindex = -1;
                }
                currentLayerView = view;
                var list = new List<XView>();
                view.ModifyChild(n =>
                {
                    if (n.Tabindex >= 0)
                    {
                        list.Add(n);
                    }
                });
                var sortedList = list.OrderBy(x => x.Tabindex == 0 ? 1 : 0)
                           .ThenBy(x => x.Tabindex)
                           .ToList();

                currentSelectTabindex += 1;
                if (currentSelectTabindex >= sortedList.Count)
                {
                    currentSelectTabindex = 0;
                }
                for (int i = currentSelectTabindex; i < sortedList.Count; i++)
                {
                    var tabindexView = sortedList[i];
                    if (tabindexView.IsFocus())
                    {
                        continue;
                    }
                    currentSelectTabindex = i;
                    if (XEvent.FocusView != null)
                    {
                        XEvent.FocusChanged(false);
                    }
                    XEvent.FocusView = tabindexView;
                    XEvent.FocusChanged(true);
                    if (tabindexView is XInput)
                    {
                        ((XInput)tabindexView).Focus(true);
                    }
                    SetTabindexViewStyle(tabindexView);
                    currentTabindexView = tabindexView;
                    break;
                }
            }
        }

        private static void HandlerEnter(XGroup rootView, XEventInfo eventInfo)
        {
            if (currentTabindexView != null)
            {
                currentTabindexView.EventParams.EventOrCreate(XEventType.Click)?.Invoke(currentTabindexView, new XEventInfo() { EventType = XEventType.Click, X = currentTabindexView.X + 1, Y = currentTabindexView.Y + 1 });
            }
        }


        private static void SetTabindexViewStyle(XView view)
        {
            if (currentTabindexView != null && !(currentTabindexView is XInput))
            {
                new XModify(currentTabindexView)
                    .Background(preBrush)
                    .Shadow(preShadow);
                currentTabindexView = null;
            }
            if (view != null && !(view is XInput))
            {
                preBrush = view.Style.Background;
                preShadow = view.Style.Shadow;
                new XModify(view)
                    .Background(GetBackgound(view))
                    .Shadow(tabShadow);
                view.Invalidate();
                currentTabindexView = view;
            }
        }

        private static XBrush GetBackgound(XView view)
        {
            if (view == null)
            {
                return new XBrush(XTheme.Color.Background);
            }
            if (!view.Style.Background.IsEmpty)
            {
                return view.Style.Background;
            }
            return GetBackgound(view.Parent);
        }



        public static bool IsSelectTabindex(this XView view)
        {
            return currentTabindexView == view;
        }
    }
}
