using System;
using XcyUI.Controls.Utils;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;
using XcyUI.widgets;
using XcyUI.widgets.extensions;

namespace XcyUI.Controls
{
    public static class CommonExtensions
    {
        public static XModify DefaultBorder(this XModify builder)
        {
            builder.Border(XTheme.Color.BaseBorder, XTheme.Size.Border);
            return builder;
        }

        public static XModify BottomBorder(this XModify builder)
        {
            builder.Border(XTheme.Color.BaseBorder, 0, 0, 0, XTheme.Size.Border);
            return builder;
        }

        public static XModify RightBorder(this XModify builder)
        {
            builder.Border(XTheme.Color.BaseBorder, 0, 0, XTheme.Size.Border, 0);
            return builder;
        }

        public static XModify TopBorder(this XModify builder)
        {
            builder.Border(XTheme.Color.BaseBorder, 0, XTheme.Size.Border, 0, 0);
            return builder;
        }

        public static XModify LeftBorder(this XModify builder)
        {
            builder.Border(XTheme.Color.BaseBorder, XTheme.Size.Border, 0, 0, 0);
            return builder;
        }
        public static XRect ToCircle(this XRect rect)
        {
            int radius = Math.Min(rect.Width, rect.Height) / 2;
            rect = new XRect(rect.Center.X - radius, rect.Center.Y - radius, radius * 2, radius * 2);
            return rect;
        }

        public static XModify ColorAll(this XModify builder, XColor color)
        {
            builder.View.ModifyChild(n =>
            {
                (n as XText)?.Also(a => a.Font.Color = new XBrush(color));
                (n as XIcon)?.Also(a => a.Color = new XBrush(color));
            });
            return builder;
        }

        public static XModify SingleLineAll(this XModify builder)
        {
            builder.View.ModifyChild(n =>
            {
                (n as XText)?.Also(a => a.Lines = 1);
            });
            return builder;
        }

        public static XModify BackgroundAll(this XModify builder, XColor color)
        {
            builder.View.ModifyChild(n =>
            {
                if (n is XGroup)
                {
                    n.RefreshCache();
                    n.Style.Background = new XBrush() { StartColor = color };
                }
            });
            return builder;
        }

        public static XModify IconSizeAll(this XModify builder, int size)
        {
            builder.View.ModifyChild(n =>
            {
                (n as XIcon)?.Also(a =>
                {
                    a.IconHeight = size.AsPx();
                    a.IconWidth = size.AsPx();
                });
            });
            return builder;
        }

        public static XModify ContentAll(this XModify builder, string text)
        {
            builder.View.ModifyChild(n =>
            {
                (n as XText)?.Also(a =>
                {
                    a.Text = text;
                });
            });
            return builder;
        }

        public static XModify FontSizeAll(this XModify builder, int size)
        {
            builder.View.ModifyChild(n =>
            {
                (n as XText)?.Also(a => a.Font.Size = size.AsPx());
            });
            return builder;
        }

        public static XModify Bind<T>(this XModify builder, XState<T> state, Func<T, string> function)
        {
            builder.AsView<XText>()?.Also(text =>
            {
                builder.Bind(state, (b, value) =>
                {
                    b.Content(function(value));
                }, needLayout: true);
            });
            return builder;
        }

        public static XModify BindInput(this XModify builder, XState<string> state)
        {
            builder.AsView<XInput>()?.Also(text =>
            {
                builder
                .TextChanged((b, info) =>
                {
                    state.Value = builder.Content();
                })
                .Bind(state, (b, value) =>
                {
                    builder.Content(state.Value);
                }, needLayout: true);
            });
            return builder;
        }

        public static XModify Hand(this XModify builder, XColor? hoverColor = null, XColor? defaultColor = null)
        {
            return builder
                .Color(defaultColor ?? XTheme.Color.PrimaryText)
                .HoverColor(hoverColor ?? XTheme.Color.Primary)
                .HoverCursor(XCursorType.Hand);
        }

        public static XModify ClearSelectContent(this XModify builder)
        {
            return builder.FirstInput(n =>
            {
                (n.View as XInput).Select(0, 0);
            });
        }

        public static XModify FirstInput(this XModify builder, Action<XModify> func)
        {
            var isInput = false;
            builder.View.ModifyChild(n =>
            {
                if (!isInput)
                {
                    isInput = n is XInput;
                    if (isInput)
                    {
                        func(new XModify(n));
                    }
                }
            });
            return builder;
        }

        public static XModify FirstText(this XModify builder, Action<XModify> func)
        {
            var isText = false;
            builder.View.ModifyChild(n =>
            {
                if (!isText)
                {
                    isText = n is XText;
                    if (isText)
                    {
                        func(new XModify(n));
                    }
                }
            });
            return builder;
        }

        public static XModify InputType(this XModify builder, InputType type, Action<string> onValidateFail = null)
        {
            builder.FirstInput(inputBuilder =>
            {
                inputBuilder.OnLossFocused(b =>
                {
                    XTask.RunDelayed(() =>
                    {
                        RenderImp.PostToQueue(() =>
                        {
                            var result = InputRegex.Validate(type, b.Content());
                            if (!result)
                            {
                                onValidateFail?.Invoke(b.Content());
                            }
                        });
                    }, 100);

                }, "inputType_lossFucused");
            });
            return builder;
        }

        public static XModify Bind(this XModify builder, Action<string> valueChanged)
        {
            return builder.FirstText(n =>
            {
                n.TextChanged((b, text) =>
                {
                    valueChanged?.Invoke(text);
                });
            });
        }
        public static XModify Bind<T>(this XModify builder, Action<XModify, T> valueChanged)
        {
            return builder.FirstText(n =>
            {
                n.TextChanged((b, text) =>
                {
                    try
                    {
                        T newValue = (T)Convert.ChangeType(text, typeof(T));
                        valueChanged.Invoke(b, newValue);
                    }
                    catch
                    {

                    }
                }, "bind_text_changed");
            });
        }
        public static XModify Bind<T>(this XModify builder, Action<T> valueChanged)
        {
            return builder.FirstText(n =>
            {
                n.TextChanged((b, text) =>
                {
                    try
                    {
                        T newValue = (T)Convert.ChangeType(text, typeof(T));
                        valueChanged.Invoke(newValue);
                    }
                    catch
                    {

                    }
                }, "bind_text_changed");
            });
        }
    }
}
