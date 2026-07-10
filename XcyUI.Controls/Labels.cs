using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.theme.XThemeManager;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XViewBuilder TextBody(this XViewBuilder builder)
        {
            builder.Color(Theme.Colors.PrimaryText).FontSize(Theme.Sizes.Body).FontWeight(Theme.Weights.Middle);
            return builder;
        }

        public static XViewBuilder TextCaption(this XViewBuilder builder)
        {
            builder.Color(Theme.Colors.PlaceholderText).FontSize(Theme.Sizes.Caption);
            return builder;
        }

        public static XViewBuilder H1(this XViewBuilder builder)
        {
            builder.Color(Theme.Colors.RegularText).FontSize(Theme.Sizes.H1).FontWeight(Theme.Weights.Large)
                .AccessibilityHeadingLevel(1);
            return builder;
        }

        public static XViewBuilder H2(this XViewBuilder builder)
        {
            builder.Color(Theme.Colors.RegularText).FontSize(Theme.Sizes.H2).FontWeight(Theme.Weights.Large)
                .AccessibilityHeadingLevel(2);
            return builder;
        }

        public static XViewBuilder H3(this XViewBuilder builder)
        {
            builder.Color(Theme.Colors.RegularText)
                .FontSize(Theme.Sizes.H3).FontWeight(Theme.Weights.Large)
                .AccessibilityHeadingLevel(3);
            return builder;
        }

        public static XViewBuilder SmallText(this XViewBuilder builder)
        {
            builder.Color(Theme.Colors.SecondaryText).FontSize(Theme.Sizes.Small).FontWeight(Theme.Weights.Large);
            return builder;
        }
    }
}
