using XcyUI.Controls;
using XcyUI.GLFW;
using XcyUI.SkiaSharp;
using static XcyUI.Controls.Controls;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Desktop
{
    public static class Desktop
    {
        public static void Main(XWindowParams param)
        {
            var loadAction = param.Load;
            var composeAction = param.Compose;
            param.RenderBackend = new SkiaRenderBackend();
            param.Load = () =>
            {
                HotkeyManager.Start();
                SvgRes.Load();
                loadAction?.Invoke();
            };
            param.Compose = () =>
            {
                Box(composeAction);
                DialogView();
                ToastView();
            };
            XApplication.Run(param);
        }

        public static void OpenWindow(int id, XWindowParams param)
        {
            var loadAction = param.Load;
            var composeAction = param.Compose;
            param.RenderBackend = new SkiaRenderBackend();
            param.Compose = () =>
            {
                Box(() => composeAction?.Invoke());
                DialogView();
                ToastView();
            };
            XApplication.Window(id, param);
        }        
    }
}
