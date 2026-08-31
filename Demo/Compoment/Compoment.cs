using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using XcyUI.Controls;
using XcyUI.Core.Utils;
using XcyUI.GLFW;
using XcyUI.models;
using XcyUI.SkiaSharp;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyDemo.Compoment
{
    public static class Compoment
    {
        public static void MinimizeButton()
        {
            Spacer().Size(WRAP, FILL).AspectRatio(1.3f)
               .Click(XApplication.MinimizeWindow)
               .EnableCache(true)
               .OnDraw(modify =>
               {
                   var canvas = (SKCanvas)RenderImp.GetCanvas();
                   var renderRect = modify.View.RenderRect;
                   var rect = new XRect(20, 20);
                   var point = XRectUtils.GetDockedPoint(renderRect, rect, XAlignment.Center);
                   rect.X = point.X;
                   rect.Y = point.Y;
                   var paint = PaintCache.GetBorderPaint(XBrush.Empty.Copy(XTheme.Color.PrimaryText), 2);
                   canvas.DrawLine(rect.LeftCenter.ToSKPoint(), rect.RightCenter.ToSKPoint(), paint);
               });
        }

        public static void MaximizeButton()
        {
            Spacer().Size(WRAP, FILL).AspectRatio(1.3f)
               .Click((modify, info) =>
               {
                   XApplication.ToggleMaximize();
                   modify.View.Invalidate();
               })
               .EnableCache(true)
               .OnDraw(modify =>
               {
                   var canvas = (SKCanvas)RenderImp.GetCanvas();
                   var renderRect = modify.View.RenderRect;
                   var rect = new XRect(20, 20);
                   var point = XRectUtils.GetDockedPoint(renderRect, rect, XAlignment.Center);
                   rect.X = point.X;
                   rect.Y = point.Y;
                   var paint = PaintCache.GetBorderPaint(XBrush.Empty.Copy(XTheme.Color.PrimaryText), 2);
                   if (XApplication.IsMaximized())
                   {
                       rect.Scale(-2);
                       var twoRect = rect;
                       twoRect.Translation(4, -4);
                       canvas.DrawLine(twoRect.LeftTop.ToSKPoint(), twoRect.RightTop.ToSKPoint(), paint);
                       canvas.DrawLine(twoRect.RightTop.ToSKPoint(), twoRect.RightBottom.ToSKPoint(), paint);
                   }
                   canvas.DrawRect(rect.ToSKRect(), paint);
               });
        }

        public static void CloseButton()
        {
            var isHoverState = StateValueOf(false);
            Spacer().Size(WRAP, FILL).AspectRatio(1.3f)
                .Click(XApplication.CloseWindow)
                .ToggleHover(isHover => isHoverState.Value = isHover)
                .HoverBackgroundColor(XColors.FromHex("#ed4c4c"))
                .OnDraw(modify =>
                {
                    var canvas = (SKCanvas)RenderImp.GetCanvas();
                    var renderRect = modify.View.RenderRect;
                    var rect = new XRect(20, 20);
                    var point = XRectUtils.GetDockedPoint(renderRect, rect, XAlignment.Center);
                    rect.X = point.X;
                    rect.Y = point.Y;
                    var color = isHoverState.Value ? XColors.White : XTheme.Color.PrimaryText;
                    var paint = PaintCache.GetBorderPaint(XBrush.Empty.Copy(color), 2);
                    canvas.DrawLine(rect.LeftTop.ToSKPoint(), rect.RightBottom.ToSKPoint(), paint);
                    canvas.DrawLine(rect.RightTop.ToSKPoint(), rect.LeftBottom.ToSKPoint(), paint);
                });
        }
    }
}
