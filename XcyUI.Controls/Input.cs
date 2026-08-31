using XcyUI.Controls.Utils;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XModify PrimaryInput(this XModify builder)
        {
            builder.DefaultInput()
                .Padding(horizontal: XTheme.Size.Space16, vertical: XTheme.Size.Space12)
                .OnFocused((_) => builder.FocusInput(), "input_focused")
                .OnLossFocused(_ => builder.DefaultInput(), "input_loss_focused")
                .BubbleEvent(XEventType.Focused)
                .BubbleEvent(XEventType.LossFocused);
            return builder;
        }
        private static XModify DefaultInput(this XModify builder)
        {
            builder
                .Tabindex(0)
                .Color(XTheme.Color.RegularText)
                .Border(XTheme.Color.BaseBorder, XTheme.Size.Border)
                .HoverBorderColor(XTheme.Color.DarkerBorder)
                .BubbleEvent(XEventType.Hover, false)
                .Radius(XTheme.Radius.Low)
                .Focusable(true)
                .Shadow(XTheme.Shadow.Input);
            return builder;
        }
        private static XModify FocusInput(this XModify builder)
        {
            builder
                .DefaultInput()
                .Border(XTheme.Color.Primary, XTheme.Size.Border)
                .HoverBorderColor(XTheme.Color.Primary)
                .Shadow(XTheme.Shadow.Input);
            return builder;
        }

        public static XModify ErrorInput(this XModify builder)
        {
            builder.DefaultInput()
                .OnFocused(null, "input_focused")
                .OnLossFocused(null, "input_loss_focused")
                .Border(XTheme.Color.Danger, XTheme.Size.Border)
                .HoverBorderColor(XTheme.Color.Danger);
            return builder;
        }

        public static XModify IconInput(int resId, string text = "", bool isRight = true)
        {
            return Row(() =>
            {
                if (isRight)
                {
                    Input(text).Weight(1).Padding(XTheme.Size.Space16, XTheme.Size.Space12);
                    Icon(resId).IconSize(20).Margin(right: XTheme.Size.Space16);
                }
                else
                {
                    Icon(resId).IconSize(20).Margin(left: XTheme.Size.Space16);
                    Input(text).Weight(1).Padding(XTheme.Size.Space16, XTheme.Size.Space12);
                }
            }).Space(10).PrimaryInput().Width(200);
        }
        public static XModify NumberInput(
            XState<string> valueState,
            float step = 1,
            int precision = 0)
        {
            return Box(() =>
            {
                string formate = "0." + new string('0', precision);
                var hoverState = StateValueOf(false);
                Input().PrimaryInput().Width(FILL)
                .KeyPress((b, info) =>
                {
                    var result = InputRegex.Validate(InputType.Number, b.Content() + info.KeyChar);
                    if (!result && info.KeyChar > 0)
                    {
                        info.isAbandoned = true;
                    }
                    if (info.KeyValue == XKeyValue.Up)
                    {
                        valueState.Value = (float.Parse(valueState.Value) + step).ToString(formate);
                        b.ClearSelectContent();
                    }
                    if (info.KeyValue == XKeyValue.Down)
                    {
                        valueState.Value = (float.Parse(valueState.Value) - step).ToString(formate);
                        b.ClearSelectContent();
                    }
                }, "numberInput_inputType_number")
                .BindInput(valueState)
                .Bind(hoverState, (b, isHover) =>
                {
                    if (isHover || b.View.IsFocus())
                    {
                        b.FocusInput();
                    }
                    else if (b.View.RenderRect.Contain(XEvent.Point))
                    {
                        b.PrimaryInput().Border(XTheme.Color.DarkerBorder).Cursor(XCursorType.Input);
                    }
                    else
                    {
                        b.PrimaryInput();
                    }
                    b.Padding(right: XTheme.Size.Space16 + 31);
                });

                Column(() =>
                {
                    Icon(SvgRes.ArrowUp).Width(FILL).IconSize(15).Weight(1)
                    .BottomBorder().Hand().Click((b, info) =>
                    {
                        valueState.Value = (float.Parse(valueState.Value) + step).ToString(formate);
                        hoverState.Send(true);
                    });
                    Icon(SvgRes.ArrowDown).Width(FILL).IconSize(15).Weight(1)
                    .Hand().Click(() =>
                    {
                        hoverState.Send(true);
                        valueState.Value = (float.Parse(valueState.Value) - step).ToString(formate);
                    });
                })
                .Alignment(XAlignment.RightCenter)
                .ToggleHover(isHover => hoverState.Value = isHover)
                .Radius(top: XTheme.Radius.Low, right: XTheme.Radius.Low)
                .Margin(1)
                .Size(30, FILL)
                .LeftBorder()
                .Background(XTheme.Color.LighterBorder);
            })
            .Width(200).Height(WRAP);
        }
        public static XModify NumberInput(
            float value = 0,
            float step = 1,
            int precision = 0)
        {
            var valueState = StateValueOf(value + "");
            return NumberInput(valueState, step, precision);
        }
    }
}
