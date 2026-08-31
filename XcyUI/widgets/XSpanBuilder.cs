using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.utils;
using XcyUI.views;
using XcyUI.widgets.extensions;
using XcyUI.theme;

namespace XcyUI.widgets
{
    public class XSpanBuilder
    {
        private XTextSpan span;
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
            return Color(XTheme.Color.PrimaryText).Size(XTheme.Size.Body);
        }

        public XSpanBuilder TextCaption()
        {
            return Color(XTheme.Color.PlaceholderText).Size(XTheme.Size.Caption);
        }

        public XSpanBuilder H1()
        {
            return Color(XTheme.Color.PrimaryText).Size(XTheme.Size.H1).Weight(XTheme.Weight.Large);
        }

        public XSpanBuilder H2()
        {
            return Color(XTheme.Color.PrimaryText).Size(XTheme.Size.H2).Weight(XTheme.Weight.Large);
        }

        public XSpanBuilder H3()
        {
            return Color(XTheme.Color.PrimaryText).Size(XTheme.Size.H3).Weight(XTheme.Weight.Large);
        }

        public XSpanBuilder SmallText()
        {
            Color(XTheme.Color.SecondaryText).Size(XTheme.Size.Small).Weight(XTheme.Weight.Large);
            return this;
        }

        

        public XSpanBuilder Click(Action click)
        {
            var builder = new XModify(textView);
            builder
            .OnHover((b, info) =>
            {
                var spanChars = textView.GetChars(span.StartIndex, span.EndIndex);
                var isIn = spanChars.Count(n => n.RenderRect.Contain(info.Point)) > 0;
                RenderImp.SetCursor(isIn ? XCursorType.Hand : XCursorType.Arrow);
            })
            .OnLeave((b, info) =>
            {
                RenderImp.SetCursor(XCursorType.Arrow);
            });
            textView.EventParams.EventOrCreate(XEventType.Click).AddFunction($"span_{span.StartIndex}_{span.EndIndex}", (b, info) =>
            {
                var spanChars = textView.GetChars(span.StartIndex, span.EndIndex);
                var isIn = spanChars.Count(n => n.RenderRect.Contain(info.Point)) > 0;
                if (isIn)
                {
                    click.Invoke();
                }
            });
            return this;
        }
    }
}
