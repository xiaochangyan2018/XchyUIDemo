using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XchyUI.animation;
using XchyUI.GLFW.window;
using XchyUI.models;
using XchyUI.navigation;
using XchyUI.utils;
using XchyUI.widgets;
using XchyUI.widgets.extensions;
using XchyUIDemo.res;
using static XchyUI.widgets.XWidget;

namespace XchyUIDemo
{
    public class MainWindow: XWindow
    {
        public MainWindow()
        {
            Width = 1380;
            Height = 800;
            Title = "XchyUI Demo";
        }

        public override void OnLoad()
        {
            var page = new XPage();
            XTask.Run(() =>
            {
                SvgResources.Load();
                RenderImp.Invalidate();
            });
            page.RootView = ContentView(() =>
            {
                Column(() =>
                {
                    // 响应式状态
                    var counterNum = StateValueOf(0);
                    Text()
                       .H3() //内置基础样式
                       .Binding(counterNum, (builder, num) =>
                       {
                           builder.TextValue($"计数器：{num}");
                       }, needLayout: true); //改变文本需要重新布局，默认为false

                    // 无Timer循环动画
                    var visibleState = StateValueOf(true);
                    var animateValue = AnimateFloatOf(visibleState, animate =>
                    {
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
            }).View;
            skiaNavigation.Navigation.Open(page);
        }
    }
}
