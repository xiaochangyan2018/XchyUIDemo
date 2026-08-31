using Silk.NET.GLFW;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using XcyUI.theme;

namespace XcyUI.GLFW.Platform
{
    internal static class Win32
    {
        public const int GWLP_WNDPROC = -4;
        public const int GWL_STYLE = -16;
        public const uint WM_NCCALCSIZE = 0x0083;
        public const uint WM_NCHITTEST = 0x0084;
        public const int WS_THICKFRAME = 0x00040000;
        public const int WS_CAPTION = 0x00C00000;
        public const int WS_MAXIMIZE = 0x01000000;
        public const int HTCAPTION = 2;

        public const int SM_CXFRAME = 32;   // 也可用 SM_CXSIZEFRAME
        public const int SM_CYFRAME = 33;
        public const int SM_CXPADDEDBORDER = 92;
        public const int HTLEFT = 10;
        public const int HTRIGHT = 11;
        public const int HTTOP = 12;
        public const int HTTOPLEFT = 13;
        public const int HTTOPRIGHT = 14;
        public const int HTBOTTOM = 15;
        public const int HTBOTTOMLEFT = 16;
        public const int HTBOTTOMRIGHT = 17;

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS { public int Left, Right, Top, Bottom; }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            // 辅助方法：向内收缩
            public void Inflate(int dx, int dy)
            {
                Left += dx;
                Right -= dx;
                Top += dy;
                Bottom -= dy;
            }
        }

        // 定义正确的委托类型
        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        // 使用 GetWindowLong/SetWindowLong（.NET 5+ 推荐）
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
        public static extern nint GetWindowLongPtrW(nint hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        public static extern nint SetWindowLongPtrW(nint hWnd, int nIndex, nint dwNewLong);

        [DllImport("user32.dll")]
        public static extern nint CallWindowProc(nint p, nint hWnd, uint msg, nint wParam, nint lParam);

        [DllImport("user32.dll")]
        public static extern short GetSystemMetrics(int nIndex);

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(nint hWnd, ref MARGINS p);
    }

    public class WindowsHideBar
    {

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(nint hWnd, ref Win32.RECT lpRect);
        private static Win32.MARGINS _margins;
        private nint _oldProc = nint.Zero;
        private nint _hwnd = nint.Zero;

        // 保存委托引用防止被 GC 回收
        private Win32.WndProc _wndProcDelegate;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }

        [DllImport("user32.dll")]
        private static extern int MapWindowPoints(nint hWndFrom, nint hWndTo, ref POINT lpPoint, uint cPoints);

        public void Apply(nint hwnd)
        {
            _hwnd = hwnd;
            _wndProcDelegate = WndProc;
            nint newProcPtr = Marshal.GetFunctionPointerForDelegate(_wndProcDelegate);
            _oldProc = Win32.SetWindowLongPtrW(_hwnd, Win32.GWLP_WNDPROC, newProcPtr);
            _margins = new Win32.MARGINS { Left = 0, Right = 0, Top = 1, Bottom = 0 };
            Win32.DwmExtendFrameIntoClientArea(_hwnd, ref _margins);
        }
        
        private nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam)
        {
            switch (msg)
            {
                case Win32.WM_NCCALCSIZE:
                    if (wParam != nint.Zero)
                    {
                        if (IsMaximized(hWnd))
                        {
                            // 正确读取 RECT 结构
                            var r = Marshal.PtrToStructure<Win32.RECT>(lParam);
                            r.Inflate(8, 8);
                            Marshal.StructureToPtr(r, lParam, false);
                        }
                        return nint.Zero;
                    }
                    break;

                case Win32.WM_NCHITTEST:
                    // 如果需要自定义命中测试，在这里实现
                     return HitTestNCA(hWnd, lParam);
            }
            return Win32.CallWindowProc(_oldProc, hWnd, msg, wParam, lParam);
        }

        
        private nint HitTestNCA(nint hWnd, nint lParam)
        {
            int l = lParam.ToInt32();
            int x = (short)(l & 0xFFFF);
            int y = (short)((l >> 16) & 0xFFFF);

            //// 1) 最大化按钮命中：返回 HTMAXBUTTON(=9)，系统据此在悬停时弹出「贴靠布局」。
            ////    必须放在边缘检测之前，否则顶部/右上角带会先把按钮区判成 HTTOP/HTTOPRIGHT。
            //var pt = new POINT { X = x, Y = y };
            //MapWindowPoints(nint.Zero, hWnd, ref pt, 1);   // 屏幕 -> 客户
            //if (pt.X >= 2087 && pt.X < 2087+89 &&
            //    pt.Y >= 0 && pt.Y < 69)
            //{
            //    Console.WriteLine("kkkkkkkkkkkkkkkkkkkkkkk");
            //     return (nint)9;  // 若 Win32 无此常量，直接用 (nint)9
            //}

            // 取窗口矩形
            var r = new Win32.RECT();
            GetWindowRect(hWnd, ref r);
            // 真实可缩放边框 = 窗口框 + padded border(92)；padded border 对 X/Y 是同一个值
            int pad = Win32.GetSystemMetrics(92);
            int bx = Win32.GetSystemMetrics(32) + pad;  // SM_CXFRAME
            int by = Win32.GetSystemMetrics(33) + pad;  // SM_CYFRAME

            bool left = x >= r.Left && x < r.Left + bx;
            bool right = x < r.Right && x >= r.Right - bx;
            bool top = y >= r.Top && y < r.Top + by;
            bool bottom = y < r.Bottom && y >= r.Bottom - by;

            // 先判断四角，再判断四边
            if (left && top) return (nint)Win32.HTTOPLEFT;
            if (right && top) return (nint)Win32.HTTOPRIGHT;
            if (left && bottom) return (nint)Win32.HTBOTTOMLEFT;
            if (right && bottom) return (nint)Win32.HTBOTTOMRIGHT;
            if (left) return (nint)Win32.HTLEFT;
            if (right) return (nint)Win32.HTRIGHT;
            if (top) return (nint)Win32.HTTOP;
            if (bottom) return (nint)Win32.HTBOTTOM;
            // 非边缘区域交给原 WndProc（会返回 HTCLIENT 等）
            return Win32.CallWindowProc(_oldProc, hWnd, Win32.WM_NCHITTEST, nint.Zero, lParam);
        }
        private bool IsMaximized(nint hWnd)
            => (Win32.GetWindowLongPtrW(hWnd, Win32.GWL_STYLE).ToInt64() & Win32.WS_MAXIMIZE) != 0;        
    }
}
