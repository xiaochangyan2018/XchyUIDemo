using System;
using System.Collections.Generic;
using XcyUI.utils;

namespace XcyUI.widgets
{
    public class XState<T>
    {
        private T _value;
        private LinkedList<Action<T>> observers = new LinkedList<Action<T>>();
        private List<Action> disposeObservers = new List<Action>();
        private object _lock = new object();
        public XState() { }
        public XState(T defaultValue)
        {
            _value = defaultValue;
        }

        internal void DefaultValue(T defaultValue)
        {
            _value = defaultValue;
        }
        public void Refresh()
        {
            RenderImp.PostToQueue(() =>
            {
                NotifyChanged();
            });
        }
        public void Send(T value)
        {
            _value = value;
            RenderImp.PostToQueue(() =>
            {
                NotifyChanged();
            });
        }
        internal void SetDefault(T value)
        {
            _value = value;
        }

        public void Post(T value)
        {
            if ((value == null && _value != null) || !value.Equals(_value))
            {
                _value = value;
                RenderImp.PostToQueue(NotifyChanged);
            }
        }
        public T Value
        {
            get => _value;
            set
            {
                RenderImp.Post(() =>
                {
                    if ((value == null && _value != null) || !value.Equals(_value))
                    {
                        _value = value;
                        NotifyChanged();
                    }
                });
            }
        }

        private void NotifyChanged()
        {
            var list = new List<Action<T>>(observers);
            foreach (var item in list)
            {
                item.Invoke(_value);
            }
        }

        public void Add(Action<T> function)
        {
            
            RenderImp.Post(() =>
            {
                if (!observers.Contains(function))
                {
                    observers.AddLast(function);
                }
            });
        }

        public void Remove(Action<T> function)
        {
            RenderImp.Post(() =>
            {
                observers.Remove(function);
            });
        }

        public void AddDispose(Action function)
        {
            RenderImp.Post(() =>
            {
                disposeObservers.Add(function);
            });
        }

        public void Dispose()
        {
            disposeObservers.ForEach(n => n.Invoke());
            //Clear();
            //observers = null;
            //_value = default;
        }

        internal void Clear()
        {
            observers.Clear();
        }

        public int Count { get => observers.Count; }
        
        public XState<T> Join<U>(XState<U> state)
        {
            lock (_lock)
            {
                Action<U> observer = value =>
                {
                    NotifyChanged();
                };
                state.Add(observer);
                AddDispose(() =>
                {
                    state.Remove(observer);
                });
                return this;
            }
        }

        public bool HasObservers()
        {
            return observers != null && observers.Count > 0;
        }
    }
}
