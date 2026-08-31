using System;
using System.Collections.Generic;
using XcyUI.views;

namespace XcyUI.models
{
    public class XEventParams
    {
        public bool Focusable;
        public bool Enable = true;
        private Dictionary<XEventType, XEventFunction> _events;
        public Dictionary<XEventType, XEventFunction> Events => _events ??= new();

        public XEventFunction EventOrCreate(XEventType eventType)
        {
            if(Events.TryGetValue(eventType, out XEventFunction function) == false)
            {
                function = new XEventFunction();
                Events[eventType] = function;
            }
            return function;
        }

        public XEventFunction Event(XEventType eventType)
        {
            if (_events?.TryGetValue(eventType, out XEventFunction function) == true)
            {
                return function;
            }
            return null;
        }

        public bool Contains(XEventType eventType)
        {
            return _events?.ContainsKey(eventType) == true;
        }

        public XEventFunction Remove(XEventType eventType)
        {
            if (_events?.TryGetValue(eventType, out XEventFunction function) == true)
            {
                Events.Remove(eventType);
                return function;
            }
            return null;
        }

        public Action<XView, XEventInfo> RemoveFunction(XEventType eventType, string key)
        {
            return Event(eventType)?.RemoveFunction(key);
        }

        public void Clear()
        {
            if (_events != null)
            {
                foreach (var key in _events.Keys)
                {
                    _events[key].Clear();
                }
                _events.Clear();
                _events = null;
            }
        }
    }
}
