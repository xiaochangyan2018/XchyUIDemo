using System;

namespace XcyUI.models
{
    public interface IWindow
    {
        /// <summary>
        /// 更改输入法的位置
        /// </summary>
        /// <param name="point"></param>
        void UpdateImmPosition(XPoint point);
        /// <summary>
        /// 设置鼠标类型
        /// </summary>
        /// <param name="type"></param>
        void SetCursor(XCursorType type);
        /// <summary>
        /// 使画布失效重绘
        /// </summary>
        void Invalidate();
        /// <summary>
        /// 切到主线程，如果是主线就直接执行如果不是就入队执行
        /// </summary>
        /// <param name="action"></param>
        void Post(Action action);
        /// <summary>
        /// 入队执行
        /// </summary>
        /// <param name="action"></param>
        void PostToQueue(Action action);
        /// <summary>
        /// 设置函数在帧循环里执行
        /// </summary>
        /// <param name="action"></param>
        void PostToRender(Action action);
        /// <summary>
        /// 添加窗口关闭时回调
        /// </summary>
        /// <param name="action"></param>
        void AddCloseAction(Action action);
        /// <summary>
        /// 移动窗口
        /// </summary>
        void MoveWindow();
    }
}
