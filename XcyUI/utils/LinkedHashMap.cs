using System;
using System.Collections.Generic;
using System.Linq;

namespace XcyUI.utils
{
    public class LinkedHashMap<T,U>
    {
        private Dictionary<T, U> map;
        private readonly LinkedList<T> linked;
        public LinkedList<T> Keys { get => linked; }
        public int CacheNum { get; set; }
        public Action<U> OnRemoved { get; set; }
        public LinkedHashMap()
        {
            CacheNum = 1000;
            map = new Dictionary<T, U>();
            linked = new LinkedList<T>();
        }

        public LinkedHashMap(int cacheNum)
        {
            CacheNum = cacheNum;
            map = new Dictionary<T, U>();
            linked = new LinkedList<T>();
        }

        public void SetCacheNum(int num)
        {
            CacheNum = num;
            RemoveFirsts();
        }

        public bool ContainsKey(T key)
        {
            return map.ContainsKey(key);
        }

        public Dictionary<T, U> Map => map;

        public List<U> Values()
        {
            return map.Values.ToList();
        }

        public void Add(T key,U value)
        {
            if (map.ContainsKey(key))
            {
                map[key] = value;
                linked.Remove(key);
                
            }
            else
            {
                map.Add(key, value);
            }
            linked.AddLast(key);
            if(linked.Count > CacheNum)
            {
                RemoveFirsts();
            }
        }
        public void Remove(T key)
        {
            if (map.ContainsKey(key))
            {
                OnRemoved?.Invoke(map[key]);
                map.Remove(key);
                linked.Remove(key);

            }
        }

        public U this[T key]
        {
            get
            {
                lock (this)
                {
                    if (map.ContainsKey(key))
                    {
                        linked.Remove(key);
                        linked.AddLast(key);
                        return map[key];
                    }
                    return default;
                }
            }
            set
            {
                Add(key, value);
            }
        } 

        private void RemoveFirsts()
        {
            while (linked.Count > CacheNum)
            {
                T key = linked.First.Value;
                OnRemoved?.Invoke(map[key]);
                map.Remove(key);
                linked.RemoveFirst();
            }
        }

        public void Clear()
        {
            Values().ForEach(n => OnRemoved?.Invoke(n));
            map.Clear();
            linked.Clear();
        }

    }
}
