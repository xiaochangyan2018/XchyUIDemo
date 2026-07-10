using Silk.NET.GLFW;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using XcyUI.GLFW.windowStyle;
using XcyUI.models;
using XcyUI.navigation;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;
using static XcyUI.GLFW.windowStyle.WindowStyleNoTitle;
using static XcyUI.models.XFunctions;

namespace XcyUI.GLFW.window
{
    public class BaseWindow: IWindow
    {        
        public int Width { get; set; }
        public int Height { get; set; }
        public string Title { get; set; }
        protected bool isMouseInWindow = false;
        protected unsafe WindowHandle* window;
        protected ICuror curor;
        protected IWindowStyle windowStyle;
        public bool IsHideTitleBar { get; set; }
        protected double targetFrameTime = 1.0 / 60;
        protected bool isSwapInterval;
        protected Glfw glfw => WindowManager.Get().GetGlfw();
        protected bool isFocused;
        protected bool isDispose = false;
        public IRenderBackend RenderBackend { get; set; }
        internal XFunction LoadAction { get; set; }
        internal XFunction RunAction { get; set; }
        
        internal XFunction LoadIcon { get; set; }
        internal List<XFunction> closeFunctions = new List<XFunction>();
        public BaseWindow()
        {
            Width = 900;
            Height = 700;
            Title = "XchyUI";
            isFocused = true;
        }
        public void Run()
        {
            OnCreate();
        }


        public virtual unsafe void OnCreate()
        {
            OnWindowCreatePre();
            glfw.WindowHint(WindowHintBool.Visible, false);
            Width = (int)(Width * XThemeManager.Scale);
            Height = (int)(Height * XThemeManager.Scale);
            window = glfw.CreateWindow(Width, Height, Title, null, null);
            glfw.MakeContextCurrent(window);
            
            OnWindowCreate();
            SetInput(window);
            glfw.GetFramebufferSize(window, out int fbWidth, out int fbHeight);

            // 创建画布
            RenderImp.SetWindow(this);
            RenderBackend?.CreateSurface(fbWidth, fbHeight, (Func<string, IntPtr>)glfw.GetProcAddress);
            OnLoad();
            RunAction?.Invoke();
            Render();
            OnWindowReady();
            LoadIcon?.Invoke();
            LoadAction?.Invoke();
            glfw.ShowWindow(window);
            glfw.PollEvents();
        }

        protected virtual void OnWindowCreatePre()
        {
            //glfw.WindowHint(WindowHintInt.Samples, 4);
            glfw.WindowHint(WindowHintBool.DoubleBuffer, true);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                glfw.WindowHint(WindowHintBool.Decorated, !IsHideTitleBar);
            }
            glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGL);
            glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3); 
            glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
            glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            glfw.WindowHint(WindowHintBool.OpenGLForwardCompat, true); 
            glfw.WindowHint(WindowHintBool.Resizable, true);
            glfw.WindowHint(WindowHintBool.FocusOnShow, true);
            
        }
        protected virtual void OnWindowCreate()
        {
        }

        public unsafe void SetWindowSize(int width,int height)
        {
            if (width == Width && height == Height) return;
            glfw.SetWindowSize(window, width, height);
        }

        protected unsafe virtual void SetInput(WindowHandle* window)
        {
            DoSetInput(window);
        }
        private unsafe void DoSetInput(WindowHandle* window)
        {
            if (glfw == null) return;
            glfw.SetWindowCloseCallback(window, OnWindowClosing);
            glfw.SetWindowRefreshCallback(window, (_) =>
            {
                Render();
            });
            glfw.SetWindowMaximizeCallback(window, (_, maximized) =>
            {
                Render();
            });
            glfw.SetWindowSizeCallback(window, (_, width, height) =>
            {
                var isChangedSize = width != Width || height != Height;
                Width = width;
                Height = height;
                if (isChangedSize)
                {
                    RenderImp.SetWindow(this);
                    RenderBackend?.Layout(width, height);
                    OnSizeChanged(width, height);

                    if (glfw.GetCurrentContext() != window)
                    {
                        glfw.MakeContextCurrent(window);
                    }
                    RenderBackend?.ResetSurface(width, height, null);
                    RenderBackend?.Render();
                }
            });

            //键盘
            glfw.SetKeyCallback(window, (_, key, scancode, action, mods) =>
            {
                OnKeyCallback(key, scancode, action, mods);
            });
            // 字符输入
            glfw.SetCharCallback(window, (_, codepoint) =>
            {
                char character = (char)codepoint;
                OnCharCallback((char)codepoint);
            });
            glfw.SetCursorEnterCallback(window, (_, entered) =>
            {
                isMouseInWindow = entered;
                OnCursorEnterCallback(entered);
            });

            // 鼠标移动
            glfw.SetCursorPosCallback(window, (_, x, y) =>
            {
                OnCursorPosCallback(x, y);
            });

            // 鼠标按钮
            glfw.SetMouseButtonCallback(window, (_, button, action, mods) =>
            {
                OnMouseButtonCallback(button, action, mods);
            });

            // 滚轮
            glfw.SetScrollCallback(window, (_, xOffset, yOffset) =>
            {
                OnScrollCallback(xOffset, yOffset);
            });

            glfw.SetWindowFocusCallback(window, (_, focus) =>
            {
                this.isFocused = focus;
                RenderBackend?.Focus(focus);
                Render();
                if (focus)
                {
                    WindowManager.Get().TopWindow = (XWindow)this;
                }
            });
        }

        protected virtual void OnSizeChanged(int width, int height)
        {
        }

        protected virtual void OnKeyCallback(Keys key, int scanCode, InputAction action,
            KeyModifiers mods)
        {
        }

        protected virtual void OnCharCallback(char character)
        {
        }

        protected virtual void OnCursorEnterCallback(bool entered)
        {
        }

        protected virtual void OnCursorPosCallback(double x, double y)
        {
        }

        protected virtual void OnMouseButtonCallback(MouseButton button,InputAction action, KeyModifiers mods)
        {
        }

        protected virtual void OnScrollCallback(double xOffset, double yOffset)
        {
        }


        public unsafe virtual void OnLoad()
        {
            
        }

        protected virtual void OnWindowReady()
        {

        }

        protected virtual void InitPlatforms()
        {

        }

        public void Invalidate()
        {
            WindowManager.Get().Invalidate(false);
        }

        public void InvalidateAll()
        {
            WindowManager.Get().Invalidate(true);
        }

        public void ExecuteOnLopper(XFunction action)
        {
            WindowManager.Get().ExecuteOnLopper(action);
        }

        public void ExecuteOnMainThread(XFunction action)
        {
            WindowManager.Get().ExecuteOnMainThread(action);
        }


        public virtual void OnDestory()
        {
        }

        private unsafe void OnWindowClosing(WindowHandle* window)
        {
            OnDestory();
            CloseWindow();
        }        

        public unsafe void MinimizeWindow()
        {
            glfw.IconifyWindow(window);
        }

        public unsafe void SwapBuffer()
        {
            glfw.SwapBuffers(window);
        }

        public unsafe void ToggleMaximize()
        {
            var isMaximized = glfw.GetWindowAttrib(window, WindowAttributeGetter.Maximized);

            if (isMaximized)
            {
                glfw.RestoreWindow(window);
            }
            else
            {
                glfw.MaximizeWindow(window);
            }
            OnMaximiseChanged(!isMaximized);
            Render();
        }

        protected virtual void OnMaximiseChanged(bool isMax)
        {
        }

        public unsafe void CloseWindow()
        {
            RenderBackend?.Dispose();
            glfw.SetWindowShouldClose(window, true);
        }

        public virtual void MoveWindow()
        {

        }

        public void PushPage(XPage page)
        {
            RenderBackend?.Open(page);
        }

        protected void DispatchEvent(XView view, XEventInfo eventInfo)
        {
            RenderImp.SetWindow(this);
            RenderBackend?.DispatchEvent(view, eventInfo);
        }

        protected void DispatchEvent(XEventInfo eventInfo)
        {
            RenderImp.SetWindow(this);
            RenderBackend?.DispatchEvent(eventInfo);
        }
        Stopwatch stopwatch = new Stopwatch();
        public unsafe void Render()
        {
            if (!IsClosed())
            {
                stopwatch.Restart();
                if (glfw.GetCurrentContext() != window)
                {
                    glfw.MakeContextCurrent(window);
                }
                RenderImp.SetWindow(this);
                RenderBackend?.Render();
                SwapBuffer();
                stopwatch.Stop();
                Console.WriteLine("times:" + stopwatch.ElapsedMilliseconds);
            }
        }

        public unsafe bool IsClosed()
        {
            return glfw.WindowShouldClose(window);
        }

        public bool HasFoucs()
        {
            return isFocused;
        }

        public unsafe void FocusWindow()
        {
            glfw.FocusWindow(window);
        }

        public unsafe bool IsMaxWindow()
        {
            return glfw.GetWindowAttrib(window, WindowAttributeGetter.Maximized);
        }

        public void ChangedNightMode(WindowColorMode mode)
        {
            windowStyle?.ChangedNightMode(mode);
        }

        public void OpenPage(XPage page)
        {
            RenderBackend?.Open(page);
        }

        public unsafe void Destory()
        {
            if (isDispose) return;
            closeFunctions.ForEach(n => n.Invoke());
            closeFunctions.Clear();
            RenderImp.SetWindow(null);
            RenderBackend?.Dispose();
            glfw.DestroyWindow(window);
            isDispose = true;
        }

        public virtual void ChangedImmPosition(XPoint point)
        {
            curor?.ChangedImmPosition(point);
        }

        public virtual void SetCursor(XCursorType type)
        {
            curor?.SetCursor(type);
        }

        public unsafe void SetWindowIcon(XBitmap bitmap)
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

        public void AddCloseAction(XFunction action)
        {
            closeFunctions.Add(action);
        }
    }
}
