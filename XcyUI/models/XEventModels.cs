using System;
using System.Collections.Generic;
using XcyUI.events;
using XcyUI.views;
using static XcyUI.models.XFunctions;

namespace XcyUI.models
{
    public enum XEventType
    {
        empty,
        Hover,
        Down,
        Move,
        Leave,
        Cancel,
        Up,
        Wheel,
        Click,
        DoubleClick,
        LongClick,
        Selected,
        KeyPress,
        KeyDown,
        Resize,
        Drag,
        DrawUnder,
        DrawOver,
        Draw,
        LocationChanged,
        LayoutStart,
        LayoutEnd,
        FirstLayout,
        OutAnimation,
        Removed,
        MeasureStart,
        MeasureEnd,
        Focused,
        LossFocused,
        TextChanged,
        Scolled,
        DispatchEvent,
        Dispose
    }

    public class XKeyChar
    {
        public const char LeftDelete = '\b';
    }
    public class XKeyValue
    {
        public const int Space = 32;
        public const int Home = 35;
        public const int End = 36;
        public const int Left = 37;
        public const int Up = 38;
        public const int Right = 39;
        public const int Down = 40;
        public const int Escape = 256;
        public const int Tab = 258;
        public const int Delete = 46;
        public const int Backspace = 259;
        public const int Enter = 257;
    }

    [Flags]
    public enum KeyModify
    {
        None = 0,
        Shift = 0x0001,
        Control = 0x0002,
        Alt = 0x0004,
        Super = 0x0008
    }

    public class XEventInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsLeft { get; set; }
        public XEventType EventType { get; set; }
        public int ClickKey { get; set; }
        public int WheelSize { get; set; }
        public bool IsVerticalWheel { get; set; }
        public char KeyChar { get; set; }
        public int KeyValue { get; set; }
        public KeyModify KeyModify { get; set; }
        public object Value { get; set; }

        public XPoint Point => new XPoint(X, Y);

        public XEventInfo Copy(XEventType eventType)
        {
            var action = new XEventInfo();
            action.X = X;
            action.Y = Y;
            action.IsLeft = IsLeft;
            action.EventType = eventType;
            action.ClickKey = ClickKey;
            action.WheelSize = WheelSize;
            action.IsVerticalWheel = IsVerticalWheel;
            action.KeyChar = KeyChar;
            action.KeyValue = KeyValue;
            action.KeyModify = KeyModify;
            action.Value = Value;
            return action;
        }
    }

    public class XEventFunction
    {
        public XEventFunction()
        {
            _functions = new Dictionary<string, XFunction<XView, XEventInfo>>();
        }
        public XEventFunction(string key, XFunction<XView, XEventInfo> function)
        {
            _functions = new Dictionary<string, XFunction<XView, XEventInfo>>();
            AddFunction(key, function);
        }
        public float RowScollerSize { get; set; }
        public float ColumnScollerSize { get; set; }
        public bool IsVerticalScoll { get; set; }
        public XPoint DownPoint { get; set; }
        public object Value { get; set; }
        public bool IsMust { get; set; }
        public bool IsIntercept { get; set; }
        private Dictionary<string, XFunction<XView, XEventInfo>> _functions;
        public int FunctionsCount => _functions.Count;
        public void AddFunction(string key, XFunction<XView, XEventInfo> function)
        {
            _functions.Remove(key);
            _functions.Add(key, function);
        }
        public XFunction<XView,XEventInfo> RemoveFunction(string key)
        {
            XFunction<XView, XEventInfo> function = null;
            if (_functions.ContainsKey(key))
            {
                function = _functions[key];
                _functions.Remove(key);
            }
            return function;
        }

        public void Clear()
        {
            _functions?.Clear();
            if (Value is IDisposable)
            {
                ((IDisposable)Value).Dispose();
            }
            Value = null;
        }

        public void Invoke(XView view, XEventInfo info)
        {
            var list = new List<XFunction<XView, XEventInfo>>();
            list.AddRange(_functions.Values);
            foreach (var function in list)
            {
                function?.Invoke(view, info);
            }
        }

        public void Invoke(XView view,string key, XEventInfo info)
        {
            if (_functions.ContainsKey(key))
            {
                _functions[key].Invoke(view, info);
            }
        }
    }

    public enum XDragType
    {
        All,
        Vertical,
        Horizontal
    }
}
