using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using XcyUI.animation;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.templates;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.views;
using XcyUI.widgets.extensions;

namespace XcyUI.widgets
{
    public class XCompose
    {
        private XView currentParent;
        private XView currentView;
        private WidgetState widgetState;
        private int index = 0;
        private static XCompose widget = new XCompose();
        private readonly static object _lockObj = new object();
        public static int FILL = XLayoutParams.Fill;
        public static int WRAP = XLayoutParams.Wrap;
        private static Dictionary<string, object> stateValues = new Dictionary<string, object>();
        private static Dictionary<int, XView> popupCards = new Dictionary<int, XView>();
        public static XState<bool> HotReload { get; private set; }
        public static bool EnableHotReload = Debugger.IsAttached;
        internal static bool isHotReload = false;
        public static string POPCARD_PRE_KEY = "PopCard";


        /// <summary>
        /// 根组合函数
        /// </summary>
        /// <param name="content">内容函数</param>
        /// <param name="key">标识key</param>
        /// <returns></returns>
        public static XModify ContentView(Action content, [CallerLineNumber] int key = 0)
        {
            lock (_lockObj)
            {
                widget.widgetState = WidgetState.Create;
                widget.index = 0;
                widget.currentParent = null;
                var modify = widget.View<XBox>(key);
                widget.currentParent = modify.View;
                SwitchParent(modify.View, content);
                widget.widgetState = WidgetState.Update;
                Action<bool> observer = darkMode =>
                {
                    widget.currentParent = modify.View;
                    widget.index = 0;
                    content.Invoke();
                    RefreshLazyView(modify.View);
                    modify.FadeIn(duration: 500, startValue: 0.45f);
                    RenderImp.Invalidate();
                };
                XTheme.DarkModeState.Add(observer);
                modify.View.AddEvent(XEventType.Dispose, "darkMode_observer", () => XTheme.DarkModeState.Remove(observer));
                if (EnableHotReload)
                {
                    if (HotReload == null)
                    {
                        HotReload = new XState<bool>();
                    }
                    Action<bool> hotReloadobserver = darkMode =>
                    {
                        isHotReload = true;
                        widget.currentParent = modify.View;
                        widget.index = 0;
                        content.Invoke();
                        modify.View.StartLayout();
                        RenderImp.Invalidate();
                        isHotReload = false;
                    };
                    HotReload.Add(hotReloadobserver);
                    modify.View.AddEvent(XEventType.Dispose, "hotReload_observer", () => XTheme.DarkModeState.Remove(observer));
                }

                return modify;
            }
        }

        private static void RefreshLazyView(XView view)
        {
            view.DrawCache.IsRefreshCache = true;
            if (view is XLazy)
            {
                ((XLazy)view).StartLayout();
            }
            for (int i = 0; i < view.ChildCount(); i++)
            {
                RefreshLazyView(view.ChildElemnt(i));
            }
        }

        public static XModify Box(Action content = null, [CallerLineNumber] int key = 0)
        {
            return widget
                .View<XBox>(key)
                .Size(FILL)
                .ContentAlignment(XAlignment.Center)
                .Also(n => SwitchParent(n.View, content));
        }

        public static XModify Box<T>(XState<T> state, Action<T> content, bool needLayout = true, [CallerLineNumber] int key = 0)
        {
            return BindGroup<XBox, T>(state, content, needLayout, key)
                .Size(FILL)
                .ContentAlignment(XAlignment.Center);
        }

        public static XModify Column(Action content = null, [CallerLineNumber] int key = 0)
        {
            return widget
                .View<XColumn>(key)
                .HorizontalAlignment(XHorizontalAlignment.Center)
                .VerticalAlignment(XVerticalAlignment.Top)
                .Size(FILL)
                .Also(n => SwitchParent(n.View, content));
        }

        public static XModify Column<T>(XState<T> state, Action<T> content, bool needLayout = true, [CallerLineNumber] int key = 0)
        {
            return BindGroup<XColumn, T>(state, content, needLayout, key)
                .HorizontalAlignment(XHorizontalAlignment.Center)
                .VerticalAlignment(XVerticalAlignment.Top)
                .Size(FILL);
        }

        public static XModify Row(Action content = null, [CallerLineNumber] int key = 0)
        {
            return widget
                .View<XRow>(key)
                .HorizontalAlignment(XHorizontalAlignment.Left)
                .VerticalAlignment(XVerticalAlignment.Center)
                .Size(WRAP)
                .Also(n => SwitchParent(n.View, content));
        }

        public static XModify Row<T>(XState<T> state, Action<T> content, bool needLayout = true, [CallerLineNumber] int key = 0)
        {
            return BindGroup<XRow, T>(state, content, needLayout, key)
                .HorizontalAlignment(XHorizontalAlignment.Left)
                .VerticalAlignment(XVerticalAlignment.Center)
                .Size(WRAP);
        }

        public static XModify Flow(Action content = null, [CallerLineNumber] int key = 0)
        {
            return widget
                .View<XFlow>(key)
                .Size(FILL)
                .Also(n => SwitchParent(n.View, content));
        }

        public static XModify Flow<T>(XState<T> state, Action<T> content, bool needLayout = true, [CallerLineNumber] int key = 0)
        {
            return BindGroup<XFlow, T>(state, content, needLayout, key).Size(FILL);
        }

        public static XModify Text(string text = null, bool selected = false, [CallerLineNumber] int key = 0)
        {

            var builder = selected ? widget.View<XInput>(key).Clip().TextDefault().ReadOnly(true).Lines(0) : widget.View<XText>(key).TextDefault();
            builder.TextAlignment(XAlignment.LeftTop);
            return builder.Content(text).Also(n =>
            {
                ((XText)n.View).Spans?.Clear();
            });
        }


        public static XModify Text(Action<XModify> content, bool selected = false, [CallerLineNumber] int key = 0)
        {
            var builder = selected ? widget.View<XInput>(key).Clip().TextDefault().ReadOnly(true).Size(WRAP).Lines(0) : widget.View<XText>(key).TextDefault();
            builder.TextAlignment(XAlignment.LeftTop);
            return builder.Also(n =>
            {
                n.AsView<XText>().Text = "";
                n.AsView<XText>().Spans?.Clear();
                SwitchParent(n.View, () => content.Invoke(n));
            });
        }
        public static XModify Text(Action content, [CallerLineNumber] int key = 0)
        {
            return widget.View<XText>(key)
                .Color(XTheme.Color.PrimaryText)
                .FontSize(XTheme.Size.Body)
                .FontWeight(XTheme.Weight.Middle)
                .TextAlignment(XAlignment.LeftTop).Also(n =>
            {
                n.AsView<XText>().Text = "";
                n.AsView<XText>().Spans?.Clear();
                SwitchParent(n.View, content);
            });
        }

        public static XSpanBuilder BreakLine()
        {
            return Span("\r\n");
        }

        public static XSpanBuilder Span(string text)
        {
            if (widget.currentParent is XText)
            {
                var textView = (XText)widget.currentParent;
                return new XSpanBuilder(textView, text);
            }
            return null;
        }

        public static XModify Input(string text = null, [CallerLineNumber] int key = 0)
        {
            return widget.View<XInput>(key)
                .Clip()
                .TextDefault()
                .Content(text)
                .Lines(1)
                .Width(200)
                .CursorColor(XTheme.DarkModeState.Value ? XTheme.Color.White : XTheme.Color.Black)
                .HoverCursor(XCursorType.Input)
                .InterceptEvent(XEventType.Move)
                .Also(n =>
            {
                ((XInput)n.View).Spans?.Clear();
            });
        }


        public static XModify Icon(int resId, [CallerLineNumber] int key = 0)
        {
            return widget.View<XIcon>(key).ScaleType(XScaleType.Normal).ResId(resId).Color(XTheme.Color.PrimaryText).Size(20);
        }

        public static XModify Spacer(int size = 0, [CallerLineNumber] int key = 0)
        {
            return widget.View<XView>(key).Size(size);
        }

        public static XModify LazyColumn(Action content, [CallerLineNumber] int key = 0)
        {
            return widget
                .View<XLazyColumn>(key)
                .Size(FILL)
                .MinHeight(50)
                .Scrollable()
                .Also(n => SwitchParent(n.View, content));
        }

        public static XModify LazyColumn<T>(XState<T> state, Action<T> content, bool needLayout = true, [CallerLineNumber] int key = 0)
        {
            return BindGroup<XLazyColumn, T>(state, content, needLayout, key).Size(FILL).MinHeight(50).Scrollable();
        }

        public static XModify LazyRow(Action content, [CallerLineNumber] int key = 0)
        {
            return widget
                .View<XLazyRow>(key)
                .Scrollable(false)
               .Also(n => SwitchParent(n.View, content));
        }

        public static XModify LazyRow<T>(XState<T> state, Action<T> content, bool needLayout = true, [CallerLineNumber] int key = 0)
        {
            return BindGroup<XLazyRow, T>(state, content, needLayout, key).Scrollable(false);
        }

        public static XModify LazyGrid(int cells, Action content, [CallerLineNumber] int key = 0)
        {
            return widget
                .View<XLazyGrid>(key)
                .FixedCells(cells)
                .Scrollable()
                .Also(n => SwitchParent(n.View, content));

        }

        public static XModify LazyGrid<T>(XState<T> state, int cells, Action<T> content, bool needLayout = true, [CallerLineNumber] int key = 0)
        {
            return BindGroup<XLazyGrid, T>(state, value =>
            {
                ((XLazyGrid)widget.currentParent).Cells = cells;
                content.Invoke(value);
            }, needLayout, key).Scrollable();
        }

        public static XLazyTemplate LazyItem<T>(List<T> datas, int span, bool isAnimate, Action<T, int> content)
        {
            XLazyTemplate template = null;
            ((XLazy)widget.currentParent).IsAnimate = isAnimate;
            if (((XLazy)widget.currentParent).Templates.Count == widget.index)
            {
                if (widget.currentParent is XLazyGrid)
                {
                    var lazy = (XLazyGrid)widget.currentParent;
                    var cells = lazy.Cells / span;
                    template = XLazyGridTemplate.Create(datas, span, cells, (v, data, index) =>
                    {
                        var tempParent = widget.currentParent;
                        var tempCurrentView = widget.currentView;
                        var tempIdnex = widget.index;
                        widget.currentParent = v;
                        widget.currentView = v;
                        widget.index = 0;
                        SwitchParent(v, () => { content(data, index); });
                        widget.currentView = tempParent;
                        widget.currentParent = tempParent;
                        widget.index = tempIdnex;
                    });
                    lazy.Templates.Add(template);
                }
                else if (widget.currentParent is XLazyColumn)
                {
                    var lazy = (XLazyColumn)widget.currentParent;
                    template = XLazyColumnTemplate.Create(datas, (v, data, index) =>
                    {
                        var tempParent = widget.currentParent;
                        var tempCurrentView = widget.currentView;
                        var tempIdnex = widget.index;
                        widget.currentParent = v;
                        widget.currentView = v;
                        widget.index = 0;
                        SwitchParent(v, () => { content(data, index); });
                        widget.currentView = tempParent;
                        widget.currentParent = tempParent;
                        widget.index = tempIdnex;
                    });
                    template.Index = lazy.Templates.Count;
                    lazy.Templates.Add(template);
                }
                else if (widget.currentParent is XLazyRow)
                {
                    var lazy = (XLazyRow)widget.currentParent;
                    template = XLazyRowTemplate.Create(datas, (v, data, index) =>
                    {
                        var tempParent = widget.currentParent;
                        var tempCurrentView = widget.currentView;
                        var tempIdnex = widget.index;
                        widget.currentParent = v;
                        widget.currentView = v;
                        widget.index = 0;
                        SwitchParent(v, () => { content(data, index); });
                        widget.currentView = tempParent;
                        widget.currentParent = tempParent;
                        widget.index = tempIdnex;
                    });
                    lazy.Templates.Add(template);
                }
            }
            else
            {
                var lazy = (XLazy)widget.currentParent;
                template = lazy.Templates[widget.index];
                template.Clear();
                if (!lazy.AnimateInfo.Enable)
                {
                    var animateInfo = lazy.AnimateInfo;
                    animateInfo.Enable = datas.Count != template.Datas.Count;
                    animateInfo.IsAdd = datas.Count > template.Datas.Count;
                    lazy.AnimateInfo = animateInfo;
                }
                template.Datas.Clear();
                datas?.ForEach(n => template.Datas.Add(n));
                template.IsNotifyChanged = true;

            }
            widget.index += 1;
            return template;
        }
        public static XLazyTemplate LazyItem<T>(List<T> datas, bool isAnimate, Action<T> content)
        {
            return LazyItem(datas, 1, isAnimate, (t, i) => content?.Invoke(t));
        }

        public static XLazyTemplate LazyItem<T>(List<T> datas, bool isAnimate, Action<T, int> content)
        {
            return LazyItem(datas, 1, isAnimate, content);
        }
        public static XLazyTemplate LazyItem<T>(List<T> datas, Action<T> content)
        {
            return LazyItem(datas, 1, false, (t, i) => content?.Invoke(t));
        }

        public static XLazyTemplate LazyItem<T>(List<T> datas, Action<T, int> content)
        {
            return LazyItem(datas, 1, false, content);
        }

        private static List<string> placeholder = new List<string> { "占位" };

        public static XLazyTemplate LazyItem(Action content)
        {
            return LazyItem(placeholder, 1, false, (t, i) => { content(); });
        }
        public static void LazyItem(int span, Action content)
        {
            LazyItem(placeholder, span, false, (t, i) => { content(); });
        }


        public static void PopupCard(
            XState<bool> visibleState,
            Action<XModify> content,
            int blurSigma = 0,
            bool disableOutClick = true,
            Action<XModify, XEventInfo> outSideClick = null,
            [CallerLineNumber] int key = 0)
        {
            var floatingKey = visibleState.GetHashCode();
            if (!popupCards.ContainsKey(floatingKey))
            {
                popupCards.Add(floatingKey, null);
                Action<bool> observer = isShow =>
                {
                    if (isShow)
                    {
                        RenderImp.lockInvalidate = true;
                        var rootView = widget.currentView.RootView() as XGroup;
                        if (blurSigma > 0)
                        {
                            rootView.ChildElemnt(0)?.EnableCache(true);
                            rootView.ChildElemnt(0)?.DrawCache?.Also(n => n.BlurSigma = blurSigma);
                        }
                        var modify = CreateFloatingCard(outSideClick);
                        popupCards[floatingKey] = modify.View;
                        widget.currentParent = modify.View;
                        widget.currentView = modify.View;
                        widget.widgetState = WidgetState.Create;
                        widget.index = 0;
                        SwitchParent(modify.View, () =>
                        {
                            content.Invoke(modify);
                        });
                        if (disableOutClick)
                        {
                            modify.View.EventParams.EventOrCreate(XEventType.Down);
                            modify.View.EventParams.EventOrCreate(XEventType.Hover);
                        }
                        modify.View.StartLayout();
                        modify.View.ChildElemnt(0)?.EventParams.EventOrCreate(XEventType.Hover);
                        ((XGroup)modify.View.Parent).UpdateDrawViews();
                        RenderImp.lockInvalidate = false;
                        modify.View.Invalidate();
                    }
                    else
                    {
                        var parent = popupCards[floatingKey]?.Parent;
                        if (parent == null) return;
                        var rootView = parent.RootView();
                        if (blurSigma > 0)
                        {
                            rootView.ChildElemnt(0)?.EnableCache(false);
                            rootView.ChildElemnt(0)?.DrawCache?.Also(n => n.BlurSigma = 0);
                        }
                        popupCards[floatingKey]?.Removed();
                        if (parent != null)
                        {
                            ((XGroup)parent).UpdateDrawViews();
                        }
                        popupCards[floatingKey] = null;
                        parent?.Invalidate();
                    }
                };
                visibleState.Add(observer);
                if (visibleState.Value)
                {
                    observer.Invoke(true);
                }
            }
        }

        private static XModify CreateFloatingCard(Action<XModify, XEventInfo> outSideClick)
        {
            var rootView = widget.currentView.RootView();
            var box = new XBox();
            box.Key = $"{POPCARD_PRE_KEY}_10000_{rootView.ChildCount() + 1}";
            box.LayoutParams.Width = FILL;
            box.LayoutParams.Height = FILL;
            box.ContentAlignment = XAlignment.Center;

            box.AddEvent(XEventType.DispatchEvent, "CreateFloatingCard", (v, info) =>
            {
                if (info.EventType != XEventType.Down)
                {
                    return;
                }
                var isActionDownConent = false;
                for (int i = 0; i < rootView.ChildCount(); i++)
                {
                    var view = rootView.ChildElemnt(i);
                    var index = rootView.ChildIndex(v);
                    if (view.Key.Contains(POPCARD_PRE_KEY) && view.ChildCount() > 0)
                    {
                        var contentView = view.ChildElemnt(0);
                        if (contentView.RenderRect.Contain(info.Point))
                        {
                            isActionDownConent = true && index <= i;
                        }
                    }
                }
                if (!isActionDownConent)
                {
                    RenderImp.PostToQueue(() =>
                    {
                        outSideClick?.Invoke(new XModify(v), info);
                    });
                }
            });
            rootView.AddView(box);
            return new XModify(box);
        }

        public static void SetCurrentView(XView view)
        {
            widget.currentView = view;
        }

        internal static void SetParentView(XView view)
        {
            widget.currentParent = view;
            widget.index = 0;
        }

        public static XState<T> StateValueOf<T>(T value = default, bool isReset = false, Action<XState<T>> onCreate = null, [CallerLineNumber] int key = 0, string keyPrefix = null)
        {
            var lastPrefix = $"{(widget.currentView.Parent ?? widget.currentParent)?.GetHashCode()}_{typeof(T)}_state_{key}";
            var keyString = $"{widget.currentView.GetHashCode()}_{lastPrefix}";
            if (keyPrefix != null)
            {
                keyString = $"{keyPrefix}-{key}";
            }
            if (keyPrefix == null && !stateValues.ContainsKey(keyString) && stateValues.Keys.Count(n => n.EndsWith(lastPrefix)) > 0 && widget.widgetState == WidgetState.Update)
            {
                var oldKey = stateValues.Keys.FirstOrDefault(n => n.EndsWith(lastPrefix));
                if (oldKey != null)
                {
                    var stateValue = stateValues[oldKey];
                    stateValues.Remove(oldKey);
                    stateValues[keyString] = stateValue;
                    widget.currentView.AddEvent(XEventType.Dispose, keyString, () =>
                    {
                        if (stateValues.ContainsKey(keyString))
                        {
                            (stateValues[keyString] as XState<T>).Dispose();
                            stateValues.Remove(keyString);
                        }
                    });
                }
            }

            if (!stateValues.ContainsKey(keyString) || !(stateValues[keyString] is XState<T>))
            {
                var newState = new XState<T>(value);
                stateValues.Add(keyString, newState);
                onCreate?.Invoke(newState);
                widget.currentView.AddEvent(XEventType.Dispose, keyString, () =>
                {
                    if (stateValues.ContainsKey(keyString))
                    {
                        (stateValues[keyString] as XState<T>).Dispose();
                        stateValues.Remove(keyString);
                    }
                });
            }
            var state = stateValues[keyString] as XState<T>;
            if (isReset)
            {
                state.SetDefault(value);
            }
            return state;
        }

        public static XState<float> AnimateFloatOf(XState<bool> visible, Action<XAnimate> function = null, bool isAutoResetVisible = true, [CallerLineNumber] int key = 0)
        {
            var animateKey = $"animte-{visible.GetHashCode()}-{widget.currentView?.GetHashCode()}";
            var floatState = StateValueOf(0f, true, keyPrefix: animateKey, key: key, onCreate: state =>
            {
                var animateItem = XAnimation.AnimateFloatOf();
                animateItem.Delay = 20;
                function?.Invoke(animateItem);
                var callback = animateItem.OnCallback;
                animateItem.OnCallback = (value, index) =>
                {
                    callback?.Invoke(value, index);
                    state.Value = value;
                    if (!state.HasObservers())
                    {
                        animateItem.Stop();
                    }
                };
                if (isAutoResetVisible)
                {
                    var finished = animateItem.OnFinished;
                    animateItem.OnFinished = () =>
                    {
                        finished?.Invoke();
                        visible.Value = false;
                    };
                }
                Action<bool> observer = isShow =>
                {
                    if (isShow)
                    {
                        animateItem.Stop();
                        animateItem.Start();
                    }
                    else
                    {
                        animateItem.Stop();
                    }
                };
                visible.Add(observer);
                widget.currentView.AddEvent(XEventType.Dispose, state.GetHashCode() + "", () =>
                {
                    animateItem.Stop();
                    visible.Remove(observer);
                });
            });
            visible.Send(visible.Value);
            return floatState;
        }

        private static XModify BindGroup<V, T>(XState<T> state, Action<T> function, bool needLayout = true, [CallerLineNumber] int key = 0) where V : XGroup, new()
        {
            var setter = widget.View<V>(key);
            var group = (XGroup)setter.View;
            SwitchParent(group, () => function.Invoke(state.Value));
            if (widget.widgetState == WidgetState.Create || isHotReload)
            {
                var bindDisposeKey = "bindGroup_dispose";
                group.RemoveEvent(XEventType.Dispose, bindDisposeKey)?.Invoke(group, XEventInfo.Empty);
                Action<T> observer = t =>
                {
                    var tempIdnex = widget.index;
                    var tempCurrentParentView = widget.currentParent;
                    var tempCurrentView = widget.currentView;
                    widget.currentParent = group;
                    widget.currentView = group;
                    widget.index = 0;
                    group?.RefreshParentCache();
                    SwitchParent(group, () => function.Invoke(t));
                    if (needLayout)
                    {
                        group.BubbleUpLayout();
                        group.Invalidate();
                    }
                    widget.index = tempIdnex;
                    widget.currentParent = tempCurrentParentView;
                    widget.currentView = tempCurrentView;
                    widget.widgetState = WidgetState.Update;

                };
                state.Add(observer);
                group.AddEvent(XEventType.Dispose, "bindGroup_dispose", () =>
                {
                    state.Remove(observer);
                });
            }
            return setter;
        }

        private static void SwitchParent(XView view, Action function = null)
        {
            if (function != null)
            {
                var tempIndex = widget.index;
                var tempView = widget.currentParent;
                widget.currentParent = view;
                widget.index = 0;
                widget.currentView = view;
                function.Invoke();
                // 检测索引是否是最后一个，不是说明有多余的元素需要删除
                if (widget.index < widget.currentParent.ChildCount() && !(view is XLazy))
                {
                    var currentParent = (XGroup)widget.currentParent;
                    var views = currentParent.Childs.GetRange(widget.index, currentParent.Childs.Count - widget.index);
                    foreach (var item in views)
                    {
                        currentParent.RemoveView(item);
                    }
                }
                widget.currentParent = tempView;
                //widget.currentView = view;
                widget.index = tempIndex;
            }
        }

        internal XModify View<T>(int keyInt) where T : XView, new()
        {
            string key = keyInt.ToString();
            XView view = null;
            // 首次创建的时候都是new
            var isCreated = false;
            if (currentParent != null && index < currentParent.ChildCount())
            {
                var parent = (XGroup)currentParent;
                view = parent.Childs[index];
                // 该索引view的key和函数所在的key不同需要检查是否新增或者调整顺序
                if (view.Key != key || view.GetType() != typeof(T))
                {
                    var oldView = view;
                    view = parent.Childs.Skip(index + 1).FirstOrDefault(n => n.Key == key && n.GetType() == typeof(T));
                    // key对应的view不存在就添加新的,否则就调换顺序

                    if (view == null)
                    {
                        view = new T();
                        view.Key = key;
                        currentParent.InsertView(index, view);
                        isCreated = true;
                    }
                    else
                    {
                        var viewIndex = parent.Childs.IndexOf(view);
                        for (int i = index; i < viewIndex; i++)
                        {
                            currentParent.RemoveView(parent.Childs[index]);
                        }
                    }
                }
            }
            else
            {
                view = new T();
                view.Key = key;
                currentParent?.InsertView(index, view);
                isCreated = true;
            }
            index += 1;
            widget.widgetState = isCreated ? WidgetState.Create : WidgetState.Update;
            var builder = new XModify(view);
            widget.currentView = view;
            return builder.ResetParams();
        }

        public enum WidgetState
        {
            Create,
            Update,
            Clear
        }
    }
}
