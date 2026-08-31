using System;
using System.Collections.Generic;
using XcyUI.views;

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
        public const int Home = 35;
        public const int End = 36;
        public const int Left = 37;
        public const int Up = 38;
        public const int Right = 39;
        public const int Down = 40;
        public const int Delete = 46;
        public const int Backspace = 259;
        public const int Enter = 257;
        public const int Tab = 258;
        public const int Escape = 256;

    }

    public enum KeyModify
    {
        Shift = 0x0001,
        Control = 0x0002,
        Alt = 0x0004,
        Super = 0x0008
    }

    public struct XEventInfo
    {
        public static readonly XEventInfo Empty;
        public int X;
        public int Y;
        public bool IsLeft;
        public XEventType EventType;
        public int ClickKey;
        public int WheelSize;
        public bool IsVerticalWheel;
        public char KeyChar;
        public int KeyValue;
        public KeyModify KeyModify;
        public object Value;
        public bool isAbandoned;

        public XPoint Point => new XPoint(X, Y);

        public XEventInfo Copy(XEventType eventType)
        {
            var action = this;
            action.EventType = eventType;
            return action;
        }
    }

    public class XEventFunction
    {
        public object Value;
        public bool IsMust;
        public bool IsIntercept;
        private bool _dirty = true;
        private Dictionary<string, Action<XView, XEventInfo>> _functions = new();
        private List<Action<XView, XEventInfo>> _cache = new();
        public int FunctionsCount => _functions.Count;
        public void AddFunction(string key, Action<XView, XEventInfo> function)
        {
            _functions[key] = function;
            _dirty = true;
        }
        public Action<XView,XEventInfo> RemoveFunction(string key)
        {
            if (_functions.TryGetValue(key, out Action<XView, XEventInfo> function))
            {
                _functions.Remove(key);
                _dirty = true;
            }
            return function;
        }

        public void Clear()
        {
            _functions?.Clear();
            _dirty = true;
            if (Value is IDisposable)
            {
                ((IDisposable)Value).Dispose();
            }
            Value = null;
        }
        private void RebuildCache()
        {
            if (_dirty)
            {
                _cache.Clear();
                _cache.AddRange(_functions.Values);
                _dirty = false;
            }
        }

        public void Invoke(XView view, XEventInfo info)
        {
            RebuildCache();
            foreach (var function in _cache)
            {
                function?.Invoke(view, info);
            }
        }

        public void Invoke(XView view,string key, XEventInfo info)
        {
            if (_functions.TryGetValue(key,out Action<XView, XEventInfo> function))
            {
                function.Invoke(view, info);
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
