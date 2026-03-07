


using XchyUI.GLFW.window;
using XchyUI.models;
using XchyUI.navigation;
using XchyUI.widgets;
using XchyUI.widgets.extensions;
using static XchyUI.widgets.XWidget;

// 初始化glfw相关参数
WindowManager.Get().Init();
// 设置主window
WindowManager.Get().SetMainWindow(new MainWindow());
// 开始渲染循环
WindowManager.Get().Start();
class MainWindow: XWindow
{
    public MainWindow()
    {
        Width = 1300;
        Height = 800;
        Title = "SharpShap2.88.6版本 可以测试一下win7系统是否支持";
    }

    public override void OnLoad()
    {
        var page = new XPage();
        page.RootView = ContentView(() =>
        {
            Column(() =>
            {
                var numState = StateValueOf(0);

                Text().H1().Binding(numState, (builder, num)=>
                {
                    builder.TextValue("一个简单的计算器:" + num);
                }, needLayout: true);

                Text("点击测试").PrimaryButton().Click(() => numState.Value++);
            })
            .Size(500)
            .Space(10) // 子元素之间的间距
            .VerticalAlignment(XVerticalAlignment.Center) // 居中对其
            .Radius(xTheme.Radius.Large) // 大圆角
            .Background(xTheme.Colors.Background) // 背景色
            .Resize() // 鼠标悬浮四周可以改变大小
            .Drag(XDragType.All) // 可以拖拽
            .Shadow(); // 默认阴影为卡片阴影
        }).View;
        skiaNavigation.Navigation.Open(page);
    }
}