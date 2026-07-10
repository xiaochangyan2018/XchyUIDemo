using System.Drawing;
using System.Runtime.CompilerServices;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;
using static XcyUI.models.XFunctions;

namespace XcyUI.widgets.extensions
{
    public static class XTextBuilderExtensions
    {
        internal static XViewBuilder TextDefault(this XViewBuilder builder)
        {
            builder
                .Color(XThemeManager.Theme.Colors.PrimaryText)
                .FontSize(XThemeManager.Theme.Sizes.Body)
                .FontWeight(XThemeManager.Theme.Weights.Middle);
            return builder;
        }
        public static XViewBuilder TextAlignment(this XViewBuilder builder, XAlignment alignment)
        {
            builder.AsView<XText>()?.Also(n => n.TextAlignment = alignment);
            return builder;
        }

        public static XViewBuilder FontSize(this XViewBuilder builder, int size)
        {
            builder.AsView<XText>()?.Also(n=>n.Font.Size = size.AsPx());
            return builder;
        }

        public static XViewBuilder FontWeight(this XViewBuilder builder, float weight)
        {
            builder.AsView<XText>()?.Also(n=>n.Font.Weight = weight);
            return builder;
        }

        public static XViewBuilder Content(this XViewBuilder builder, string text)
        {
            builder.AsView<XText>()?.Also(n => n.Text = text ?? n.Text);
            return builder;
        }
        public static string Content(this XViewBuilder builder)
        {
            return builder.AsView<XText>()?.Text ?? "";
        }

        public static XViewBuilder FontName(this XViewBuilder builder, string name)
        {
            builder.AsView<XText>()?.Also(n => n.Font.Name = name);
            return builder;
        }

        public static XViewBuilder FontPath(this XViewBuilder builder, string path)
        {
            builder.AsView<XText>()?.Also(n => n.Font.Path = path);
            return builder;
        }

        public static XViewBuilder FontItalic(this XViewBuilder builder, bool italic = true)
        {
            builder.AsView<XText>()?.Also(n => n.Font.Italic = italic);
            return builder;
        }

        public static XViewBuilder FontUnderline(this XViewBuilder builder, bool underline = true)
        {
            builder.AsView<XText>()?.Also(n => n.Font.Underline = underline);
            return builder;
        }

        public static XViewBuilder FontDeleteLine(this XViewBuilder builder, bool deleteLine = true)
        {
            builder.AsView<XText>()?.Also(n => n.Font.DeleteLine = deleteLine);
            return builder;
        }

        public static XViewBuilder TextSuffix(this XViewBuilder builder, string text = "...")
        {
            builder.AsView<XText>()?.Also(n => n.AddSuffixCharItems(text));
            return builder;
        }

        public static XViewBuilder SingleLine(this XViewBuilder builder)
        {
            return builder.Lines(1);
        }

        public static XViewBuilder ReadOnly(this XViewBuilder builder, bool readOnly = true)
        {
            builder.AsView<XInput>()?.Also(n =>
            {
                n.ReadOnly = readOnly;
                n.Accessibility.IsReadOnly = readOnly;
            });
            return builder;
        }

        public static XViewBuilder Lines(this XViewBuilder builder, int lines)
        {
            builder.AsView<XText>()?.Also(n =>
            {
                n.Lines = lines;
                if (n is XInput)
                {
                    n.Accessibility.Role = lines == 1 ? XAccessibilityRole.TextBox : XAccessibilityRole.TextArea;
                    n.Accessibility.IsMultiline = lines != 1;
                }
            });
            return builder;
        }

        public static XViewBuilder MaxLines(this XViewBuilder builder, int maxLines)
        {
            builder.AsView<XText>()?.Also(n => n.MaxLines = maxLines);
            return builder;
        }

        public static XViewBuilder Hint(this XViewBuilder builder, string text)
        {
            builder.AsView<XInput>()?.Also(n =>
            {
                n.Hint = text;
                n.Accessibility.Hint = text;
            });
            return builder;
        }

        public static XViewBuilder CursorColor(this XViewBuilder builder, XColor color)
        {
            builder.AsView<XInput>()?.Also(n => n.CurorStyle.Background = new XBrush() { StartColor = color});
            return builder;
        }

        public static XViewBuilder PasswordKey(this XViewBuilder builder, char? key)
        {
            builder.AsView<XInput>()?.Also(n =>
            {
                n.SetPasswordChar(key);
                n.Accessibility.IsPassword = key != null;
            });
            return builder;
        }

        internal static XViewBuilder HoverCursor(this XViewBuilder builder, XCursorType cursorType, string eventKey = "hoverCursor", [CallerLineNumber] int key = 0)
        {
            return builder.ToggleHover(isHover =>
            {
                RenderImp.SetCursor(isHover ? cursorType : XCursorType.Arrow);
            }, eventKey, key)
            .OnDispose(b => RenderImp.SetCursor(XCursorType.Arrow), eventKey);
        }
    }
}
