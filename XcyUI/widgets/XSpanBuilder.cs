using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;
using XcyUI.widgets.extensions;
using static XcyUI.models.XFunctions;

namespace XcyUI.widgets
{
    public class XSpanBuilder
    {
        private XTextSpan span;
        private XTheme Theme => XThemeManager.Theme;
        private XText textView;
        public XSpanBuilder(XText view, string text)
        {
            textView = view;
            var index = view.Text.Length;
            view.Text = view.Text.Insert(index, text);
            span = new XTextSpan();
            span.StartIndex = index;
            span.EndIndex = view.Text.Length - 1;
            span.Font = view.Font.Copy();
            if (view.Spans == null)
            {
                view.Spans = new List<XTextSpan>();
            }
            view.Spans.Add(span);
        }
        

        public XSpanBuilder Color(XColor color)
        {
            span.Font.Color = new XBrush() { StartColor = color};
            return this;
        }

        public XSpanBuilder Color(XBrush color)
        {
            span.Font.Color = color;
            return this;
        }

        public XSpanBuilder Size(int size)
        {
            span.Font.Size = size.AsPx();
            return this;
        }

        public XSpanBuilder Weight(float weight)
        {
            span.Font.Weight = weight;
            return this;
        }

        public XSpanBuilder FontFamily(string family)
        {
            span.Font.Name = family;
            return this;
        }

        public XSpanBuilder FontPath(string path)
        {
            span.Font.Path = path;
            return this;
        }

        public XSpanBuilder Italic(bool isItalic = true)
        {
            span.Font.Italic = isItalic;
            return this;
        }
        public XSpanBuilder Underline(bool isUnderline = true)
        {
            span.Font.Underline = isUnderline;
            return this;
        }

        public XSpanBuilder DeleteLine(bool isDeleteLine = true)
        {
            span.Font.DeleteLine = isDeleteLine;
            return this;
        }

        public XSpanBuilder TextBody()
        {
            return Color(Theme.Colors.PrimaryText).Size(Theme.Sizes.Body);
        }

        public XSpanBuilder TextCaption()
        {
            return Color(Theme.Colors.PlaceholderText).Size(Theme.Sizes.Caption);
        }

        public XSpanBuilder H1()
        {
            return Color(Theme.Colors.PrimaryText).Size(Theme.Sizes.H1).Weight(Theme.Weights.Large);
        }

        public XSpanBuilder H2()
        {
            return Color(Theme.Colors.PrimaryText).Size(Theme.Sizes.H2).Weight(Theme.Weights.Large);
        }

        public XSpanBuilder H3()
        {
            return Color(Theme.Colors.PrimaryText).Size(Theme.Sizes.H3).Weight(Theme.Weights.Large);
        }

        public XSpanBuilder SmallText()
        {
            Color(Theme.Colors.SecondaryText).Size(Theme.Sizes.Small).Weight(Theme.Weights.Large);
            return this;
        }

        

        public XSpanBuilder Click(XFunction click)
        {
            var builder = new XViewBuilder(textView);
            textView.EventParams.Focusable = true;
            textView.Accessibility.Role = XAccessibilityRole.Link;
            textView.Accessibility.Name = textView.Text.Substring(span.StartIndex, span.EndIndex - span.StartIndex + 1);
            builder
            .OnHover((b, info) =>
            {
                var spanChars = textView.GetChars(span.StartIndex, span.EndIndex) ?? new List<XChar>();
                var isIn = spanChars.Count(n => n.RenderRect.Contain(info.Point)) > 0;
                RenderImp.SetCursor(isIn ? XCursorType.Hand : XCursorType.Arrow);
            })
            .OnLeave((b, info) =>
            {
                RenderImp.SetCursor(XCursorType.Arrow);
            });
            textView.EventParams.EventOrCreate(XEventType.Click).AddFunction($"span_{span.StartIndex}_{span.EndIndex}", (b, info) =>
            {
                var spanChars = textView.GetChars(span.StartIndex, span.EndIndex) ?? new List<XChar>();
                var isIn = spanChars.Count(n => n.RenderRect.Contain(info.Point)) > 0;
                var isKeyboard = info.KeyValue == XKeyValue.Enter || info.KeyValue == XKeyValue.Space;
                if (isIn || isKeyboard)
                {
                    click.Invoke();
                }
            });
            return this;
        }
    }
}
