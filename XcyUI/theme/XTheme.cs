using System;
using System.Collections.Concurrent;
using XcyUI.utils;
using XcyUI.widgets;

namespace XcyUI.theme
{
    public class XTheme
    {
        public readonly static LinkedHashMap<string, object> Images = new LinkedHashMap<string, object>()
        {
            CacheNum = 50,
            OnRemoved = image => (image as IDisposable)?.Dispose()
        };

        public readonly static ConcurrentDictionary<int, object> SvgResources = new ConcurrentDictionary<int, object>();
        public readonly static ConcurrentDictionary<int, object> ImgResources = new ConcurrentDictionary<int, object>();

        public static XState<bool> DarkModeState = new XState<bool>(false);
        public static XThemeColor Light = new XThemeColor();
        public static XThemeColor Dark = new XThemeDarkColors();
        public static XThemeColor Color = Light;
        public static XThemeRadius Radius = new XThemeRadius();
        public static XThemeSizes Size = new XThemeSizes();
        public static XThemeWeights Weight = new XThemeWeights();
        public static XThemeShadows Shadow = new XThemeShadows();
        public static int DesignWidth = 1920;
        public static int TargetWidth = 1920;
        public static int ScreenWidth;
        public static int ScreenHeight;
        public static float Scale = 1f;
        public static bool EnableDebugRect = false;
       
        public static void ApplyThemeColor(XThemeColor color, bool isDarkMode = false)
        {
            if (Color == color) return;
            Color = color;
            DarkModeState.SetDefault(isDarkMode);
            DarkModeState.Refresh();
        }

        public static void ApplyTheme(bool isDarkMode)
        {
            if (DarkModeState.Value != isDarkMode)
            {
                Color = isDarkMode ? Dark : Light;
                DarkModeState.Value = isDarkMode;
            }
        }

        public static string DefaultFontName = "Microsoft YaHei";
    }
}
