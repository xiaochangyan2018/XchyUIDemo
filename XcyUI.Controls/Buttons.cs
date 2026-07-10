using XcyUI.models;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.models.XFunctions;
using static XcyUI.theme.XThemeManager;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XViewBuilder PrimaryButton(this XViewBuilder builder, XFunction function = null)
        {
            return builder.Padding(horizontal: Theme.Sizes.Space20, vertical: Theme.Sizes.Space12)
                .Background(Theme.Colors.Primary)
                .EnableEvent(true)
                .Lines(1)
                .TextAlignment(XAlignment.Center)
                .Shadow(Theme.Shadows.PrimaryButton)
                .ColorAll(Theme.Colors.White)
                .ColorAll(Theme.Colors.White)
                .IconSizeAll(20)
                .FontWeight(Theme.Weights.Button)
                .Border(Theme.Colors.Primary, 0)
                .Radius(Theme.Radius.Middle)
                .AccessibilityRole(XAccessibilityRole.Button)
                .AccessibilityMergeDescendants()
                .Click(function);
        }

        public static XViewBuilder SubButton(this XViewBuilder builder, XFunction function = null)
        {
            return builder.PrimaryButton(function)
                .Shadow(new XShadow())
                .ColorAll(Theme.Colors.RegularText)
                .IconSizeAll(20)
                .Background(Theme.Colors.LightFill)
                .Border(Theme.Colors.BaseBorder, Theme.Sizes.Border);
        }

        public static XViewBuilder DangerButton(this XViewBuilder builder, XFunction function = null)
        {
            return builder.PrimaryButton(function)
                .Background(Theme.Colors.Danger);
        }

        public static XViewBuilder DisableButton(this XViewBuilder builder)
        {
            return builder.PrimaryButton()
                .EnableEvent(false)
                .Focusable(false)
                .AccessibilityEnabled(false)
                .TabIndex(-1)
                .Shadow(new XShadow())
               .ColorAll(Theme.Colors.DisabledText)
               .Border(new XBorder())
               .Background(Theme.Colors.LightFill);
        }

        public static XViewBuilder Disable(this XViewBuilder builder)
        {
            return builder.EnableEvent(false)
               .Focusable(false)
               .AccessibilityEnabled(false)
               .TabIndex(-1)
               .Alpha(Theme.Colors.DisabledAlpha);
        }
    }
}
