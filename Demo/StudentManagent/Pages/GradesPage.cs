using XcyDemo.StudentManagent.Services;
using XcyUI.Controls;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.views;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyDemo.StudentManagent.Components.XCommonComponents;
using static XcyUI.Controls.Controls;
using static XcyUI.widgets.XDIWidget;
using static XcyUI.widgets.XCompose;

namespace XcyDemo.StudentManagent.Pages
{
    public static class GradesPage
    {
        public static XModify View()
        {
            var service = Service<UserService>();
            return Column(() =>
            {
                TopBar("成绩管理").Shadow(new XShadow());
                var scolledState = StateValueOf<int>();
                var isScolledToRightState = StateValueOf(false);
                var widthState = StateValueOf((0, 0));
                var dataState = StateValueOf(service.GradesInfos);
                DataGrid(dataState,
                [
                    new("学号", 150, v=> v.UserNumber) { IsResize = true },
                    new("姓名", 150, v => v.Name){ IsResize = true },
                    new("班级", 200, v => v.ClassName){ IsResize = true },
                    new("科目", 200, v => v.SubjectName){ IsResize = true },
                    new("成绩", 150, v => v.GradesValue) { IsResize = true },
                    new("学期", 200, v => v.Renewal) { IsResize = true },
                    new("考试日期", 200, v =>v.ExamDate){ IsResize = true },
                    new("操作", 100, v =>"")
                    { 
                        Fixed= Fixed.Right,
                        CellContent = (cell, data) => {
                            Row(()=>
                            {
                                Icon(SvgRes.Edit).HoverCursor(XCursorType.Hand);
                                Icon(SvgRes.Delete).HoverCursor(XCursorType.Hand);
                            }).Size(FILL).Space(10).HorizontalAlignment(XHorizontalAlignment.Left);
                        }
                    },
                ], isGridBorder: true).Weight(1);
            }).HorizontalAlignment(XHorizontalAlignment.Left);
        }
    }
}
