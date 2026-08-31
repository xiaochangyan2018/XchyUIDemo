using XcyUI.navigation;
using XcyUI.views;

namespace XcyUI.models
{
    public interface IRenderBackend
    {
        /// <summary>
        /// 创建画布
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="paramsData">底层opengl函数指针</param>
        void CreateSurface(int width, int height, object paramsData);
        /// <summary>
        /// 重新设置画布大小
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="paramsData"></param>
        void ResetSurface(int width, int height);
        /// <summary>
        /// 渲染元素树
        /// </summary>
        void Render();
        /// <summary>
        /// 窗口唯一的Page对象
        /// </summary>
        XPage Page { get; set; }
        /// <summary>
        /// 设置画布背景,默认XTheme.Color.Backgound
        /// </summary>
        /// <param name="color"></param>
        void SetBackgoundColor(XColor color);
        /// <summary>
        /// 销毁
        /// </summary>
        void Dispose();
    }
}
