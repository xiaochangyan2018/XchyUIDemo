using System.Runtime.CompilerServices;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;

namespace XcyUI.widgets.extensions
{
    public static class XTextBuilderExtensions
    {
        internal static XModify TextDefault(this XModify builder)
        {
            builder
                .Color(XTheme.Color.PrimaryText)
                .FontSize(XTheme.Size.Body)
                .FontWeight(XTheme.Weight.Middle);
            return builder;
        }
        public static XModify TextAlignment(this XModify builder, XAlignment alignment)
        {
            builder.AsView<XText>()?.Also(n => n.TextAlignment = alignment);
            return builder;
        }

        public static XModify FontSize(this XModify builder, int size)
        {
            builder.AsView<XText>()?.Also(n=>n.Font.Size = size.AsPx());
            return builder;
        }

        public static XModify FontWeight(this XModify builder, float weight)
        {
            builder.AsView<XText>()?.Also(n=>n.Font.Weight = weight);
            return builder;
        }

        public static XModify Content(this XModify builder, string text)
        {
            builder.AsView<XText>()?.Also(n => n.Text = text ?? n.Text);
            return builder;
        }
        public static string Content(this XModify builder)
        {
            return builder.AsView<XText>()?.Text ?? "";
        }

        public static XModify FontName(this XModify builder, string name)
        {
            builder.AsView<XText>()?.Also(n => n.Font.Name = name);
            return builder;
        }

        public static XModify FontPath(this XModify builder, string path)
        {
            builder.AsView<XText>()?.Also(n => n.Font.Path = path);
            return builder;
        }

        public static XModify FontItalic(this XModify builder, bool italic = true)
        {
            builder.AsView<XText>()?.Also(n => n.Font.Italic = italic);
            return builder;
        }

        public static XModify FontUnderline(this XModify builder, bool underline = true)
        {
            builder.AsView<XText>()?.Also(n => n.Font.Underline = underline);
            return builder;
        }

        public static XModify FontDeleteLine(this XModify builder, bool deleteLine = true)
        {
            builder.AsView<XText>()?.Also(n => n.Font.DeleteLine = deleteLine);
            return builder;
        }

        public static XModify TextSuffix(this XModify builder, string text = "...")
        {
            builder.AsView<XText>()?.Also(n => n.AddSuffixCharItems(text));
            return builder;
        }

        public static XModify SingleLine(this XModify builder)
        {
            return builder.Lines(1);
        }

        public static XModify ReadOnly(this XModify builder, bool readOnly = true)
        {
            builder.AsView<XInput>()?.Also(n => n.ReadOnly = readOnly);
            return builder;
        }

        public static XModify Lines(this XModify builder, int lines)
        {
            builder.AsView<XText>()?.Also(n => n.Lines = lines);
            return builder;
        }

        public static XModify MaxLines(this XModify builder, int maxLines)
        {
            builder.AsView<XText>()?.Also(n => n.MaxLines = maxLines);
            return builder;
        }

        public static XModify Hint(this XModify builder, string text)
        {
            builder.AsView<XInput>()?.Also(n => n.Hint = text);
            return builder;
        }

        public static XModify CursorColor(this XModify builder, XColor color)
        {
            builder.AsView<XInput>()?.Also(n => n.CurorStyle.Background = new XBrush() { StartColor = color});
            return builder;
        }

        public static XModify Cursor(this XModify builder, XCursorType type)
        {
            RenderImp.SetCursor(type);
            return builder;
        }

        public static XModify PasswordKey(this XModify builder, char? key)
        {
            builder.AsView<XInput>()?.Also(n =>
            {
                n.SetPasswordChar(key);
            });
            return builder;
        }

        internal static XModify HoverCursor(this XModify builder, XCursorType cursorType, string eventKey = "hoverCursor", [CallerLineNumber] int key = 0)
        {
            return builder.ToggleHover(isHover =>
            {
                RenderImp.SetCursor(isHover ? cursorType : XCursorType.Arrow);
            }, eventKey, key)
            .OnDispose(b => RenderImp.SetCursor(XCursorType.Arrow), eventKey);
        }

        public static XModify Focus(this XModify builder, bool focus = true, bool isSelect = false)
        {
            builder.AsView<XInput>()?.Also(n => n.Focus(focus, isSelect));
            builder.View.EventParams?.Event(focus ? XEventType.Focused : XEventType.LossFocused)?.Invoke(builder.View, new XEventInfo());
            if (focus)
            {
                XEvent.FocusView = builder.View;
            }
            for (int i = 0; i < builder.View.ChildCount(); i++)
            {
                var input = builder.View.ChildElemnt(i) as XInput;
                input?.Focus(focus, isSelect);
            }
            return builder;
        }
    }
}
