using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyDemo.StudentManagent.Theme
{
    public static class XStyleBuilder
    {
        public static XModify DCard(this XModify builder)
        {
            builder
                .Background(XTheme.Color.LighterFill)
                .Shadow(XTheme.Shadow.MinCard)
                .Radius(XTheme.Radius.Middle);
            return builder;
        }

        public static XModify IconButtonStyle(this XModify builder)
        {
            return builder.Size(30).IconSize(24).Radius(XTheme.Radius.Low);
        }

        public static XModify FormRow(this XModify builder)
        {
            return builder.Size(FILL, WRAP).Space(10).Padding(10).HorizontalAlignment(XHorizontalAlignment.Left);
        }

        public static XShadow DataRowLeftShadow = new XShadow()
        {
            Dx = 0,
            Blur = 6,
            Color = XColors.Black.Copy(0.2f)
        };
    }
}
