# XCY UI函数式组合跨平台UI引擎软件
这是一款基于 C# + SkiaSharp 构建的跨平台声明式 UI 框架，深度借鉴 Jetpack Compose 现代化设计理念，彻底摒弃 WPF、Avalonia 传统的 XAML + MVVM 臃肿架构，全新采用简洁高效的函数组合式 UI 开发模型。让 .NET 也能像移动端、前端一样拥有现代化的极速开发体验。

## 项目介绍
- 项目采用插拔式构架, XcyUI.GLFW(实现IWindow适配window),XcyUI(核心层),XcyUI.SkiaSharp(实现IDraw，适配渲染),可以实现不同的窗口适配和渲染适配，可以很好的扩展到web端以及移动端
- 函数组合式 API + 状态对象驱动界面重组
- 自研 **无Timer高性能动画系统**
- 完整UI布局系统：Row / Column / Flow / 虚拟滚动
- 百万级数据列表轻松稳定 **60fps+**
- 自研渲染管线 + **脏矩形局部刷新**
- 支持 **.NET8 AOT 原生发布**
- 已验证平台：Windows 10+ / Ubuntu22.04 /macOS m4(目前只是验证能运行，具体窗口适配细节以及输入法只适配了windows，目前作者没macOs以及ubantu等环境电脑，后期优先适配苹果）
- 支持热重载、自适应频率分辨率


## hellor代码示例
```csharp
using XcyUI.Controls;
using XcyUI.GLFW;
using static XcyUI.Desktop.Desktop;
using static XcyUI.widgets.XCompose;

Main(new XWindowParams()
{
    Title = "XcyUI GUI软件示例",
    MinWidth = 750,
    MinHeight = 800,
    Compose = () =>
    {
        Box(() =>
        {
            Text("hello world");
        }).Size(600).Card();
    }
});

项目demo主要在Demo里面
大家可以参数demo里面的实现
