using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using XcyUI.utils;
using XcyUI.widgets;

namespace XcyUI.Desktop
{
    public class HotkeyManager
    {
        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int vKey);
        [DllImport("user32.dll")] private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private const int VK_CONTROL = 0x11;
        private const int VK_S = 0x53;
        private const int KEY_PRESSED = 0x8000;

        private static bool _triggerPending;

        public static void Start()
        {
            if (!Debugger.IsAttached) return;

            var thread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(15);

                    if (!IsVisualStudioForeground()) continue;

                    bool ctrlDown = (GetAsyncKeyState(VK_CONTROL) & KEY_PRESSED) != 0;
                    bool sDown = (GetAsyncKeyState(VK_S) & KEY_PRESSED) != 0;

                    // 1. 检测到按下 Ctrl+S
                    if (ctrlDown && sDown)
                    {
                        _triggerPending = true;
                    }

                    // 2. 🔥 关键：必须等按键全部松开，再执行 Alt+F10
                    if (_triggerPending && !ctrlDown && !sDown)
                    {
                        _triggerPending = false;
                        Thread.Sleep(30);
                        PressAltF10();
                        Thread.Sleep(80); 
                        RenderImp.PostToQueue(() =>
                        {
                            XCompose.HotReload.Send(true);
                            RenderImp.GetWindow().Invalidate();
                        });
                    }
                }
            })
            { IsBackground = true };
            thread.Start();
        }

        private static void PressAltF10()
        {
            keybd_event(0x12, 0, 0, 0);
            keybd_event(0x79, 0, 0, 0);
            Thread.Sleep(20);
            keybd_event(0x79, 0, 2, 0);
            keybd_event(0x12, 0, 2, 0);
        }

        private static bool IsVisualStudioForeground()
        {
            try
            {
                GetWindowThreadProcessId(GetForegroundWindow(), out uint pid);
                return Process.GetProcessById((int)pid).ProcessName.Contains("devenv");
            }
            catch { return false; }
        }
    }
}
