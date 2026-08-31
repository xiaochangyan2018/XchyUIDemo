using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;
using static XcyUI.GLFW.XApplication;
using static XcyUI.Controls.Controls;
using XcyUI.theme;
using XcyUI.Controls;

namespace XcyDemo.StudentManagent.Components
{
    public static class XCommonComponents
    {
        public static XModify TopBar(string title)
        {
            return Row(() =>
            {
                Text(title).H2();
                Spacer().Weight(1);
                Text("登陆");
                Icon(SvgRes.User).Size(20);
            })
            .Padding(20)
            .Width(FILL)
            .Space(10)
            .ZIndex(1)
            .Background(XTheme.Color.LighterFill)
            .Shadow(XTheme.Shadow.MinCard);
        }
    }
}
