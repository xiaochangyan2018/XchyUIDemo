using XcyUI.widgets;

namespace XcyDemo.Config
{
    public static class Router
    {
        public static XState<int> RouterState = new();
        public static int PreRouterId = -1;
        public static readonly (int,string)[] Routers = [(StudentManagent,"学生管理"), (DataGrid,"数据表格"), (MuiltWindow, "多窗口"), (XTheme,"主题样式"), (Resources,"图标")];
        public const int StudentManagent = 0;
        public const int DataGrid = 1;
        public const int MuiltWindow = 2;
        public const int XTheme = 3;
        public const int Resources = 4;
        
    }
}
