using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using XcyUI.animation;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.views;

namespace XcyUI.widgets
{
    public class XModify
    {
        internal static Dictionary<string, Action> bindKeys = new Dictionary<string, Action>();
        public XView View { get; private set; }

        public static XModify With(XView view)
        {
            return new XModify(view);
        }

        public bool IsScrolledToBottom()
        {
            return AsView<XGroup>()?.IsScrolledToBottom() ?? false;
        }
        public bool IsScrolledToRight()
        {
            return AsView<XGroup>()?.IsScrolledToRight() ?? false;
        }

        public bool IsScrolledToLeft()
        {
            return AsView<XGroup>()?.IsScrolledToLeft() ?? false;
        }

        public XModify RefreshParentCache()
        {
            View.RefreshParentCache();
            return this;
        }
        public XModify(XView view)
        {
            View = view; 
        }
        
        public XModify Background(XBrush brush)
        {
            View.Style.Background = brush;
            return this;
        }

        public XModify Background(XColor color)
        {
            View.Style.Background = new XBrush() { StartColor = color };
            return this;
        }

        public XModify Border(XColor color, float? left = null, float? top = null, float? right = null, float? bottom = null, XDashType? type = null)
        {
            var border = View.Style.Border.Size;
            View.Style.Border = new XBorder(new XBrush() { StartColor = color },
                new XSpace(left?.AsPx() ?? border.Left, top?.AsPx() ?? border.Top, right?.AsPx() ?? border.Right, bottom?.AsPx() ?? border.Bottom), type ?? View.Style.Border.DashType);
            return this;
        }

        public XModify Border(XColor color)
        {
            View.Style.Border = View.Style.Border.Copy(color);
            return this;
        }

        public XModify Border(XColor color, float size, XDashType type = XDashType.Solid)
        {
            return Border(color, size,size,size,size,type);
        }

        public XModify Border(XBorder border)
        {
            View.Style.Border = border;
            return this;
        }
        public XModify Radius(XSpace space)
        {
            View.Style.Radius = space;
            return this;
        }
        public XModify Radius(int left = 0, int top = 0, int right = 0, int bottom = 0)
        {
            View.Style.Radius = new XSpace(left.AsPx(), top.AsPx(), right.AsPx(), bottom.AsPx());
            return this;
        }

        public XModify Radius(int radius)
        {
            return Radius(radius, radius, radius, radius);
        }

        public XModify Circle()
        {
            var radius = 0.5f;
            View.Style.Radius = new XSpace(radius, radius, radius, radius);
            return this;
        }

        public XModify Shadow(int x, int y, XColor color, int blur)
        {
            View.Style.Shadow = new XShadow()
            {
                Dx = x.AsPx(),
                Dy = y.AsPx(),
                Color = color,
                Blur = blur.AsPx()
            };
            return this;
        }

        public XModify Shadow(XShadow shadow)
        {
            View.Style.Shadow = shadow;
            return this;
        }

        public XModify Alpha(float alpha)
        {
            View.Parent?.RefreshParentCache();
            View.EnableCache(true);
            View.DrawCache.Alpha = alpha;
            return this;
        }

        public XModify Scale(float scale, XPoint? point = null)
        {
            return Scale(scale,scale,point);
        }
        public XModify Scale(float scaleX, float scaleY, XPoint? point = null)
        {
            View.Parent?.RefreshParentCache();
            View.EnableCache(true);
            View.DrawCache.ScaleX = scaleX;
            View.DrawCache.ScaleY = scaleY;
            View.DrawCache.ScalePoint = point ?? XPoint.Empty;
            return this;
        }

        public XModify Rotate(float degrees, XPoint? point = null)
        {
            View.Parent?.RefreshParentCache();
            View.EnableCache(true);
            View.DrawCache.Degrees = degrees;
            View.DrawCache.DegreesPoint = point ?? XPoint.Empty;
            return this;
        }
       

        public XModify Translate(float? x = null, float? y = null)
        {
            View.Parent?.RefreshParentCache();
            View.EnableCache(true);
            View.DrawCache.TranslateX = (int)(x??-1);
            View.DrawCache.TranslateY = (int)(y??-1);
            return this;
        }

        public XModify EnableCache(bool enable, XCacheType cacheType = XCacheType.Pictrue, int blurSigma = 0)
        {
            View.EnableCache(enable, cacheType);
            View.DrawCache.BlurSigma = blurSigma;
            return this;
        }

        public XModify BlurSigma(int blurSigma = 0)
        {
            View.Parent?.RefreshParentCache();
            View.EnableCache(true);
            View.DrawCache.BlurSigma = blurSigma;
            return this;
        }

        public XModify CacheShadow(bool enable,bool isClear = false)
        {
            View.DrawCache.CacheShadow = enable;
            if (isClear)
            {
                View.AddEvent(XEventType.Dispose, "clear_cache_shadow", () =>
                {
                    var hashCode = View.Style.Shadow.ShadowHashCode();
                    var keys = XTheme.Images.Keys.ToList();
                    foreach(var key in keys)
                    {
                        if (key.Contains(hashCode.ToString()))
                        {
                            XTheme.Images.Remove(key);
                        }
                    }
                });
            }
            else
            {
                View.RemoveEvent(XEventType.Dispose, "clear_cache_shadow");
            }
            return this;
        }
       
        public XModify Shadow()
        {
            return Shadow(XTheme.Shadow.Card);
        }

        public XModify Clip(bool clipChildren = true, bool clipPadding = true)
        {
            View.Style.Also(n =>
            {
                n.IsClip = clipChildren;
                n.IsClipPadding = clipPadding;
            });
            return this;
        }

        public XModify EnableOverDraw(bool overDraw = true)
        {
            View.Style.IsOverDraw = overDraw;
            return this;
        }
        public XModify Size(int size)
        {
            return Size(size, size);
        }
        public XModify Size(int width, int height)
        {
            View.LayoutParams.Width = width > 0 ? width.AsPx() : width;
            View.LayoutParams.Height = height > 0 ? height.AsPx() : height;
            return this;
        }

        public XModify Width(int width)
        {
            View.LayoutParams.Width = width > 0 ? width.AsPx() : width;
            return this;
        }

        public XModify Height(int height)
        {
            View.LayoutParams.Height = height > 0 ? height.AsPx() : height;
            return this;
        }

        public XModify Weight(int weight)
        {
            View.LayoutParams.Weight = weight;
            return this;
        }

        public XModify MaxHeight(int height)
        {
            View.LayoutParams.MaxHeight = height.AsPx();
            return this;
        }

        public XModify MaxWidth(int width)
        {
            View.LayoutParams.MaxWidth = width.AsPx();
            return this;
        }

        public XModify MinHeight(int height)
        {
            View.LayoutParams.MinHeight = height.AsPx();
            return this;
        }

        public XModify MinWidth(int width)
        {
            View.LayoutParams.MinWidth = width.AsPx();
            return this;
        }

        public XModify ZIndex(int index)
        {
            View.LayoutParams.ZIndex = index;
            if (View.Parent is XGroup)
            {
                ((XGroup)View.Parent).UpdateDrawViews();
            }
            return this;
        }

        public XModify Alignment(XAlignment alignment)
        {
            View.LayoutParams.Alignment = alignment;
            return this;
        }

        public XModify Padding(XSpace space)
        {
            View.LayoutParams.Padding = space;
            return this;
        }

        public XModify Padding(int? left = null, int? top = null, int? right = null, int? bottom = null)
        {
            var padding = View.LayoutParams.Padding;
            View.LayoutParams.Padding = new XSpace(left?.AsPx() ?? padding.Left, top?.AsPx() ?? padding.Top, right?.AsPx() ?? padding.Right, bottom?.AsPx() ?? padding.Bottom);
            return this;
        }

        public XModify Padding(int? horizontal = null, int? vertical = null)
        {
            return Padding(horizontal, vertical, horizontal, vertical);
        }

        public XModify Padding(int size)
        {
            return Padding(size, size, size, size);
        }

        public XModify Margin(int? left = null, int? top = null, int? right =null, int? bottom = null)
        {
            var margin = View.LayoutParams.Margin;
            View.LayoutParams.Margin = new XSpace(left?.AsPx() ?? margin.Left, top?.AsPx() ?? margin.Top, right?.AsPx() ?? margin.Right, bottom?.AsPx() ?? margin.Bottom);
            return this;
        }

        public XModify Margin(int? horizontal = null, int? vertical = null)
        {
            return Margin(horizontal, vertical, horizontal, vertical);
        }

        public XModify Margin(int size)
        {
            return Margin(size, size, size, size);
        }

        public XModify Visible(XVisibleType visible)
        {
            View.LayoutParams.Visible = visible;
            return this;
        }
        public XModify InVisible(bool isShow)
        {
            return Visible(isShow ? XVisibleType.Visible : XVisibleType.InVisible);
        }
        public XModify Visible(bool isShow)
        {
            return Visible(isShow ? XVisibleType.Visible : XVisibleType.Gone);
        }

        public XModify Freeze(bool freeze = true)
        {
            View.LayoutParams.Freeze = freeze;
            return this;
        }

        public XModify AspectRatio(float aspectRatio)
        {
            View.LayoutParams.AspectRatio = aspectRatio;
            return this;
        }

        public XModify Focusable(bool focusable)
        {
            View.EventParams.Focusable = focusable;
            return this;
        }

        public XModify EnableEvent(bool enable)
        {
            View.EventParams.Enable = enable;
            return this;
        }

        public XModify Tabindex(int index)
        {
            View.Tabindex = index;
            return this;
        }

        public XModify Removed()
        {
            View.Removed();
            return this;
        }

        public void ClearAllBind()
        {
            var items = bindKeys.Where(n => n.Key.StartsWith($"{View.GetHashCode()}")).ToList();
            items.ForEach(n =>
            {
                bindKeys[n.Key].Invoke();
                bindKeys.Remove(n.Key);
            });
        }
        
        public XModify Bind<T>(XState<T> state, Action<XModify, T> function, bool needLayout = false,[CallerLineNumber] int key = 0)
        {
            function.Invoke(this, state.Value);
            var keyString = $"{View.GetHashCode()}-{state.GetHashCode()}-{typeof(T)}-{key}";
            if (!bindKeys.ContainsKey(keyString) || XCompose.isHotReload)
            {
                Action<T> observer = (t) =>
                {
                    var margin = View.LayoutParams.Margin;
                    function.Invoke(this, t);
                    XView layoutView = View;
                    if (needLayout)
                    {
                        var view = View;
                        if (!view.LayoutParams.Margin.Equals(margin) || view.Parent.HasWeightChild())
                        {
                            view = view.Parent == null ? view : view.Parent;
                        }
                        view.BubbleUpLayout();
                        view.Invalidate();
                    }
                    else if (!XAnimation.IsStart())
                    {
                        View.Invalidate();
                    }
                    else
                    {
                        View.Parent?.RefreshParentCache();
                    }
                };
                state.Add(observer);
                bindKeys[keyString] = () => state.Remove(observer);
                View.AddEvent(XEventType.Dispose, "binding_dispose", () =>
                {
                    state.Remove(observer);
                    bindKeys.Remove(keyString);
                });
            }
            return this;
        }
        public T AsView<T>() where T : XView, new()
        {
            if (View is T) return (T)View;
            return null;
        }
    }
}
