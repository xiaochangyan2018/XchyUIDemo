using Silk.NET.GLFW;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using XcyUI.models;
using XcyUI.theme;
using static XcyUI.GLFW.Platform.WindowsHideBar;
using static XcyUI.GLFW.XApplication;

namespace XcyUI.GLFW.Platform
{
    public static class WindowsPlatform
    {
        [DllImport("glfw3.dll", EntryPoint = "glfwGetWin32Window", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern IntPtr GetWin32Window(WindowHandle* window);
        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("imm32.dll")]
        internal static extern int ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("imm32.dll")]
        internal static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct POINTAPI
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct COMPOSITIONFORM
        {
            public uint dwStyle;
            public POINTAPI ptCurrentPos;
            public RECT rcArea;
        }
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private static XPoint _curorPoint;
        internal static Dictionary<IntPtr, WindowsHideBar> styles = new();

        public static unsafe void UpdateImmPosition(XPoint point)
        {
            if (CurrentWindow == null || _curorPoint.Equals(point)) return;
            var curor = GetWin32Window(CurrentWindow.Handle);
            _curorPoint = point;
            IntPtr hImc = ImmGetContext(curor);
            COMPOSITIONFORM cf = new COMPOSITIONFORM();
            cf.dwStyle = 2;
            cf.ptCurrentPos.x = point.X;
            cf.ptCurrentPos.y = point.Y;
            ImmSetCompositionWindow(hImc, ref cf);
            ImmReleaseContext(hImc, CurrentWindow.IntPtr);
        }

        public static unsafe void MoveWindow()
        {
            if (CurrentWindow == null) return;
            var hwnd = GetWin32Window(CurrentWindow.Handle);
            ReleaseCapture();
            PostMessage(hwnd, WM_NCLBUTTONDOWN, (IntPtr)HT_CAPTION, IntPtr.Zero);
        }

        public unsafe static void Remmove(WindowHandle* handle)
        {
            var key = (IntPtr)handle;
            styles.Remove(key);
        }

        /// <summary>
        /// 设置无标题模式
        /// </summary>
        /// <param name="handle"></param>
        public unsafe static void SetNoTitleStyle(WindowHandle* handle)
        {
            var key = (IntPtr)handle;
            if(!styles.TryGetValue(key,out WindowsHideBar style)){
                style = new WindowsHideBar();
                styles[key] = style;
            }
            var hwnd = GetWin32Window(handle);
            style.Apply(hwnd);
        }

        public enum WindowColorMode
        {
            Auto = 0,    // 跟随系统
            Light = 1,   // 浅色模式（白天）
            Dark = 2     // 深色模式（黑夜）
        }

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd,int dwAttribute,ref int pvAttribute,int cbAttribute);
        /// <summary>
        /// 设置黑夜白天模式
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="mode"></param>
        public static void SetWindowColorMode(IntPtr handle, WindowColorMode mode)
        {
            if (handle == IntPtr.Zero) return;
            int value = mode == WindowColorMode.Dark ? 1 : 0;
            DwmSetWindowAttribute(handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
        }
    }
}
