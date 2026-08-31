using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.models;
using XcyUI.utils;

namespace XcyUI.views
{
    public struct XChar
    {
        public int Index { get; set; }
        public int RowIndex { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public char Value { get; set; }
        public XFont Font { get; set; }
        public bool IsNewLine { get; set; }
        public bool IsSelected { get; set; }
        public XRect RenderRect => new XRect(X, Y, Width, Height);

        //public XChar(int index,char value,XFont font)
        //{
        //    Index = index;
        //    RowIndex = 0;
        //    Value = value;
        //    Font = font;
        //    X = 0;
        //    Y = 0;
        //    Width = 0;
        //    Height = 0;
        //    IsNewLine = false;
        //    IsSelected = false;
        //}

        public void Update(int? index = null, int? rowIndex = null, char? value = null, XFont font = null, int? x = null, int? y = null, int? width = null, int? height = null, bool? isNewLine = null, bool? isSelected = null)
        {
            Index = index ?? Index;
            RowIndex = rowIndex ?? RowIndex;
            Value = value ?? Value;
            Font = font ?? Font;
            X = x ?? X;
            Y = y ?? Y;
            Width = width ?? Width;
            Height = height ?? Height;
            IsNewLine = isNewLine ?? IsNewLine;
            IsSelected = isSelected ?? IsSelected;
        }

        public void Measure()
        {
            var rect = RenderImp.MeasureText(Value, Font);
            Width = rect.Width;
            Height = rect.Height;
        }
        public void Translation(int x, int y)
        {
            X += x;
            Y += y;
        }
    }

    public struct XTextRow
    {
        public int Index { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<XChar> Chars { get; set; }
        public XRect RenderRect => new XRect(X, Y, Width, Height);
        public XTextRow(int index)
        {
            Index = index;
            Chars = new List<XChar>();
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }

        public void Update(int? index = null,List<XChar> chars = null, int? x = null, int? y = null, int? width = null, int? height = null)
        {
            Index = index ?? Index;
            Chars = chars ?? Chars;
            X = x ?? X;
            Y = y ?? Y;
            Width = width ?? Width;
            Height = height ?? Height;
        }

        public void Add(XChar xchar)
        {
            xchar.RowIndex = Index;
            Chars.Add(xchar);
        }

        public void Mesurse()
        {
            if (Width > 0 || Chars.Count == 0) return;
            X = Chars[0].X;
            Y = Chars[0].Y;
            var right = Chars.Last().RenderRect.Right;
            Width = right - X;
            for (int i = 0; i < Chars.Count; i++)
            {
                var c = Chars[i];
                c.Translation(0, Height - c.Font.LineHeight);
                Chars[i] = c;
            }
        }

        public void AlignText(XRect contentRect, XAlignment alignment)
        {
            var rect = RenderRect;
            var y = rect.Y;
            rect.Move(contentRect, alignment);
            var diffX = rect.X - X;
             Translation(rect.X - X, 0);
        }
        public void Translation(int x, int y)
        {
            X += x;
            Y += y;
            for (int i = 0; i < Chars.Count; i++)
            {
                var c = Chars[i];
                c.Translation(x, y);
                Chars[i] = c;
            }
        }

        public void AddSuffix(string suffix)
        {
            
        }

        public static int FindRowIndex(List<XTextRow> rows, XPoint point,int rowspace)
        {
            var rowRects = rows.Select(n => n.RenderRect).ToList();
            for (int i = 0; i < rowRects.Count; i++)
            {
                var rect = rowRects[i];
                rect.Scale(rowspace);
                if (i == 0)
                {
                    rect.Y = 0;
                    rect.Height = rowRects[i].Bottom;
                }
                else if (i == rowRects.Count - 1)
                {
                    rect.Height = int.MaxValue;
                }
                rect.X = 0;
                rect.Width = int.MaxValue;
                rowRects[i] = rect;
            }
            var index = rowRects.FindIndex(n => n.Contain(point));
            if (index < 0)
            {
                index = rows.Count - 1;
            }
            return index;
        }

        public int FindIndex(XPoint point,int colspace)
        {
            var item = Chars.First();
            for (int i = 0; i < Chars.Count; i++)
            {
                var rect = Chars[i].RenderRect;
                rect.Scale(colspace);
                rect.Y = 0;
                if (i == 0)
                {
                    rect.X = 0;
                }
                else if ((i == Chars.Count - 1 && !Chars[Chars.Count-1].IsNewLine) || (Chars[Chars.Count - 1].IsNewLine && i == Chars.Count -2))
                {
                    rect.Width = short.MaxValue;
                }
                rect.Height = short.MaxValue;
                if (rect.Contain(point))
                {
                    item = Chars[i];
                    break;
                }
            }
            var isLeft = (point.X - item.RenderRect.X) < (item.RenderRect.Right - point.X);
            return isLeft ? item.Index : item.Index+1;
        }

        public void Draw()
        {
            RenderImp.DrawText(Chars);
        }
    }

    public struct XTextSpan
    {
        public static readonly XTextSpan Empty = new XTextSpan();
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public XFont Font { get; set; }
        public Action<string> Function { get; set; }
    }
}
