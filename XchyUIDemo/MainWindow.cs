using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XchyUI.GLFW.window;
using XchyUI.navigation;
using XchyUI.widgets;
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
            page.RootView = ContentView(() =>
            {
                Text("hellow world").H3();
            }).View;
            skiaNavigation.Navigation.Open(page);
        }
    }
}
