using System;
using System.Collections.Generic;
using XcyUI.views;

namespace XcyUI.models
{
    /// <summary>
    /// XcyUI需要渲染端实现的接口
    /// </summary>
    public interface IDraw
    {
        /// <summary>
        /// 绘制缓存
        /// </summary>
        void DrawCache(XRect rect, XStyle style, XDrawCache cache, Action onDraw);
        /// <summary>
        /// 绘制矩形
        /// </summary>
        void DrawRect(XRect rect, XStyle style, Action onDraw);     
        /// <summary>
        /// 绘制文字
        /// </summary>
        void DrawText(List<XChar> chars);
        /// <summary>
        /// 绘制图片
        /// </summary>
        /// <param name="resId">预先存入的图片资源ID</param>
        /// <param name="rect">矩形范围</param>
        /// <param name="color">着色器</param>
        /// <param name="scaleType">缩放类型</param>
        void DrawImage(int resId, XRect rect, XBrush color, XScaleType scaleType);
        /// <summary>
        /// 绘制图片
        /// </summary>
        /// <param name="images">第三方ragb像素点</param>
        void DrawImage(byte[] images, XRect rect, XBrush color, XScaleType scaleType);
        /// <summary>
        /// 绘制SVG
        /// </summary>
        /// <param name="resId">预先存入的SKPictrue对应的资源ID</param>
        /// <param name="rect"></param>
        /// <param name="color">着色器，支持渐变</param>
        void DrawSvg(int resId, XRect rect, XBrush color);
        /// <summary>
        /// 获取svg的SKPictrue等缓存对象
        /// </summary>
        /// <param name="svgContent"></param>
        /// <returns></returns>
        object GetSvg(string svgContent);
        /// <summary>
        /// 获取图片对象
        /// </summary>
        /// <param name="base64">图片内容</param>
        /// <param name="hasBuffer">是否存入像素点</param>
        /// <returns></returns>
        XBitmap GetBitmap(string base64, bool hasBuffer);
        /// <summary>
        /// 测量大小
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        XRect MeasureText(string text, XFont font);
        /// <summary>
        /// 刷新缓存，所有缓存为SKPictrue/SKBitmap的元素都会失效，适用全局刷新，比如主题切换，资源更新等
        /// </summary>
        /// <param name="isRefresh"></param>
        void RefreshCache(bool isRefresh);
        /// <summary>
        /// 获取SKCanvas等对象
        /// </summary>
        /// <returns></returns>
        object GetCanvas();
        /// <summary>
        /// 绘制扇形
        /// </summary>
        /// <param name="rect">矩形范围</param>
        /// <param name="style">样式</param>
        /// <param name="startAngle">开始角度</param>
        /// <param name="sweepAngle">绘制多少度</param>
        /// <param name="userCenter">扇形是否有连接线</param>
        void DrawArc(XRect rect, XStyle style, float startAngle, float sweepAngle, bool userCenter);
        /// <summary>
        /// 开始绘制一段path
        /// </summary>
        /// <param name="rect">矩形范围</param>
        /// <param name="style">样式</param>
        /// <param name="isCache">是否缓存</param>
        /// <param name="content">路径具体的函数</param>
        void DrawPath(XRect rect, XStyle style, bool isCache, Action content);
        /// <summary>
        /// 移动到起点
        /// </summary>
        void MoveTo(int x,int y);
        /// <summary>
        /// 和另一个点连接成一条线
        /// </summary>
        void LineTo(int x, int y);
        /// <summary>
        /// 和另一个点连接成一个圆角
        /// </summary>
        void ArcTo(int x,int y, int radius);
        /// <summary>
        /// 三次贝塞尔曲线
        /// </summary>
        void CubicTo(XPoint point1,XPoint point2,XPoint point3);
        /// <summary>
        /// 添加渐变矩形
        /// </summary>
        /// <param name="rect">矩形范围</param>
        /// <param name="colors">渐变颜色</param>
        /// <param name="direction">渐变方向</param>
        void AddRect(XRect rect, XColor[] colors, XGradientDirection direction);
    }
}