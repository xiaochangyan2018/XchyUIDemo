using SkiaSharp;
using System;
using System.Collections.Generic;
using XcyUI.animation;
using XcyUI.events;
using XcyUI.models;
using XcyUI.navigation;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;

namespace XcyUI.SkiaSharp
{
    public class SkiaRenderBackend : IRenderBackend
    {
        private SKSurface surface;
        private GRContext grContext;
        public SkiaDraw SkiaDraw { get; private set; }
        public XNavigationPage Navigation { get; private set; }
        public SKColor BackgoundColor { get; set; }
        private bool isDrak = false;

        public SkiaRenderBackend()
        {
            SkiaDraw = new SkiaDraw();
            Navigation = new XNavigationPage();
            RenderImp.SetDraw(SkiaDraw);
            isDrak = XTheme.DarkModeState.Value;
            BackgoundColor = XThemeManager.Theme.Colors.Background.ToSKColor();
        }

        public void ResetSurface(int width, int height, object paramsData)
        {
            grContext?.Flush();
            grContext?.PurgeUnusedResources(100);
            surface?.Dispose();
            CreateSurface(width, height);
        }

        public void CreateSurface(int width, int height, object paramsData)
        {
            Delegate del = (Delegate)paramsData;
            GRGlGetProcedureAddressDelegate get = (GRGlGetProcedureAddressDelegate)Delegate.CreateDelegate(
                typeof(GRGlGetProcedureAddressDelegate),
                del.Target,
                del.Method
            );
            var gl = GRGlInterface.Create(get);
            grContext = GRContext.CreateGl(gl);

            if (grContext == null)
            {
                Console.WriteLine("OpenGL 上下文无效");
            }
            CreateSurface(width, height);
        }

        private void CreateSurface(int width, int height)
        {
            Navigation.WindowWidth = width;
            Navigation.WindowHeight = height;
            surface = SKSurface.Create(
                grContext,
                new GRBackendRenderTarget(
                    width, height, 0, 8,
                    new GRGlFramebufferInfo(0, SKColorType.Rgba8888.ToGlSizedFormat())
                ),
                GRSurfaceOrigin.BottomLeft,
                SKColorType.Rgba8888
            );
        }

        public void Render()
        {
            if (surface == null)
            {
                return;
            }
            RenderImp.SetDraw(SkiaDraw);
            if (XAnimation.IsStart())
            {
                XAnimation.HandlerAnimationItems();
            }
            SkiaDraw.Surface = surface;
            SkiaDraw.Canvas = surface.Canvas;
            if (isDrak != XTheme.DarkModeState.Value)
            {
                BackgoundColor = XThemeManager.Theme.Colors.Background.ToSKColor();
                isDrak = XTheme.DarkModeState.Value;
            }
            SkiaDraw.Canvas.Clear(BackgoundColor);
            Navigation?.Draw();
            SkiaDraw.Canvas.Flush();
            grContext.PurgeUnusedResources(1000);
        }

        public void DispatchEvent(XView view, XEventInfo info)
        {
            RenderImp.SetDraw(SkiaDraw);
            XEvent.Dispatch(view, info);
        }

        public void DispatchEvent(XEventInfo info)
        {
            RenderImp.SetDraw(SkiaDraw);
            Navigation?.DispatchEvent(info);
        }

        public void Focus(bool focus)
        {
            XEvent.FocusChanged(focus);
        }

        public void Layout(int width, int height)
        {
            Navigation?.Layout(width, height);
        }

        public void Open(XPage page)
        {
            Navigation?.Open(page);
        }

        public XAccessibilityNode GetAccessibilityTree()
        {
            return Navigation?.GetAccessibilityTree();
        }

        public List<XAccessibilityIssue> AuditAccessibility()
        {
            return Navigation?.AuditAccessibility() ?? new List<XAccessibilityIssue>();
        }

        public void Dispose()
        {
            if (grContext == null) return;
            XEvent.Clear();
            Navigation.Dispose();
            Navigation = null;
            SkiaDraw.Canvas = null;
            surface?.Dispose();
            grContext?.Dispose();
            surface = null;
            grContext = null;
        }
    }
}
