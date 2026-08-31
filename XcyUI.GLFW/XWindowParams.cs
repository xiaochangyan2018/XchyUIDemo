using Silk.NET.GLFW;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using XcyUI.GLFW.Platform;
using XcyUI.models;
using XcyUI.theme;

namespace XcyUI.GLFW
{
    public class XWindowParams
    {
        public bool IsSwapInterval = true; // 是否开启垂直同步（V-Sync）
        public int X = int.MinValue; // 窗口位置X
        public int Y = int.MinValue; // 窗口位置Y
        public int Width = 1380; // 窗口宽度
        public int Height = 800; // 窗口高度
        public int MinWidth = -1; // 窗口最小宽度
        public int MinHeight = -1; // 窗口最小高度
        public int MaxWidth = -1; // 窗口最大宽度
        public int MaxHeight = -1; // 窗口最大高度
        public double Fps = 1 / 60.0; // 窗口刷新帧率默认60帧
        public bool HideTitleBar = false; // 是否隐藏窗口标题栏
        public bool Decorated = true; // 是否保持标准窗口
        public bool Resize = true; // 是否可以改变大小
        public bool IsTransparent; // 设置窗口可以透明
        public bool Floating; // 设置窗口可以悬浮
        public bool Modal; // 设置模态窗口
        public string Title = "XcyUI"; // 标题
        public string Logo = ""; // 窗口图标 base64格式
        public Action OnCreate; // 开始创建窗口回调函数
        public Action Load; // 创建完窗口初始化画布以及布局后的回调函数
        public Action Dispose; // 窗口销毁函数
        public Action Compose; // 组合函数
        public IRenderBackend RenderBackend; // 渲染后端
    }

    public class WindowInfo
    {
        public unsafe WindowHandle* Handle; // 窗口句柄
        public IRenderBackend Render; // 渲染后端
        public IWindow Window; // 窗口需要实现的接口
        public bool IsTopWindow = true; // 是否最前面的窗口
        public bool Focused = true; // 是否有焦点
        public bool Refresh; // 是否刷新
        public XWindowParams Param; // 窗口构建参数
        public unsafe IntPtr IntPtr => (IntPtr)Handle;

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj != null && IntPtr == ((WindowInfo)obj).IntPtr;
        }

        public override int GetHashCode()
        {
            return IntPtr.GetHashCode();
        }

        public unsafe WindowInfo(WindowHandle* window, IRenderBackend render,XWindowParams windowParams)
        {
            Handle = window;
            Render = render;
            Param = windowParams;
            render.SetBackgoundColor(Param.IsTransparent ? XColors.Transparent : XTheme.Color.Background);
            XTheme.DarkModeState.Add(ChangeDarkMode);
            SetDarkMode();
        }

        public void Dispose()
        {
            foreach (var item in ((XWindowImpl)Window).DisposeActions)
            {
                item.Invoke();
            }
            XTheme.DarkModeState.Remove(ChangeDarkMode);
            Render?.Dispose();
            Param?.Dispose?.Invoke();
            Render = null;
            Param = null;
        }

        public unsafe void ChangeDarkMode(bool isDark)
        {
            Render.SetBackgoundColor(Param.IsTransparent ? XColors.Transparent : XTheme.Color.Background);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var hwnd = WindowsPlatform.GetWin32Window(Handle);
                WindowsPlatform.SetWindowColorMode(hwnd, isDark ? WindowsPlatform.WindowColorMode.Dark : WindowsPlatform.WindowColorMode.Light);
            }
            Refresh = true;
        }

        public unsafe void SetDarkMode()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var hwnd = WindowsPlatform.GetWin32Window(Handle);
                WindowsPlatform.SetWindowColorMode(hwnd, XTheme.DarkModeState.Value ? WindowsPlatform.WindowColorMode.Dark : WindowsPlatform.WindowColorMode.Light);
            }
        }
    }
}
