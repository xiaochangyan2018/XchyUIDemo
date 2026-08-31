
using XcyDemo.StudentManagent.Common.Models;
using XcyUI.widgets;

namespace XcyDemo.StudentManagent.State
{
    public static class StateRepository
    {
        public static XState<MenuItem> SelectedMenuItem = new();
    }
}
