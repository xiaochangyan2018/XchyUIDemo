using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.utils;

namespace XcyUI.views
{
    public class XText: XView
    {
        public XFont Font { get; set; }
        public int Lines { get; set; }
        public int MaxLines { get; set; }
        public string Suffix { get; private set; }
        public XAlignment TextAlignment { get; set; }
        protected string _text;
        protected bool isChangedText;
        public bool IsContentOver { get; internal set; }
        public string Text
        {
            get => _text;
            set
            {
                isChangedText = _text != value;
                if (isChangedText)
                {
                    _text = value;
                    OnTextChanged(value);
                }
            }
        }
        public int RowSpace { get; set; }
        public int ColumnSpace { get; set; }
        public bool IsAutoSpace { get; set; }
        protected List<XTextRow> rows;
        protected Dictionary<int,List<XChar>> drawChars;
        protected List<XChar> charItems;
        protected XChar[] suffixCharItems;
        protected bool IsLayoutAllItem;
        protected XRect _childRect;
        private int textMeasureHashCode;
        internal List<XTextSpan> Spans { get; set; }
        public XText()
        {
            _text = "";
            TextAlignment = XAlignment.LeftTop;
            rows = new List<XTextRow>();
            drawChars = new Dictionary<int, List<XChar>>();
            charItems = new List<XChar>();
            Font = new XFont();
            IsAutoSpace = true;
            Style.IsClip = false;
        }

        internal void AddSuffixCharItems(string suffix)
        {

            Suffix = suffix;
            if (string.IsNullOrEmpty(suffix))
            {
                suffixCharItems = Array.Empty<XChar>();
                return; 
            }
            var chars = suffix.ToCharArray();
            suffixCharItems = new XChar[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                suffixCharItems[i].Update(index: i, value: chars[i], font: Font);
                suffixCharItems[i].Measure();
            }
        }

        public override bool IsNeedMeasure()
        {
            return base.IsNeedMeasure() || (LayoutParams.IsWrapWidth && Lines == 0) || textMeasureHashCode != GetTextMeasrueHashCode() || charItems.Count == 0;
        }

        private int GetTextMeasrueHashCode()
        {
            return (Font.GetHashCode(), Text, Lines, MaxLines, TextAlignment, Suffix).GetHashCode();
        }

        public override void Measure()
        {
            if (IsNeedMeasure())
            {
                base.Measure();
            }
            textMeasureHashCode = GetTextMeasrueHashCode();
        }

        protected override void OnMeasure()
        {
            base.OnMeasure();
            MesaureText(Text, Font, rows, charItems);
        }

        protected void MesaureText(string text, XFont font, List<XTextRow> rows, List<XChar> charItems)
        {
            DoMesaureText(text, font, rows, charItems);
        }

        private void DoMesaureText(string text, XFont font, List<XTextRow> rows, List<XChar> charItems)
        {
            if (IsAutoSpace)
            {
                var fontSpace = RenderImp.MeasureText("A", font);
                _childRect.Height = font.LineHeight;
            }

            if (LayoutParams.IsWrapHeight)
            {
                Height = Parent?.LayoutParams?.IsWrapHeight == true ? 5000 : this.ParentHeight();
            }

            if (Lines > 0 && LayoutParams.IsWrapHeight)
            {
                Height = font.LineHeight * Lines + LayoutParams.Padding.VerticalSize;
            }

            if (MaxLines > 0)
            {
                LayoutParams.MaxHeight = MaxLines * font.LineHeight + LayoutParams.Padding.VerticalSize;
            }

            if (LayoutParams.IsWrapWidth)
            {
                Width = Parent?.LayoutParams?.IsWrapWidth == true ? 5000 : this.ParentWidth();
                if(Width == 0)
                {
                    Width = 5000;
                }
            }
            this.MeasureMaxOrMin();
            var contentRect = ContentRect;
            charItems.Clear();
            rows.Clear();
            IsContentOver = false;
            if (!string.IsNullOrEmpty(text))
            {
                var left = 0;
                var top = 0;
                var chars = text.ToCharArray();
                var row = new XTextRow(rows.Count);
                row.Height = font.LineHeight; // 后面可以单独设置行高
                rows.Add(row);
                var lineHeight = 0;
                for (int i = 0; i < chars.Length; i++)
                {
                    if (Spans != null && Spans.Count > 0)
                    {
                        var span = Spans.FirstOrDefault(n => n.StartIndex <= i && i <= n.EndIndex);
                        if (!span.Equals(XTextSpan.Empty))
                        {
                            font = span.Font;
                        }
                    }
                    var item = new XChar();
                    item.Update(index: i, value: chars[i], font: font);
                    item.Measure();
                    // 还需要优化
                    lineHeight = Math.Max(lineHeight, font.LineHeight);
                    row.Height = lineHeight;
                    var isNewLineChar = chars[i] == '\r' && i + 1 < chars.Length && chars[i + 1] == '\n';
                    var isOverRight = left + item.Width + ColumnSpace > contentRect.Width;
                    var isNewLine = Lines != 1 && (isOverRight || isNewLineChar);
                    if (isNewLine)
                    {
                        if (isNewLineChar)
                        {
                            item.Update(x: left, y: top, width: 0, isNewLine: true);
                            row.Add(item);
                        }
                        left = 0;
                        top += lineHeight + RowSpace;
                        row.Height = lineHeight;
                        row.Mesurse();
                        rows[row.Index] = row;
                        row = new XTextRow(rows.Count);
                        row.Y = top;
                        row.Height = lineHeight;
                        rows.Add(row);
                        if (isNewLineChar)
                        {
                            charItems.Add(item);
                            i++;
                            item = new XChar();
                            item.Update(index: i, value: chars[i], font: font, x: left, y: top, width: 0, isNewLine: true);
                            row.Add(item);
                            charItems.Add(item);
                            continue;
                        }
                    }

                    if (!IsLayoutAllItem && ((Lines == 1 && isOverRight) || (top + lineHeight > Height && rows.Count > 1)))
                    {
                        if (isNewLine)
                        {
                            rows.Remove(row);
                            row = rows.Last();
                        }
                        AddSuffix();
                        IsContentOver = true;
                        break;
                    }

                    item.X = left;
                    item.Y = top;
                    left += item.Width + ColumnSpace;
                    if (!isNewLineChar)
                    {
                        row.Add(item);
                    }
                    charItems.Add(item);
                }
                row.Mesurse();
                rows[row.Index] = row;
                _childRect = new XRect(rows.Max(n => n.Width), row.RenderRect.Bottom - rows[0].Y);
            }

            if (LayoutParams.IsWrapWidth)
            {
                Width = _childRect.Width + LayoutParams.Padding.HorizontalSize;
            }

            if (LayoutParams.IsWrapHeight && Lines == 0)
            {
                Height = _childRect.Height + LayoutParams.Padding.VerticalSize;
            }

            this.MeasureMaxOrMin();

            if (rows.Count > 1 && TextAlignment != XAlignment.None && TextAlignment != XAlignment.LeftTop)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    if (rows[i].Width < _childRect.Width)
                    {
                        var c = rows[i];
                        c.AlignText(_childRect, TextAlignment);
                        c.Chars.ForEach(n =>
                        {
                            charItems[n.Index] = n;
                        });
                        rows[i] = c;
                    }
                }
            }
        }

        protected override void OnLayout()
        {
            var prePoint = _childRect.Point;
            _childRect.Move(ContentRect, TextAlignment);
            var point = _childRect.Point;
            var x = point.X - prePoint.X;
            var y = point.Y - prePoint.Y;
            drawChars.Clear();
            TranslationChars(x, y);
            
        }

        protected virtual void TranslationChars(int x, int y)
        {
            foreach (var item in drawChars)
            {
                item.Value.Clear();
            }
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                row.Translation(x, y);
                row.Chars.ForEach(n =>
                {
                    if (n.Index < charItems.Count)
                    {
                        charItems[n.Index] = n;
                    }
                    var fontHashCode = n.Font.FontHasCode();
                    if (!drawChars.ContainsKey(fontHashCode))
                    {
                        drawChars.Add(fontHashCode, new List<XChar>());
                    }
                    drawChars[fontHashCode].Add(n);
                });
                rows[i] = row;
            }
        }

        public override void Translation(int x, int y)
        {
            base.Translation(x, y);
            _childRect.Translation(x, y);
            TranslationChars(x, y);
        }

        protected virtual void OnTextChanged(string text)
        {
            EventParams.Event(XEventType.TextChanged)?.Invoke(this, new XEventInfo() { Value = text });
        }

        protected virtual void AddSuffix()
        {
            var row = rows.Last();
            if (suffixCharItems?.Length > 0 && row.Chars.Count>0)
            {
                var suffixWidth = suffixCharItems.Sum(n=>n.Width);
                var removeItems = new List<XChar>();
                var width = 0;
                var left = 0;
                var top = 0;
                for (int i = row.Chars.Count - 1; i >= 0; i--)
                {
                    if (width >= suffixWidth) break;
                    var c = row.Chars[i];
                    width += c.Width;
                    removeItems.Add(c);
                    left = c.X;
                    top = c.Y;
                }
                removeItems.ForEach(n => row.Chars.Remove(n));

                for (int i = 0; i < suffixCharItems.Length; i++)
                {
                    suffixCharItems[i].Index = row.Chars.Count;
                    suffixCharItems[i].RowIndex = row.Index;
                    suffixCharItems[i].X = left;
                    suffixCharItems[i].Y = top;
                    left += suffixCharItems[i].Width;
                    row.Chars.Add(suffixCharItems[i]);
                }
            }
        }

        internal List<XChar> GetChars(int start, int end)
        {
            var list = new List<XChar>();
            if (start < end && start >= 0 && end < charItems.Count)
            {
                return charItems.ToList().GetRange(start, end - start + 1);
            }
            return null;
        }

        protected override void DrawContent()
        {

            foreach (var item in drawChars)
            {
                RenderImp.DrawText(item.Value);
            }
        }
    }
}
