using SkiaSharp;
using System;
using XcyUI.animation;
using XcyUI.events;
using XcyUI.models;
using XcyUI.navigation;
using XcyUI.theme;
using XcyUI.utils;

namespace XcyUI.SkiaSharp
{
    /// <summary>
    /// SkiaSharp渲染后端
    /// </summary>
    public class SkiaRenderBackend : IRenderBackend
    {
        private SKSurface surface;
        private GRContext grContext;
        public SkiaDraw SkiaDraw { get; private set; }
        
        public SKColor BackgoundColor { get; set; }
        private XPage _page;
        XPage IRenderBackend.Page { get => _page; set => _page = value; }

        public bool _eanbleAnimate;
        public SkiaRenderBackend()
        {
            SkiaDraw = new SkiaDraw();
            RenderImp.SetDraw(SkiaDraw);
            BackgoundColor = XTheme.Color.Background.ToSKColor();
        }
        public void ResetSurface(int width, int height)
        {
            surface?.Dispose();
            CreateSurface(width, height);
            _page?.StartLayout(width, height);
        }
        public void CreateSurface(int width, int height, object paramsData)
        {
            if (grContext == null)
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
            }
            CreateSurface(width, height);
            DrawHelper.PreWarmSkia(surface);
        }

        private void CreateSurface(int width, int height)
        {
            var framebufferInfo = new GRGlFramebufferInfo(0, SKColorType.Rgba8888.ToGlSizedFormat());
            surface = SKSurface.Create(
                grContext,
                new GRBackendRenderTarget(
                    width, height, 0, 8,
                    framebufferInfo
                ),
                GRSurfaceOrigin.BottomLeft,
                SKColorType.Rgba8888
            );
        }

        public void SetBackgoundColor(XColor color)
        {
            BackgoundColor = color.ToSKColor();
        }
        public void Render()
        {
            if (surface == null)
            {
                return;
            }
            RenderImp.SetDraw(SkiaDraw);
            if (XAnimation.IsStart() && !_eanbleAnimate)
            {
                XAnimation.HandlerAnimationItems();
            }
            SkiaDraw.Surface = surface;
            SkiaDraw.Canvas = surface.Canvas;
            SkiaDraw.Canvas.Clear(BackgoundColor);
            _page?.Draw();
            surface.Flush();
            grContext.PurgeUnusedResources(500);
        }

        public void Dispose()
        {
            if (grContext == null) return;
            XEvent.Clear();
            _page?.RootView?.Dispose();
            _page = null;
            SkiaDraw.Canvas = null;
            surface?.Dispose();
            surface = null;
            grContext?.Dispose();
            grContext = null;
        }
    }
}
