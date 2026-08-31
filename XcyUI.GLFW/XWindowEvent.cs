using Silk.NET.GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.GLFW.Platform;
using XcyUI.models;
using XcyUI.utils;
using XcyUI.views;
using static XcyUI.GLFW.XApplication;

namespace XcyUI.GLFW
{
    /// <summary>
    /// 对接Xcy核心窗口相关接口
    /// </summary>
    internal class XWindowImpl : IWindow
    {
        internal List<Action> DisposeActions { get; set; } = new();
        public void AddCloseAction(Action action)
        {
            if (!DisposeActions.Contains(action))
            {
                DisposeActions.Remove(action);
            }
        }
        public void UpdateImmPosition(XPoint point)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsPlatform.UpdateImmPosition(point);
            }
        }

        public void Post(Action action)
        {
            XApplication.Post(() => action?.Invoke());
        }

        public void PostToQueue(Action action)
        {
            XApplication.PostToQueue(() => action?.Invoke());
        }

        public void PostToRender(Action action)
        {
            XApplication.PostToRender(action);
        }


        public void Invalidate()
        {
            XApplication.Invalidate();
        }

        public void MoveWindow()
        {
            XApplication.MoveWindow();
        }


        public unsafe void SetCursor(XCursorType type)
        {
            if (CurrentWindow == null) return;
            var cursor = glfw.CreateStandardCursor(CursorShape.Arrow);
            switch (type)
            {
                case XCursorType.Arrow:
                    cursor = glfw.CreateStandardCursor(CursorShape.Arrow);
                    break;
                case XCursorType.Input:
                    cursor = glfw.CreateStandardCursor(CursorShape.IBeam);
                    break;
                case XCursorType.Crosshair:
                    cursor = glfw.CreateStandardCursor(CursorShape.Crosshair);
                    break;
                case XCursorType.Hand:
                    cursor = glfw.CreateStandardCursor(CursorShape.Hand);
                    break;
                case XCursorType.HResize:
                    cursor = glfw.CreateStandardCursor(CursorShape.HResize);
                    break;
                case XCursorType.VResize:
                    cursor = glfw.CreateStandardCursor(CursorShape.VResize);
                    break;
                case XCursorType.NwseResize:
                    cursor = glfw.CreateStandardCursor(CursorShape.NwseResize);
                    break;
                case XCursorType.NeswResize:
                    cursor = glfw.CreateStandardCursor(CursorShape.NeswResize);
                    break;
                case XCursorType.AllResize:
                    cursor = glfw.CreateStandardCursor(CursorShape.AllResize);
                    break;
                case XCursorType.NotAllowed:
                    cursor = glfw.CreateStandardCursor(CursorShape.NotAllowed);
                    break;
            }
            glfw.SetCursor(CurrentWindow.Handle, cursor);
        }
    }
    /// <summary>
    /// 对接窗口事件
    /// </summary>
    public static class XWindowEvent
    {
        private static MouseButton _currentMouseButton;
        private static XPoint _currentPoint;
        private const double doubleClickInterval = 300;
        private static double _lastClickTime;
        private static MouseButton _lastClickButton;

        /// <summary>
        /// 设置窗口事件
        /// </summary>
        /// <param name="windowData"></param>
        /// <param name="dispose"></param>
        public unsafe static void SetWindowEvent(WindowInfo windowData, Action dispose)
        {
            var window = windowData.Handle;
            // 窗口刷新
            glfw.SetWindowRefreshCallback(window, (_) =>
            {
                RenderImp.SetWindow(windowData.Window);
                Render();
            });
            // 窗口最大化最小化
            glfw.SetWindowMaximizeCallback(window, (_, maximized) =>
            {
                RenderImp.SetWindow(windowData.Window);
                Invalidate();
            });
            // 窗口大小改变
            glfw.SetWindowSizeCallback(window, (_, width, height) =>
            {
                RenderImp.SetWindow(windowData.Window);
                var page = windowData.Render?.Page;
                if (page == null) return;
                var oldWidth = page.RootView.Width;
                var oldHeight = page.RootView.Height;
                var isChangedSize = width != oldWidth || height != oldHeight;
                if (isChangedSize)
                {
                    if (glfw.GetCurrentContext() != window)
                    {
                        glfw.MakeContextCurrent(window);
                    }
                    // 更新画布大小
                    windowData.Render?.ResetSurface(width, height);
                }
            });
            // 窗口焦点变化
            glfw.SetWindowFocusCallback(window, (_, focus) =>
            {
                if (CurrentWindow?.Param?.Modal == true)
                {
                    glfw.FocusWindow(CurrentWindow.Handle);
                    return;
                }               
                RenderImp.SetWindow(windowData.Window);
                windowData.Focused = focus;
                windowData.Render?.Page?.Focus(focus);
                Invalidate();
                // 更新topWindow属性
                if (focus)
                {
                    foreach (var item in Windows)
                    {
                        item.Value.IsTopWindow = false;
                    }
                    windowData.IsTopWindow = true;
                }
            });

            //键盘
            glfw.SetKeyCallback(window, (_, key, scancode, action, mods) =>
            {
                RenderImp.SetWindow(windowData.Window);
                var page = windowData.Render?.Page;
                if (page == null) return;
                var actionEvent = new XEventInfo
                {
                    KeyValue = KeyValueManager.GetKeyValue(key),
                    EventType = XEventType.KeyPress,
                    KeyModify = (KeyModify)mods
                };
                if (XEvent.FocusView != null && action == InputAction.Press)
                {
                    XEvent.Dispatch(XEvent.FocusView, actionEvent);
                }
                if (XEvent.EnableTabindex && action == InputAction.Press)
                {
                    XEvent.Dispatch(page.RootView, actionEvent);
                }
            });

            // 字符输入
            glfw.SetCharCallback(window, (_, codepoint) =>
            {
                RenderImp.SetWindow(windowData.Window);
                char character = (char)codepoint;
                if (XEvent.FocusView != null)
                {
                    var actionEvent = new XEventInfo
                    {
                        KeyChar = character,
                        EventType = XEventType.KeyPress
                    };
                    XEvent.Dispatch(XEvent.FocusView, actionEvent);
                    if (XEvent.FocusView is XInput)
                    {
                        ((XInput)XEvent.FocusView).ChangeImmPosition();
                    }
                }
            });
            // 鼠标是否在窗口内监听
            glfw.SetCursorEnterCallback(window, (_, entered) =>
            {
                RenderImp.SetWindow(windowData.Window);               
                if (!entered)
                {
                    var actionEvent = new XEventInfo
                    {
                        X = _currentPoint.X,
                        Y = _currentPoint.Y,
                        IsLeft = _currentMouseButton == MouseButton.Left,
                        EventType = XEventType.Leave
                    };
                    if (glfw.GetCurrentContext() != window)
                    {
                        glfw.MakeContextCurrent(window);
                    }
                    XEvent.HoverView?.InvokeEvent(XEventType.Leave);
                }
            });

            // 鼠标移动
            glfw.SetCursorPosCallback(window, (_, x, y) =>
            {
                RenderImp.SetWindow(windowData.Window);
                var page = windowData.Render?.Page;
                if (page == null) return;
                if (CurrentWindow?.Param?.Modal != true)
                {
                    foreach (var item in Windows)
                    {
                        item.Value.IsTopWindow = false;
                    }
                    windowData.IsTopWindow = true;
                }
                _currentPoint = new XPoint((int)x, (int)y);
                var actionEvent = new XEventInfo
                {
                    X = _currentPoint.X,
                    Y = _currentPoint.Y,
                    IsLeft = _currentMouseButton == MouseButton.Left,
                    EventType = XEventType.Move
                };

                if (XEvent.TargetView == null)
                {
                    actionEvent.EventType = XEventType.Hover;
                }
                XEvent.Dispatch(page.RootView, actionEvent);
            });

            // 鼠标按键
            glfw.SetMouseButtonCallback(window, (_, button, action, mods) =>
            {
                if (CurrentWindow?.Param?.Modal == true || CurrentWindow != windowData)
                {
                    glfw.FocusWindow(CurrentWindow.Handle);
                    return;
                }
                RenderImp.SetWindow(windowData.Window);
                var page = windowData.Render?.Page;
                if (page == null) return;
                _currentMouseButton = button;
                var actionEvent = new XEventInfo
                {
                    ClickKey = (int)mods,
                    X = _currentPoint.X,
                    Y = _currentPoint.Y,
                    IsLeft = button == MouseButton.Left
                };
                var currentPos = _currentPoint;
                switch (action)
                {
                    case InputAction.Press:
                        actionEvent.EventType = XEventType.Down;
                        XEvent.Dispatch(page.RootView, actionEvent);
                        break;

                    case InputAction.Release:
                        actionEvent.EventType = XEventType.Click;
                        XEvent.Dispatch(page.RootView, actionEvent);
                        double currentTime = glfw.GetTime() * 1000;
                        bool isDoubleClick = button == _lastClickButton
                                          && (currentTime - _lastClickTime) <= doubleClickInterval;
                        if (isDoubleClick)
                        {
                            actionEvent.EventType = XEventType.DoubleClick;
                            XEvent.Dispatch(page.RootView, actionEvent);
                            _lastClickTime = 0;
                            _lastClickButton = MouseButton.Left;
                        }
                        else
                        {
                            _lastClickTime = currentTime;
                            _lastClickButton = button;
                        }
                        actionEvent.EventType = XEventType.Up;
                        XEvent.Dispatch(page.RootView, actionEvent);
                        break;
                }                
            });

            // 滚轮
            glfw.SetScrollCallback(window, (_, xOffset, yOffset) =>
            {
                RenderImp.SetWindow(windowData.Window);
                var page = windowData.Render?.Page;
                if (page == null) return;
                var dist = 50;
                var actionEvent = new XEventInfo
                {
                    X = _currentPoint.X,
                    Y = _currentPoint.Y,
                    IsLeft = _currentMouseButton == MouseButton.Left,
                    WheelSize = ((int)(yOffset * dist)).AsPx()
                };
                if (actionEvent.WheelSize == 0)
                {
                    actionEvent.WheelSize = ((int)(xOffset * dist)).AsPx();
                }
                actionEvent.IsVerticalWheel = yOffset != 0;
                actionEvent.EventType = XEventType.Wheel;
                XEvent.Dispatch(page.RootView, actionEvent);
            });

            // 窗口关闭
            glfw.SetWindowCloseCallback(window, _ =>
            {
                CloseWindow(windowData);
            });
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="windowData">窗口对象</param>
        public unsafe static void CloseWindow(WindowInfo windowData)
        {
            windowData.Dispose(); 
            RenderImp.SetWindow(null);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsPlatform.Remmove(windowData.Handle);
            }
            for (int i = 0; i < Windows.Keys.Count; i++)
            {
                int key = Windows.Keys.ElementAt(i);
                if (Windows[key].Handle == windowData.Handle)
                {
                    Windows.Remove(key);
                }
            }
            
            glfw.DestroyWindow(windowData.Handle);
        }
    }
}
