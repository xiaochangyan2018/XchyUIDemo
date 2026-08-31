using XcyDemo;
using XcyDemo.Images;
using XcyUI.GLFW;
using static XcyUI.Desktop.Desktop;

Main(new XWindowParams()
{
    Title = "XcyUI GUI软件示例",
    HideTitleBar = true,
    MinWidth = 750,
    MinHeight = 800,
    Logo = ImgRes.LogoBase64,
    Load = ImgRes.Load,
    Compose = MainPage.View
});

