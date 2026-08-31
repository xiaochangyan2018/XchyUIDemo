using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.models;
using XcyUI.utils;
using XcyUI.views;
using XcyUI.widgets;

namespace XcyUI.templates
{
    public class XLazyItem
    {
        internal XLazyItem() { }
        internal XLazyItem(int index, XGroup view, object data)
        {
            Index = index;
            View = view;
            Data = data;
        }
        internal int TempleteIndex { get; set; }
        internal int Index { get; set; }
        internal int IndexInGroup { get; set; }
        //internal bool IsLast { get; set; }
        internal XGroup View { get; set; }
        internal object Data { get; set; }
    }

    public class XLazyTemplate
    {
        internal static int GroupItemCount = 50;
        internal int Index { get; set; }
        internal int Height { get; set; }
        internal int Width { get; set; }
        internal int MeausreHeight { get; set; }
        internal int MeausreWidth { get; set; }
        internal int SumHeight { get; set; }
        internal int SumWidth { get; set; }
        internal int Space { get; set; }
        internal bool IsNotifyChanged { get; set; }
        protected Action<XModify, object, int> onViewSetting;
        internal List<object> Datas { get; set; }
        internal Dictionary<int, int> CacheSize { get; private set; }
        internal Dictionary<int, int> GroupSize { get; private set; }
        internal LinkedHashMap<int, XLazyItem> FirstCache { get; private set; }
        internal LinkedHashMap<int, XLazyItem> SecondCache { get; private set; }

        internal Action<XGroup, object,int> Content { get; set; }
        internal XLazyTemplate()
        {
            Datas = new List<object>();
            FirstCache = new LinkedHashMap<int, XLazyItem>();
            FirstCache.SetCacheNum(100);
            SecondCache = new LinkedHashMap<int, XLazyItem>();
            SecondCache.SetCacheNum(100);
            CacheSize = new Dictionary<int, int>();
            GroupSize = new Dictionary<int, int>();
            IsNotifyChanged = true;
        }

        internal void Clear()
        {
            CacheSize.Clear();
            foreach (var item in FirstCache.Values())
            {
                item.View.Dispose();
            }
            foreach (var item in SecondCache.Values())
            {
                item.View.Dispose();
            }
            FirstCache.Clear();
            SecondCache.Clear();
        }

        internal virtual void MeasureDynamic(XLazy lazy) { }
        internal virtual void MeasureFixed(XLazy lazy) { }

        internal virtual XLazyItem LayoutItem(XLazy lazy, int index, int top,int left) { return null; }

        internal void UpdateDatas<T>(List<T> datas)
        {
            // Cache.Clear();
            Datas.Clear();
            datas.ForEach(n => Datas.Add(n));
        }

        internal int ItemHeightAt(int index)
        {
            return CacheSize.ContainsKey(index) ? CacheSize[index] : Height;
        }

        internal int ItemWidthAt(int index)
        {
            return CacheSize.ContainsKey(index) ? CacheSize[index] : Width;
        }

        internal XLazyItem ItemAt(XLazy lazy, int index)
        {
            var isNotifyChanged = IsNotifyChanged;
            if (!FirstCache.ContainsKey(index))
            {
                if (SecondCache.ContainsKey(index))
                {
                    var item = SecondCache[index];
                    SecondCache.Remove(index);
                    FirstCache[index] = item;
                    item.Data = Datas[index];
                }
                else if (SecondCache.Keys.Count > 0)
                {
                    var time = DateTime.Now.Ticks / 10000;
                    var itemKey = SecondCache.Keys.Last();
                    var item = SecondCache[itemKey];
                    isNotifyChanged = true;
                    SecondCache.Remove(itemKey);
                    FirstCache[index] = item;
                }
                else
                {
                    var item = CreateItem(lazy, index);
                    FirstCache[index] = item;
                }
            }
            if (isNotifyChanged)
            {
                var item = FirstCache[index];
                item.Data = Datas[index];
                item.Index = index;
                RefreshContent(item);
                item.View.Measure();
            }
            return FirstCache[index];
        }

        public void RefreshContent(XLazyItem item)
        {
            onViewSetting?.Invoke(new XModify(item.View), item.Data, item.Index);
            Content?.Invoke(item.View, item.Data, item.Index);
        }

        public void OnViewSetting<T>(Action<XModify,T> function)
        {
            this.onViewSetting = (setter, t, index) =>
            {
                function.Invoke(setter, (T)t);
            };
        }

        public void OnViewSetting<T>(Action<XModify, T, int> function)
        {
            this.onViewSetting = (setter, t, Index) =>
            {
                function.Invoke(setter, (T)t, Index);
            };
        }

        internal virtual XLazyItem CreateItem(XLazy lazy, int index)
        {
            var box = new XBox();
            box.Key = index.ToString();
            box.Parent = lazy;
            box.ContentAlignment = XAlignment.LeftTop;
            box.LayoutParams.Width = MeausreWidth;
            box.LayoutParams.Height = MeausreHeight;
            var item = new XLazyItem(index, box, Datas[index]);
            item.TempleteIndex = Index;
            RefreshContent(item);
            item.View.EnableCache(true, XCacheType.Pictrue);
            item.View.Measure();
            CacheSize[index] = item.View.Height;
            return item;
        }

        internal int GetGroupHeight(int groupIndex)
        {
            if(Height < 0)
            {
                return 0;
            }
            if (!GroupSize.ContainsKey(groupIndex))
            {
                GroupSize.Add(groupIndex, GroupItemCount * Height);
            }
            return GroupSize[groupIndex];
        }

        internal int GetGroupWidth(int groupIndex)
        {
            if (Width < 0)
            {
                return 0;
            }
            if (!GroupSize.ContainsKey(groupIndex))
            {
                GroupSize.Add(groupIndex, GroupItemCount * Width);
            }
            return GroupSize[groupIndex];
        }
    }
}
