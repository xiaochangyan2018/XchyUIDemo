# XchyUI函数式组合跨平台UI引擎软件
内核 <200KB · .NET8 AOT跨平台 · 百万数据60fps+ · 无Web套壳

## ✨ 项目介绍
本人历经 **6年业余时间**，全链路自研了这套 **纯C#用户态UI引擎**。
从最初基于 **WinForms + GDI** 探索渲染与布局，到历经多次架构重构与彻底推翻重做，最终定型于 **GLFW + SkiaSharp** 技术栈，打造出一款真正从底层驱动、面向高性能与轻量化的跨平台UI方案。

当前PC客户端开发，大多基于以下技术体系：
• .NET 官方框架：WinForms / WPF / WinUI / .NET MAUI
• 开源跨平台方案：Avalonia
• Web 套壳技术：Electron / Tauri
• C++ 原生框架：Qt

绝大多数开发者与企业，都选择在这些成熟框架之上做二次封装、组件扩展，以此快速实现业务需求。
但**真正愿意从源头开始，全链路自研一整套UI引擎的开发者少之又少**。

而我的这套引擎，正是**从0到1完全自研**：
渲染管线、视图布局系统、动画调度、虚拟滚动、事件分发、主题体系、状态管理，全部自主实现，形成**全链路闭环**。
可满足 **90% 以上的桌面客户端UI需求**，复杂绘图可直接对接底层Skia渲染，生成绘制指令并提交GPU执行。

框架设计追求极简与高效：
• 单线程架构 + 对象复用机制，大幅降低GC压力
• 元素结构无冗余设计，内存占用极低
• 函数式组合编程 + 状态驱动界面重组
• 组件树一次声明、多处复用
• 业务逻辑与UI结构高度内聚，不分散
• 思想贴近 React / Flutter / Jetpack Compose，现代前端/移动端开发者可快速上手

与传统XML、重量级框架不同，本引擎坚持 **小而精** 的设计理念：
只提供最基础的原子组件，所有复杂组件（DataGrid、TreeView、图表、卡片等）均通过基础组件**积木式组合**实现。
框架不提供冗余、不内置臃肿组件，保持最轻量、最灵活、最可定制的核心优势。

全程无黑盒、无深度封装、无Web套壳、无浏览器内核，回归原生渲染本质。

---

## 🎯 核心特性
- 纯C#用户态实现，**Release 内核 DLL < 200KB**
- 函数组合式 API + 状态对象驱动界面重组
- 自研 **无Timer高性能动画系统**
- 完整布局系统：Row / Column / Flow / 虚拟滚动
- 百万级数据列表轻松稳定 **60fps+**
- 自研渲染管线 + **脏矩形局部刷新**
- 窗口对接：Silk.NET.GLFW
- 渲染引擎：SkiaSharp
- 支持 **.NET8 AOT 原生发布**
- 已验证平台：Windows 10+ / Ubuntu（虚拟机unbantu 2核 4G UI依然很流畅)
- macOS 理论100%支持（未验证是因为本人还没有苹果电脑）
- 支持热重载
- 插拔式架构，可快速对接其他平台与渲染器


## 🧩 内置基础组件
Text / Input / Icon / Row / Column / Flow
LazyRow / LazyColumn / LazyGrid / PopupCard

复杂组件（DataGrid、TreeView、图表等）
均可通过基础组件 **积木式组合** 实现。

---

## 🚀 已实现 Demo
1. 百万数据高性能虚拟滚动列表
2. 仿微信 PC 端主界面
3. 基础图表（饼图、柱状图、折线图、仪表盘）

---

## 📌 极简示例代码
```csharp
ContentView(() => {
    Column(() => {
        // 响应式状态
        var counterNum = StateValueOf(0);
        Text()
           .H3() //内置基础样式
           .Binding(counterNum, (builder, num) => {
               builder.TextValue($"计数器：{num}");
           }, needLayout: true); //改变文本需要重新布局，默认为false

        // 无Timer循环动画
        var visibleState = StateValueOf(true);
        var animateValue = AnimateFloatOf(visibleState, animate => {
               animate.Duration = 800;
               animate.Times = int.MaxValue;
               animate.Delay = 200;
               animate.Interpolator = XAnimationInterpolator.Uniform;
           });

        Icon(SvgResources.CircleProgress)
           .Size(32)
           .Binding(animateValue, (builder, value) =>
               builder.Rotate(value * 360)
           );

        // 点击交互
        Text("点击增加计数")
           .PrimaryButton()
           .Click(() => counterNum.Value++);
    })
   .Size(WRAP)
   .Space(10);
});
```

### 可以下载下来直接visual stuido 2022及以上打开，直接运行, 项目会一直优化更新

### 热重载使用：
框架并没有提供完整的热重载功能，而是提供刷新界面的方法
```C#
XWidget.HotReload.Send(true);
RenderImp.InvalidateAll();
```
demo里面加了一个 HotkeyManager用来监听ctrl+s来出发alt+10 触发热重载功能，然后再调用上面的方法刷新界面，同时也需要在.csproj文件里面添加HotReloadEnabled为true
```C#
<HotReloadEnabled>true</HotReloadEnabled>
```

### 关于win7系统的支持，当前因为使用的是.net 8 + SkiaSharp3.119.1不能在win上运行，后面会出一个低版本的demo用在win7运行

该UI引擎具体具体如何使用加微信号 ***xiaochangyanwx*** 入群分享
