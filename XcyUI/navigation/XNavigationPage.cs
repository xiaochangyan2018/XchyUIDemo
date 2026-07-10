using System.Collections.Generic;
using System.Linq;
using XcyUI.events;
using XcyUI.models;
using XcyUI.views;
using XcyUI.utils;

namespace XcyUI.navigation
{
    public class XNavigationPage
    {
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        private Stack<XPage> _Pages;

        public XNavigationPage()
        {
            _Pages = new Stack<XPage>();
        }

        public Stack<XPage> CopyPages()
        {
            Stack<XPage> pages = null;
            if (_Pages != null && _Pages.Count > 0)
            {
                pages = new Stack<XPage>();
                for (int i = _Pages.Count - 1; i >= 0; i--)
                {
                    pages.Push(_Pages.ElementAt(i));
                }
            }
            return pages;
        }

        public Stack<XPage> ReversalPages()
        {
            Stack<XPage> pages = null;
            if (_Pages != null && _Pages.Count > 0)
            {
                pages = new Stack<XPage>();
                for (int i = 0; i < _Pages.Count; i++)
                {
                    pages.Push(_Pages.ElementAt(i));
                }
            }
            return pages;
        }

        public void Layout(int width,int height)
        {
            WindowWidth = width;
            WindowHeight = height;
            Stack<XPage> pages = ReversalPages();
            if (pages != null && pages.Count > 0)
            {
                foreach (XPage page in pages)
                {
                    page.StartLayout(width, height);
                }
            }
        }

        public void DispatchEvent(XEventInfo eventInfo)
        {
            Stack<XPage> pages = ReversalPages();
            if (pages != null && pages.Count > 0)
            {
                foreach (XPage page in pages)
                {
                    XEvent.Dispatch(page.RootView, eventInfo);
                    break;
                }
            }
        }

        public void UpdateWidgetParentView()
        {

        }

        public void Draw()
        {
            Stack<XPage> pages = ReversalPages();
            if (pages != null && pages.Count > 0)
            {
                foreach (XPage page in pages)
                {
                    page.Draw();
                }
            }
        }

        public XGroup RouteView
        {
            get
            {
                var pages = ReversalPages();
                return pages != null && pages.Count > 0 ? pages.Pop().RootView as XGroup : null;
            }
        }

        public XAccessibilityNode GetAccessibilityTree()
        {
            return RouteView == null ? null : XAccessibility.BuildTree(RouteView);
        }

        public List<XAccessibilityIssue> AuditAccessibility()
        {
            return RouteView == null ? new List<XAccessibilityIssue>() : XAccessibility.Audit(RouteView);
        }

        public void Open(XPage page)
        {
            page.StartLayout(WindowWidth, WindowHeight);
            _Pages.Push(page);
        }


        public void Back()
        {
            if (_Pages.Count > 0)
            {
                var page = _Pages.Pop();
                page.Close();
                RenderImp.Invalidate(null);
            }
        }

        public void Dispose()
        {
            Stack<XPage> pages = ReversalPages();
            if (pages != null && pages.Count > 0)
            {
                foreach (XPage page in pages)
                {
                    page.Close();
                }
            }
            _Pages.Clear();
        }
    }
}
