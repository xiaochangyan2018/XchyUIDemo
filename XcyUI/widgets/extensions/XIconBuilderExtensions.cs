using XcyUI.expansions;
using XcyUI.models;
using XcyUI.views;

namespace XcyUI.widgets.extensions
{
    public static class XIconBuilderExtensions
    {
        
        public static XModify ResId(this XModify builder, int resId)
        {
            builder.AsView<XIcon>()?.Also(n => n.ResId = resId);
            return builder;
        }


        public static XModify IconSize(this XModify builder, int width,int height)
        {
            builder.AsView<XIcon>()?.Also(n =>
            {
                n.IconWidth = width > 0 ? width.AsPx() : width;
                n.IconHeight = height > 0 ? height.AsPx() : height;
            });
            return builder;
        }

        public static XModify IconSize(this XModify builder, int size)
        {
            return builder.IconSize(size, size);
        }

        public static XModify ScaleType(this XModify setter, XScaleType scaleType)
        {

            setter.AsView<XIcon>()?.Also(n => n.ScaleType = scaleType);
            return setter;
        }

        public static XModify Color(this XModify setter, XColor color)
        {
            setter.AsView<XIcon>()?.Also(n => n.Color = new XBrush(color));
            setter.AsView<XText>()?.Also(n => n.Font.Color = new XBrush(color));
            return setter;
        }
        public static XModify Color(this XModify setter, XBrush color)
        {
            setter.AsView<XIcon>()?.Also(n => n.Color = color);
            setter.AsView<XText>()?.Also(n => n.Font.Color = color);
            return setter;
        }
    }
}
