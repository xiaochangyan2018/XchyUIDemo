using System;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XModify PrimaryButton(this XModify builder, Action function = null)
        {
            return builder
                .Tabindex(0)
                .Padding(horizontal: XTheme.Size.Space20, vertical: XTheme.Size.Space12)
                .Background(XTheme.Color.Primary)
                .EnableEvent(true)
                .SingleLineAll()
                .TextAlignment(XAlignment.Center)
                .Shadow(XTheme.Shadow.PrimaryButton)
                .ColorAll(XTheme.Color.White)
                .IconSizeAll(20)
                .FontWeight(XTheme.Weight.Middle)
                .Border(XTheme.Color.Primary, 0)
                .Radius(XTheme.Radius.Middle)
                .HoverCursor(XCursorType.Arrow)
                .Click(function);
        }

        public static XModify SubButton(this XModify builder, Action function = null)
        {
            return builder.PrimaryButton(function)
                .Shadow(new XShadow())
                .ColorAll(XTheme.Color.RegularText)
                .IconSizeAll(20)
                .Background(XTheme.Color.LightFill)
                .Border(XTheme.Color.BaseBorder, XTheme.Size.Border);
        }

        public static XModify DangerButton(this XModify builder, Action function = null)
        {
            return builder.PrimaryButton(function)
                .Background(XTheme.Color.Danger);
        }

        public static XModify DisableButton(this XModify builder)
        {
            return builder.PrimaryButton()
                .EnableEvent(false)
                .Shadow(new XShadow())
               .ColorAll(XTheme.Color.DisabledText)
               .Border(new XBorder())
               .Background(XTheme.Color.LightFill);
        }

        public static XModify Disable(this XModify builder, bool isDisable = true)
        {
            return builder.EnableEvent(!isDisable)
               .Alpha(isDisable?XTheme.Color.DisabledAlpha: 1);
        }
    }
}
