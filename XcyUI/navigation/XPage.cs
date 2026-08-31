using System;
using XcyUI.events;
using XcyUI.models;
using XcyUI.views;

namespace XcyUI.navigation
{
    public class XPage
    {        
        public int Width { get; set; }
        public int Height { get; set; }
        public XView RootView { get; set; }
        public void StartLayout(int width, int height)
        {
            if (RootView == null)
            {
                OnViewCreated();
            }
            Width = width;
            Height = height;
            RootView.LayoutParams.Width = width;
            RootView.LayoutParams.Height = height;
            RootView.Style.IsClip = false;
            RootView.StartLayout();
        }

        public void DispatchEvent(XView view, XEventInfo info)
        {
            XEvent.Dispatch(view, info);
        }
        public void DispatchEvent(XEventInfo info)
        {
            XEvent.Dispatch(RootView, info);
        }

        public void Focus(bool focus)
        {
            XEvent.FocusChanged(focus);
        }

        public virtual void OnViewCreated() { }

        public void Draw()
        {
            RootView.Draw();
        }

        public void Close()
        {
            OnDispose();
        }

        protected void OnDispose()
        {
            RootView.Dispose();
            RootView = null;
        }
    }
}
