using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;

namespace XcyUI.views
{
    public class XIcon : XView
    {
        public XBrush Color { get; set; }
        public XScaleType ScaleType { get; set; }
        public int ResId { get; set; }
        private XRect iconRect;
        public int IconWidth { get; set; }
        public int IconHeight { get; set; }

        public XIcon()
        {
            ScaleType = XScaleType.Normal;
        }

        protected override void OnMeasure()
        {
            base.OnMeasure();
            var contentRect = ContentRect;
            contentRect.X = 0;
            contentRect.Y = 0;
            iconRect.Width = IconWidth > 0 ? IconWidth : ContentRect.Width;
            iconRect.Height = IconHeight > 0 ? IconHeight : ContentRect.Height;
            if (Width < 0)
            {
                Width = iconRect.Width + LayoutParams.Padding.HorizontalSize;
            }
            if (Height < 0)
            {
                Height = iconRect.Height + LayoutParams.Padding.VerticalSize;
            }
            iconRect.Move(contentRect, XAlignment.Center);
        }

        protected override void OnLayout()
        {
            var contentRect = ContentRect;
            iconRect.Move(contentRect, XAlignment.Center);
        }

        public override void Translation(int x, int y)
        {
            base.Translation(x, y);
            iconRect.Translation(x, y);
        }

        protected override void DrawContent()
        {
            if (XTheme.ImgResources.ContainsKey(ResId))
            {
                RenderImp.DrawImage(ResId, iconRect, Color, ScaleType);
            }
            else
            {
                RenderImp.DrawSvg(ResId, iconRect, Color);
            }
        }
    }
}
