using Silk.NET.GLFW;
using System.Runtime.InteropServices;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.GLFW.accessibility;
using XcyUI.GLFW.curor;
using XcyUI.GLFW.manager;
using XcyUI.GLFW.windowStyle;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.views;

namespace XcyUI.GLFW.window
{
    public class XWindow : BaseWindow
    {
        private MouseButton currentMouseButton;
        private XPoint currentPoint;
        private const double doubleClickInterval = 300;
        private double lastClickTime;
        private MouseButton lastClickButton;
        private WindowsUiaBridge accessibilityProvider;

        protected unsafe override void OnWindowCreate()
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (isWindows)
            {
                XThemeManager.Theme.DefaultFontName = "Microsoft YaHei";
                curor = new WindowCursor()
                {
                    Window = window,
                    Glfw = glfw
                };
                windowStyle = new WindowStyleImp()
                {
                    Window = window,
                    Glfw = glfw
                };
                if (IsHideTitleBar)
                {
                    windowStyle.SetStyle();
                    glfw.GetWindowSize(window, out int windowWidth, out int windowHeight);
                    Width = Width * 2 - windowWidth;
                    Height = Height * 2 - windowHeight;
                    glfw.SetWindowSize(window, Width, Height);
                }
                var primaryMonitor = glfw.GetPrimaryMonitor();
                glfw.GetMonitorContentScale(primaryMonitor, out float xscale, out float yscale);
                glfw.GetMonitorWorkarea(primaryMonitor,
                out int x, out int y, out int width, out int height);
                glfw.SetWindowPos(window, (width - Width) / 2, (height - Height) / 2);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                //XThemeManager.Theme.DefaultFontName = "Microsoft YaHei";
                XThemeManager.Theme.DefaultFontName = "WenQuanYi Micro Hei";
                curor = new LinuxCursor()
                {
                    Window = window,
                    Glfw = glfw
                };
                windowStyle = new LinuxWindowStyleImp()
                {
                    Window = window,
                    Glfw = glfw
                };
                if (IsHideTitleBar)
                {
                    windowStyle.SetStyle();
                }
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                XThemeManager.Theme.DefaultFontName = "PingFang SC";
            }
        }

        protected override void OnWindowReady()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                InstallAccessibilityProvider();
            }
        }

        private unsafe void InstallAccessibilityProvider()
        {
            var hwnd = WindowStyleImp.GetWin32Window(window);
            if (hwnd == System.IntPtr.Zero)
            {
                return;
            }

            accessibilityProvider?.Dispose();
            accessibilityProvider = new WindowsUiaBridge(
                hwnd,
                () => RenderBackend?.GetAccessibilityTree(),
                action => ExecuteOnMainThread(action));
            AddCloseAction(() =>
            {
                accessibilityProvider?.Dispose();
                accessibilityProvider = null;
            });
        }

        public override void OnDestory()
        {
            accessibilityProvider?.Dispose();
            accessibilityProvider = null;
            base.OnDestory();
        }

        protected override unsafe void OnCursorEnterCallback(bool entered)
        {
            if (!entered)
            {
                var actionEvent = new XEventInfo();
                actionEvent.X = currentPoint.X;
                actionEvent.Y = currentPoint.Y;
                actionEvent.IsLeft = currentMouseButton == MouseButton.Left;
                actionEvent.EventType = XEventType.Leave;
                if (glfw.GetCurrentContext() != window)
                {
                    glfw.MakeContextCurrent(window);
                }
                XEvent.HoverView?.EventParams?.Event(XEventType.Leave)?.Invoke(XEvent.HoverView, actionEvent);
            }
        }

        protected unsafe override void OnCursorPosCallback(double x, double y)
        {
            currentPoint = new XPoint((int)x, (int)y);
            var actionEvent = new XEventInfo
            {
                X = currentPoint.X,
                Y = currentPoint.Y,
                IsLeft = currentMouseButton == MouseButton.Left,
                EventType = XEventType.Move
            };

            if (XEvent.TargetView == null)
            {
                actionEvent.EventType = XEventType.Hover;
            }

            if(!isMouseInWindow)
            {
                actionEvent.EventType = XEventType.Leave;
            }
            DispatchEvent(actionEvent);
        }

       

        protected override void OnMouseButtonCallback(MouseButton button, InputAction action, KeyModifiers mods)
        {
            currentMouseButton = button;
            var actionEvent = new XEventInfo
            {
                ClickKey = (int)mods,
                X = currentPoint.X,
                Y = currentPoint.Y,
                IsLeft = button == MouseButton.Left
            };
            var currentPos = currentPoint;
            switch(action)
            {
                case InputAction.Press:
                    actionEvent.EventType = XEventType.Down;
                    DispatchEvent(actionEvent);
                    break;
                
                case InputAction.Release:
                    actionEvent.EventType = XEventType.Click;
                    DispatchEvent(actionEvent);
                    double currentTime = glfw.GetTime() * 1000;
                    bool isDoubleClick = button == lastClickButton
                                      && (currentTime - lastClickTime) <= doubleClickInterval;
                    if (isDoubleClick)
                    {
                        actionEvent.EventType = XEventType.DoubleClick;
                        DispatchEvent(actionEvent);
                        lastClickTime = 0;
                        lastClickButton = MouseButton.Left;
                    }
                    else
                    {
                        lastClickTime = currentTime;
                        lastClickButton = button;
                    }
                    actionEvent.EventType = XEventType.Up;
                    DispatchEvent(actionEvent);
                    break;
            }
        }

        protected override void OnScrollCallback(double xOffset, double yOffset)
        {
            var dist = 50;
            var actionEvent = new XEventInfo
            {
                X = currentPoint.X,
                Y = currentPoint.Y,
                IsLeft = currentMouseButton == MouseButton.Left,
                WheelSize = ((int)(yOffset * dist)).AsPx()
            };
            if (actionEvent.WheelSize == 0)
            {
                actionEvent.WheelSize = ((int)(xOffset * dist)).AsPx();
            }
            actionEvent.IsVerticalWheel = yOffset != 0;
            actionEvent.EventType = XEventType.Wheel;
            DispatchEvent(actionEvent);
        }

        protected override void OnKeyCallback(Keys key, int scanCode, InputAction action, KeyModifiers mods)
        {
            if (action == InputAction.Press)
            {
                var actionEvent = new XEventInfo
                {
                    KeyValue = KeyValueManager.GetKeyValue(key),
                    EventType = XEventType.KeyPress,
                    KeyModify = (KeyModify)mods
                };
                DispatchEvent(actionEvent);
            }
        }

        protected override void OnCharCallback(char character)
        {
            if (XEvent.FocusView != null)
            {
                var actionEvent = new XEventInfo
                {
                    KeyChar = character,
                    EventType = XEventType.KeyPress
                };
                DispatchEvent(XEvent.FocusView, actionEvent);
                if (XEvent.FocusView is XInput)
                {
                    ((XInput)XEvent.FocusView).ChangeImmPosition();
                }
            }
        }

        public override void MoveWindow()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                windowStyle?.MoveWindow();
                XEvent.ClearTargetView();
            }
        }
    }
}
