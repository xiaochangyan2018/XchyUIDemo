using System;
using System.Collections.Generic;
using XcyUI.models;
using XcyUI.utils;
using XcyUI.animation;
using System.Threading;
using XcyUI.theme;
using XcyUI.events;
using XcyUI.expansions;

namespace XcyUI.views
{
    public class XInput: XText
    {
        public bool ReadOnly { get; set; }
        public bool Focused { get; set; }
        public XStyle SelectedStyle { get; set; }
        public XStyle CurorStyle { get; set; }
        public string Hint { get; set; }

        protected int curorIndex;
        protected int precurorIndex;
        protected int selectedIndex;
        protected int selectedLength;
        protected bool isShowCursor = true;
        protected XAnimate animationItem;
        protected List<XTextRow> hintRows;
        protected Timer timer;
        protected int curorAimationTimes = 0;
        internal char? PasswordChar { get; set; }

        private XRect curorRect
        {
            get
            {
                var rect = new XRect(ContentRect.X, ContentRect.Y, 2, Font.LineHeight);
                if (charItems?.Count > 0)
                {
                    var isLast = curorIndex >= charItems.Count;
                    var isFirst = curorIndex < 0;
                    var index = isFirst ? 0 : isLast ? charItems.Count - 1 : curorIndex;
                    rect.X = charItems[index].RenderRect.X - ColumnSpace / 2;
                    rect.Y = charItems[index].Y;
                    if (isLast)
                    {
                        rect.X = charItems[index].RenderRect.RightTop.X;
                    }
                }
                else
                {
                    rect.Move(ContentRect, TextAlignment);
                }
                return rect;
            }
        }

        public XInput()
        {
            hintRows = new List<XTextRow>();
            EventParams.Focusable = true;
            Lines = 1;
            IsLayoutAllItem = true;            
            CurorStyle = new XStyle();
            CurorStyle.Background = new XBrush() { StartColor = XColors.Black };
            SelectedStyle = new XStyle();
            SelectedStyle.Background = new XBrush() { StartColor = XTheme.Color.PrimaryLight3.Copy(0.9f) };
            EventParams.EventOrCreate(XEventType.Down);
            EventParams.EventOrCreate(XEventType.KeyPress);
            EventParams.EventOrCreate(XEventType.Focused);
            EventParams.EventOrCreate(XEventType.LossFocused);
            EventParams.EventOrCreate(XEventType.Move);
            EventParams.EventOrCreate(XEventType.DoubleClick);
        }

        internal void SetPasswordChar(char? key)
        {
            PasswordChar = key;
            charItems.Clear();
        }

        protected override void OnMeasure()
        {
            var tempText = _text;
            if (!string.IsNullOrEmpty(_text) && PasswordChar != null)
            {
                _text = new string(PasswordChar.Value, _text.Length);
            }
            base.OnMeasure();
            _text = tempText;
            if (selectedLength != 0)
            {
                Select(selectedIndex, selectedLength);
            }

            if (!string.IsNullOrEmpty(Hint))
            {
                var font = Font.Copy();
                font.Color = font.Color.Copy(XTheme.Color.PlaceholderText);
                MesaureText(Hint,font, hintRows, new List<XChar>());
            }
            if(curorIndex > Text.Length)
            {
                curorIndex = 0;
            }
        }

        public string GetSelectText()
        {
            if(selectedIndex>=0 && selectedLength > 0 && !string.IsNullOrEmpty(Text))
            {
                return Text.Substring(selectedIndex, selectedLength);
            }
            return "";
        }

        protected override void AddSuffix()
        {
        }

        protected override void TranslationChars(int x, int y)
        {
            base.TranslationChars(x, y);
            for(int i = 0; i < hintRows.Count; i++)
            {
                var row = hintRows[i];
                row.Translation(x, y);
                hintRows[i] = row;
            }
        }

        private void HandleCuror(object state)
        {
            if(curorAimationTimes >= 10)
            {
                StopCursorAnimation();
                isShowCursor = true;
                Invalidate();
                return;
            }
            isShowCursor = !isShowCursor;
            Invalidate();
            curorAimationTimes += 1;
        }

        private void StartCursorAnimation()
        {
            timer = new Timer(new TimerCallback(HandleCuror), 0, 530, 530);
            curorAimationTimes = 0;
        }

        private void StopCursorAnimation()
        {
            if (timer != null)
            {
                timer.Change(-1, -1);
                timer.Dispose();
                timer = null;
            }
        }

        private void ShowCursor()
        {
            if (ReadOnly) return;
            isShowCursor = true;
            Invalidate();
            StopCursorAnimation();
            StartCursorAnimation();
        }

        protected void SetCurcorIndex(XPoint point)
        {
            if (rows.Count > 0)
            {
                var rowIndex = XTextRow.FindRowIndex(rows, point, RowSpace / 2);
                curorIndex = rows[rowIndex].FindIndex(point, ColumnSpace / 2);                
                Invalidate();
            }
        }

        private void HorizontalNextCurorIndex(bool isLeft)
        {
            if(curorIndex>=0 && curorIndex <= charItems.Count)
            {
                curorIndex = isLeft ? curorIndex - 1 : curorIndex + 1;
                curorIndex = curorIndex < 0 ? 0 : curorIndex > charItems.Count ? charItems.Count : curorIndex;

                // 处理换行符
                if (curorIndex< charItems.Count && charItems[curorIndex].IsNewLine && charItems[curorIndex].Value == '\n')
                {
                    if (isLeft) curorIndex -= 1;
                    else curorIndex += 1;
                }

                MoveCurorPosition();
            }
        }

        private void VerticalNextCurorIndex(bool isTop)
        {
            var rect = curorRect;
            rect.Translation(0, isTop ? -curorRect.Height : curorRect.Height);
            SetCurcorIndex(rect.Center);
            MoveCurorPosition();
        }

        private void MoveCurorPosition()
        {
            if (ReadOnly) return;
            var curorRectTemp = curorRect;
            var contentRect = ContentRect;
            var leftDist = curorRectTemp.Left - contentRect.Left;
            var topDist = curorRectTemp.Top - contentRect.Top;
            var rightDist = curorRectTemp.Right - contentRect.Right;
            var bottomDist = curorRectTemp.Bottom - contentRect.Bottom;
            if (leftDist < 0)
            {
                TranslationChars(-leftDist, 0);
            }
            else if (topDist < 0)
            {
                TranslationChars(0, -topDist);
            }
            else if (rightDist > 0)
            {
                TranslationChars(-rightDist, 0);
            }
            else if (bottomDist > 0)
            {
                TranslationChars(0, -bottomDist);
            }
            ChangeImmPosition();
            Invalidate();
        }

        private bool RemoveSelected()
        {
            if (selectedLength > 0)
            {
                Remove(selectedIndex, selectedLength);
                curorIndex = selectedIndex;
                Select(0, 0);
                return true;
            }
            return false;
        }
        private void delete(bool isLeft)
        {
            if (RemoveSelected())
            {
                return;
            }
            var isDelete = isLeft && curorIndex > 0 || !isLeft && curorIndex < charItems.Count;
            if (isLeft)
            {
                HorizontalNextCurorIndex(isLeft);
            }
            if (isDelete)
            {
                // 处理换行符
                var length = 1;
                if (curorIndex+1 < charItems.Count && charItems[curorIndex+1].IsNewLine)
                {
                    length = 2;
                }

                Remove(curorIndex, length);
            }
        }

        internal void Remove(int index,int length)
        {            
            Text = Text.Remove(index, length);
            if ((LayoutParams.IsWrapHeight && Lines == 0) || LayoutParams.IsWrapWidth)
            {
                (Parent ?? this).StartLayout();
            }
            else
            {
                StartLayout();
            }
        }

        internal void Add(int index,string text)
        {
            var tempText = _text;
            Text = Text.Insert(index, text);
            if (_text == tempText) return;
            curorIndex += text.Length;
            if (LayoutParams.IsWrapHeight || LayoutParams.IsWrapWidth)
            {
                (Parent ?? this).StartLayout();
            }
            else
            {
                StartLayout();
            }
        }

        public void Select(int index, int length)
        {
            selectedIndex = index;
            selectedLength = length;
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                for (int y = 0; y < rows[i].Chars.Count; y++)
                {
                    var charItem = row.Chars[y];
                    charItem.IsSelected = charItem.Index >= index && charItem.Index < (index + length);
                    if (charItem.Index < charItems.Count)
                    {
                        charItems[charItem.Index] = charItem;
                    }
                    row.Chars[y] = charItem;
                }
                rows[i] = row;
            }
            Invalidate();
        }

        public void Focus(bool focus,bool isSelect = false)
        {
            if (isSelect)
            {
                Select(0, Text?.Length ?? 0);
            }
            XEvent.FocusView = this;
            curorIndex = Text.Length;
            Focused = focus;
            isShowCursor = focus;
            ChangeImmPosition();
            Invalidate();
        }

        protected void OnFocusChanged(bool focues)
        {
            if(!focues)
            {
                Select(0, 0);
            }
            if (ReadOnly) return;
            Focused = focues;
            if (!focues)
            {
                Select(0, 0);
            }
        }

        protected void OnMove(XPoint point)
        {
            SetCurcorIndex(point);
            var start = Math.Min(precurorIndex, curorIndex);
            var end = Math.Max(precurorIndex, curorIndex);
            Select(start, end - start);
        }

        private void OnKeyPress(XEventInfo info)
        {
            if (ReadOnly || info.isAbandoned) return;
            switch (info.KeyChar)
            {
                case XKeyChar.LeftDelete:                    
                    delete(true);
                    break;
                default:
                    if (info.KeyChar > 0)
                    {
                        RemoveSelected();
                        Add(curorIndex, info.KeyChar.ToString());
                    }
                    break;
            }
            switch (info.KeyValue)
            {
                case  XKeyValue.Left:
                    HorizontalNextCurorIndex(true);
                    break;
                case XKeyValue.Right:
                    HorizontalNextCurorIndex(false);
                    break;
                case XKeyValue.Up:
                    VerticalNextCurorIndex(true);
                    break;
                case XKeyValue.Down:
                    VerticalNextCurorIndex(false);
                    break;
                case XKeyValue.Delete:
                    delete(false);
                    break;
                case XKeyValue.Backspace:
                    delete(true);
                    break;
                case XKeyValue.Enter:
                    if (Lines != 1)
                    {
                        Add(curorIndex, "\r\n");
                    }
                    break;
            }
            Invalidate();
        }

        public override void OnEvent(XEventInfo info)
        {
            
            switch (info.EventType)
            {
                case XEventType.Down:
                    SetCurcorIndex(info.Point);
                    precurorIndex = curorIndex;
                    if (info.IsLeft) Select(0, 0);
                    ShowCursor();
                    break;
                case XEventType.KeyPress:
                    if (!ReadOnly)
                    {
                        OnKeyPress(info);
                        ShowCursor();
                    }
                    break;
                case XEventType.Focused:
                    OnFocusChanged(true);
                    break;
                case XEventType.LossFocused:
                    OnFocusChanged(false);
                    break;
                case XEventType.DoubleClick:
                    if (info.IsLeft) Select(0, Text.Length);
                    ShowCursor();
                    break;
                case XEventType.Move:
                    OnMove(info.Point);                    
                    break;
            }
            MoveCurorPosition();
        }

        protected override void DrawContent()
        {
            DoDrawContent();
        }

        private void DoDrawContent()
        {
            rows.ForEach(n =>
            {
                n.Chars.ForEach(c =>
                {
                    if (!c.IsNewLine && c.IsSelected)
                    {
                        var rect = c.RenderRect;
                        rect.Scale(ColumnSpace / 2);
                        rect.Y = rows[c.RowIndex].Y;
                        rect.Height = n.Height;
                        RenderImp.DrawRect(rect, SelectedStyle);
                    }
                });
            });
            base.DrawContent();

            if (drawChars.Count == 0 && hintRows.Count != 0)
            {
                hintRows.ForEach(n => n.Draw());
            }

            if (!ReadOnly && Focused && isShowCursor)
            {
                RenderImp.DrawRect(curorRect, CurorStyle);
            }
        }

        public void ChangeImmPosition()
        {
            if (Focused && !ReadOnly)
            {
                RenderImp.ChangedImmPosition(curorRect.Point);
            }
        }
    }
}
