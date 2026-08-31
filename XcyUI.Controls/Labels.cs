using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XModify TextBody(this XModify builder)
        {
            builder.Color(XTheme.Color.PrimaryText).FontSize(XTheme.Size.Body).FontWeight(XTheme.Weight.Middle);
            return builder;
        }

        public static XModify TextCaption(this XModify builder)
        {
            builder.Color(XTheme.Color.PlaceholderText).FontSize(XTheme.Size.Caption);
            return builder;
        }

        public static XModify H1(this XModify builder)
        {
            builder.Color(XTheme.Color.RegularText).FontSize(XTheme.Size.H1).FontWeight(XTheme.Weight.Large);
            return builder;
        }

        public static XModify H2(this XModify builder)
        {
            builder.Color(XTheme.Color.RegularText).FontSize(XTheme.Size.H2).FontWeight(XTheme.Weight.Large);
            return builder;
        }

        public static XModify H3(this XModify builder)
        {
            builder.Color(XTheme.Color.RegularText)
                .FontSize(XTheme.Size.H3).FontWeight(XTheme.Weight.Large);
            return builder;
        }

        public static XModify SmallText(this XModify builder)
        {
            builder.Color(XTheme.Color.SecondaryText).FontSize(XTheme.Size.Small).FontWeight(XTheme.Weight.Large);
            return builder;
        }
    }
}
