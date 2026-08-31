using Silk.NET.GLFW;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using XcyUI.animation;
using XcyUI.events;
using XcyUI.GLFW.Platform;
using XcyUI.models;
using XcyUI.navigation;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.widgets;

namespace XcyUI.GLFW
{
    public static class XApplication
    {
        internal static Dictionary<int, WindowInfo> Windows = new(); // 窗口集合
        private volatile static bool _enableDraw = true; // 是否绘制
        public static readonly Glfw glfw = Glfw.GetApi();
        internal static double currentFlushTime; // 当前绘制完的时间
        private static double startInvalidateTime; // 开始惠子时间
        internal static double targetFrameTime = 1.0 / 60; // 帧率
        public static bool isWaitEvents = false; // 是否处于等待状态
        private readonly static int _mainThreadId = Thread.CurrentThread.ManagedThreadId; // 当前线程ID
        private static bool IsMainThread() => Thread.CurrentThread.ManagedThreadId == _mainThreadId; // 判断是否主线程
        private static Action _lastRenderAction; // 设置额外的渲染函数


        // 排队进入主线程
        private static readonly Channel<Action> _Channel = Channel.CreateUnbounded<Action>(
           new UnboundedChannelOptions
           {
               SingleWriter = true,
               SingleReader = true
           }
        );      

        public unsafe static void Run(XWindowParams param)
        {
            glfw.Init();
            glfw.SwapInterval(param.IsSwapInterval ? 1 : 0);
            // 获取宽度比例
            var primaryMonitor = glfw.GetPrimaryMonitor();
            VideoMode* videoModel = glfw.GetVideoMode(primaryMonitor);
            XTheme.TargetWidth = videoModel->Width;
            XTheme.ScreenWidth = videoModel->Width;
            XTheme.ScreenHeight = videoModel->Height;
            // 获取设计宽度和实际屏幕宽度的比例
            XTheme.Scale = (float)XTheme.TargetWidth / XTheme.DesignWidth;
            // 设置窗口属性
            glfw.WindowHint(WindowHintBool.DoubleBuffer, true);
            glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGL);
            glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
            glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
            glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            glfw.WindowHint(WindowHintBool.OpenGLForwardCompat, true);

            // 创建主窗口
            var window = Window(0, param);
            targetFrameTime = param.Fps;
            
            // 渲染循环
            while (!glfw.WindowShouldClose(window.Handle))
            {
                glfw.PollEvents();
                // 读取排队任务
                if (_Channel.Reader.TryRead(out Action action))
                {
                    action?.Invoke();
                }
                // 渲染
                if ((_enableDraw || XAnimation.IsStart()))
                {
                    double currentTime = glfw.GetTime();
                    // 控制帧率
                    if (currentTime - currentFlushTime >= targetFrameTime)
                    {
                        Render();
                        currentFlushTime = currentTime;
                        if (currentTime - startInvalidateTime > 0.1)
                        {
                            _enableDraw = false;
                        }
                    }
                }
                else
                {
                    isWaitEvents = true;
                    glfw.WaitEvents();
                }
            }

            window.Render?.Dispose();
            glfw.Terminate();
        }

        /// <summary>
        /// 创建窗口
        /// </summary>
        /// <param name="id">窗口ID</param>
        /// <param name="param">窗口参数</param>
        /// <returns></returns>
        public unsafe static WindowInfo Window(int id, XWindowParams param)
        {
            if (Windows.ContainsKey(id))
            {
                glfw.FocusWindow(Windows[id].Handle);
                return null;
            }
            param.OnCreate?.Invoke();
            glfw.WindowHint(WindowHintBool.Floating, param.Floating);
            glfw.WindowHint(WindowHintBool.Visible, false);
            glfw.WindowHint(WindowHintBool.Decorated, param.Decorated);
            glfw.WindowHint(WindowHintBool.FocusOnShow, true);
            glfw.WindowHint(WindowHintBool.Focused, false);
            glfw.WindowHint(WindowHintBool.TransparentFramebuffer, param.IsTransparent);
            glfw.WindowHint(WindowHintBool.Resizable, param.Resize);

            var Width = (int)(param.Width * XTheme.Scale);
            var Height = (int)(param.Height * XTheme.Scale);
            var window = glfw.CreateWindow(Width, Height, param.Title, null, null);
            glfw.SetWindowSizeLimits(window, (int)(param.MinWidth * XTheme.Scale), (int)(param.MinHeight * XTheme.Scale), (int)(param.MaxWidth * XTheme.Scale), (int)(param.MaxHeight * XTheme.Scale));
            glfw.MakeContextCurrent(window);
            XEvent.Clear();
            foreach (var item in Windows)
            {
                item.Value.Focused = false;
                item.Value.IsTopWindow = false;
            }
            var windowInfo = new WindowInfo(window, param.RenderBackend, param);
            Windows[id] = windowInfo;
            // 设置无标题样式
            if (param.HideTitleBar)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    WindowsPlatform.SetNoTitleStyle(window);
                    glfw.GetWindowSize(window, out int windowWidth, out int windowHeight);
                    Width = Width * 2 - windowWidth-1;
                    Height = Height * 2 - windowHeight-1;
                    glfw.SetWindowSize(window, Width, Height);
                }
            }

            // 居中显示
            if (param.X == int.MinValue || param.Y == int.MinValue)
            {
                var primaryMonitor = glfw.GetPrimaryMonitor();
                glfw.GetMonitorContentScale(primaryMonitor, out float xscale, out float yscale);
                glfw.GetMonitorWorkarea(primaryMonitor, out int x, out int y, out int width, out int height);
                glfw.SetWindowPos(window, (width - Width) / 2, (height - Height) / 2);
            }
            else
            {
                glfw.SetWindowPos(window, param.X, param.Y);
            }

            glfw.GetFramebufferSize(window, out int fbWidth, out int fbHeight);
            // 创建画布
            param.RenderBackend?.CreateSurface(fbWidth, fbHeight, (Func<string, IntPtr>)glfw.GetProcAddress);
            windowInfo.Window = new XWindowImpl();
            RenderImp.SetWindow(windowInfo.Window);
            _enableDraw = false;
            if (!string.IsNullOrEmpty(param.Logo))
            {
                var bitmap = RenderImp.GetBitmap(param.Logo, true);
                SetWindowIcon(window, bitmap);
            }
            // UI布局
            if (windowInfo.Render != null)
            {
                windowInfo.Render.Page = new XPage()
                {
                    RootView = XCompose.ContentView(param.Compose).View
                };
                windowInfo.Render.Page.StartLayout(fbWidth, fbHeight);
                windowInfo.Render.Page.Focus(true);
            }
            param.Load?.Invoke();

            // 窗口事件
            XWindowEvent.SetWindowEvent(windowInfo, param.Dispose);
            glfw.ShowWindow(window);
            return windowInfo;
        }

        private static Stopwatch stopwatch = new Stopwatch();
        /// <summary>
        /// 渲染
        /// </summary>
        internal unsafe static void Render()
        {
            stopwatch.Restart();
            // 控制高频的绘制进入帧区间
            if (_lastRenderAction != null)
            {
                _lastRenderAction?.Invoke();
                _lastRenderAction = null;
            }

            for (int i = 0; i < Windows.Keys.Count; i++)
            {
                var key = Windows.Keys.ElementAt(i);
                var window = Windows[key];
                // 如果窗口具有焦点或者是TOP窗口或者为刷新状态
                if (window.Focused || window.IsTopWindow || window.Refresh)
                {
                    if(glfw.GetCurrentContext() != window.Handle)
                    {
                        glfw.MakeContextCurrent(window.Handle);
                    }
                    RenderImp.SetWindow(window.Window);
                    window.Render?.Render();
                    glfw.SwapBuffers(window.Handle);
                    window.Refresh = false;
                }
            }
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 16)
            {
                Console.WriteLine("flush times:" + stopwatch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 重置开始失效时间
        /// </summary>
        private static void ResetInvalidateTime()
        {
            startInvalidateTime = glfw.GetTime();
        }

        /// <summary>
        /// 失效函数
        /// </summary>
        public static void Invalidate()
        {
            _enableDraw = true;
            Post(ResetInvalidateTime);
            if (isWaitEvents)
            {
                isWaitEvents = false;
                glfw.PostEmptyEvent();
            }
        }

        /// <summary>
        /// 如果在主线程就直接执行否则入队
        /// </summary>
        /// <param name="action"></param>
        public static void Post(Action action)
        {
            if (IsMainThread())
            {
                action?.Invoke();
            }
            else
            {
                _Channel.Writer.TryWrite(action);
            }
            if (isWaitEvents)
            {
                isWaitEvents = false;
                glfw.PostEmptyEvent();
            }
        }

        /// <summary>
        /// 入队执行
        /// </summary>
        /// <param name="action"></param>
        public static void PostToQueue(Action action)
        {
            _Channel.Writer.TryWrite(action);
            if (isWaitEvents)
            {
                isWaitEvents = false;
                glfw.PostEmptyEvent();
            }
        }

        /// <summary>
        /// 把函数直接提交到渲染循环执行
        /// </summary>
        /// <param name="action"></param>
        public static void PostToRender(Action action)
        {
            _lastRenderAction = action;
            if (isWaitEvents)
            {
                isWaitEvents = false;
                glfw.PostEmptyEvent();
            }
        }
        /// <summary>
        /// 获取当前窗口
        /// </summary>
        public static WindowInfo CurrentWindow => Windows.FirstOrDefault(n => n.Value.IsTopWindow || n.Value.Focused).Value;

        /// <summary>
        /// 设置窗口图标
        /// </summary>
        /// <param name="window">窗口对象</param>
        /// <param name="bitmap">图标对象</param>
        public unsafe static void SetWindowIcon(WindowHandle* window, XBitmap bitmap)
        {
            byte* ptr = (byte*)Marshal.AllocHGlobal(bitmap.Buffers.Length);
            Marshal.Copy(bitmap.Buffers, 0, (IntPtr)ptr, bitmap.Buffers.Length);
            try
            {
                Image img = new Image
                {
                    Width = bitmap.Width,
                    Height = bitmap.Height,
                    Pixels = ptr
                };
                glfw.SetWindowIcon(window, 1, &img);
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr)ptr);
            }
        }

        /// <summary>
        /// 最小化窗口
        /// </summary>
        public unsafe static void MinimizeWindow()
        {
            glfw.IconifyWindow(CurrentWindow.Handle);
        }

        /// <summary>
        /// 判断是否最大化
        /// </summary>
        /// <returns></returns>
        public unsafe static bool IsMaximized()
        {
            var window = CurrentWindow.Handle;
            var isMaximized = glfw.GetWindowAttrib(window, WindowAttributeGetter.Maximized);
            return isMaximized;
        }

        /// <summary>
        /// 切换最大化
        /// </summary>
        public unsafe static void ToggleMaximize()
        {
            if (IsMaximized())
            {
                glfw.RestoreWindow(CurrentWindow.Handle);
            }
            else
            {
                glfw.MaximizeWindow(CurrentWindow.Handle);
                
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public unsafe static void CloseWindow()
        {
            glfw.SetWindowShouldClose(CurrentWindow.Handle, true);
            XWindowEvent.CloseWindow(CurrentWindow);
        }

        /// <summary>
        /// 移动窗口位置
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        public unsafe static void MoveWindow(int x, int y)
        {
            glfw.SetWindowPos(CurrentWindow.Handle, x, y);
        }

        /// <summary>
        /// 移动窗口
        /// </summary>
        public unsafe static void MoveWindow()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsPlatform.MoveWindow();
                XEvent.ClearTargetView();
            }
            else
            {
                // 通用移动窗口
                glfw.GetWindowPos(CurrentWindow.Handle, out int x, out int y);
                MoveWindow(x + (XEvent.X - XEvent.DownPoint.X), y + XEvent.Y - XEvent.DownPoint.Y);
            }
        }
    }
}
