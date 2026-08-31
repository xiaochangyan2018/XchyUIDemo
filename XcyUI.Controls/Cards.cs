using XcyUI.theme;
using XcyUI.widgets;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XModify Card(this XModify builder)
        {
            builder.Padding(XTheme.Size.Space16)
                .Background(XTheme.Color.LightFill)
                .Border(XTheme.Color.BaseBorder, 1)
                .Shadow(XTheme.Shadow.Card)
                .Radius(XTheme.Radius.Large);
            return builder;
        }

        public static XModify MiniCard(this XModify builder)
        {
            builder.Padding(XTheme.Size.Space10)
                .Background(XTheme.Color.LightFill)
                .Border(XTheme.Color.BaseBorder, 1)
                .Shadow(XTheme.Shadow.MinCard)
                .Radius(XTheme.Radius.Low);
            return builder;
        }
    }
}
