using XcyDemo.StudentManagent.Common.Models;
using XcyDemo.StudentManagent.Services;
using XcyUI.Controls;
using XcyUI.GLFW;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyDemo.StudentManagent.Components.XCommonComponents;
using static XcyDemo.Sample.Component.CustomControls;
using static XcyUI.Controls.Controls;
using static XcyUI.widgets.XCompose;
using static XcyUI.widgets.XDIWidget;

namespace XcyUI.Demo.StudentManagent.Pages;

public static class StudentListPage
{
    private static XState<bool> showEdityForm = new();
    private static XState<bool> refreshList = new();
    public static XModify View()
    {
        return Column(() =>
        {
            TopBar("学生信息管理");
            var studentVisibleState = StateValueOf(false);
            var service = Service<UserService>();
            var student = StateValueOf(new StudentInfo());
            EdityStudent(showEdityForm, student);
            Column(() =>
            {
                Row(() =>
                {
                    Spacer().Weight(1);

                    Row(() =>
                    {
                        Icon(SvgRes.Search).Size(24).Color(XTheme.Color.PlaceholderText);
                        Input().Width(230).Hint("搜索学生");
                    }).Space(10).PrimaryInput();

                    IconButton(SvgRes.Plus, "添加学生").PrimaryButton(()=> showEdityForm.Value = true);

                }).Width(FILL).Padding(10).Space(10);
                var studentsState = StateValueOf(service.Students);
                DataGrid(studentsState, new()
                {
                    new("",80,v=> ""){ Fixed = Fixed.Left,SelectItemsState = new(new()) },
                    new("学号" , 120, v=> v.Number){Alignment = XHorizontalAlignment.Center},
                    new("姓名" , 120, v=> v.Name),
                    new("性别" , 120, v=> v.Sex == 1? "男":"女"){Alignment = XHorizontalAlignment.Center},
                    new("出生日期" , 150, v=> v.Birthday),
                    new("班级" , 150, v=> v.ClassName),
                    new("专业" , -1, v=> v.SubjectName),
                    new("操作" , 150, v=> "")
                    {
                        Fixed = Fixed.Right,
                        Alignment = XHorizontalAlignment.Center,
                        CellContent = (cell, v) =>
                        {
                            Icon(SvgRes.Edit).Hand().Click(()=>
                            {
                                student.Value = v;
                                showEdityForm.Value = true;
                            }, false);
                        }
                    },
                }, isGridBorder: false, modify: (modify, index) =>
                {
                    if (index >= 0)
                    {
                        var color = index % 2 == 0 ? XTheme.Color.LighterFill : XTheme.Color.BaseFill;
                        modify.Background(color).HoverBackgroundAllColor(XTheme.Color.LightFill);
                    }
                })
                .Weight(1).Bind(refreshList, (modify, info) => studentsState.Refresh());
            }).Weight(1);
        });
    }

    public static XModify OtherSimple(string name)
    {
        return Column(() =>
        {
            TopBar(name);
            var studentVisibleState = StateValueOf(false);
            Column(() =>
            {
                Row(() =>
                {
                    Text(name).FontSize(24);
                    Spacer().Weight(1);

                    Row(() =>
                    {
                        Icon(SvgRes.Search).Size(24).Color(XTheme.Color.PlaceholderText);
                        Input().Width(230).Hint("搜索");
                    }).Space(10).PrimaryInput();

                    IconButton(SvgRes.Plus, "添加").PrimaryButton();
                }).Width(FILL).Padding(10).Space(10);
                var service = Service<UserService>();
                var studentsState = StateValueOf(service.Students);
                DataGrid(studentsState, new()
                {
                    new("学号" , 120, v=> v.Number),
                    new("姓名" , 120, v=> v.Name),
                    new("性别" , 120, v=> v.Sex == 1? "男":"女"),
                    new("出生日期" , -1, v=> v.Birthday),
                    new("班级" , -1, v=> v.ClassName),
                    new("专业" , -1, v=> v.SubjectName)
                }).Weight(1);
            }).Margin(20).Card().Weight(1);
        });
    }

    public static void EdityStudent(XState<bool> visibleState, XState<StudentInfo> infoState)
    {
        DialogFormView(visibleState, state =>
        {
            Column(() =>
            {
                var info = infoState.Value;
                Text("添加学生").H2().Alignment(XAlignment.Center);
                Spacer();
                Column(() =>
                {
                    ResetValidate();
                    LableInput("学号", info.Number, v => info.Number = v, required: true).Focus();

                    LableInput("姓名", info.Name, v => info.Name = v);

                    LableRadioGroup("性别", [("男", 1), ("女", 2)], info.Sex, v => info.Sex = v);

                    LableDate("出生日期", string.IsNullOrEmpty(info.Birthday)?DateTime.Now: Convert.ToDateTime(info.Birthday), v => info.Birthday = v.ToString("yyyy-MM-dd"), required: true);

                    LableInput("班级", info.ClassName, v => info.ClassName = v);

                    LableInput("专业", info.SubjectName, v => info.SubjectName = v);

                }).Weight(1).Scrollable().Height(WRAP).Space(20).Padding(20);

                Row(() =>
                {
                    Text("取消").SubButton(()=>
                    {
                        state.Value = true;
                    }).Width(150);

                    AsyncButton(
                        text: "保存",
                        loadingText: "保存中...",
                        preFunc: startState => Validate(() => startState.Value = true),
                        asyncFun: () =>
                        {
                            ShowToast("保存成功");
                            state.Value = true;
                            refreshList.Send(true);
                        }).Width(150);
                })
                .Width(FILL).Space(50)
                .HorizontalAlignment(XHorizontalAlignment.Center);
            })
            .Space(20)
            .Size(600, WRAP)
            .MaxHeight(800)
            .Tabindex(100)
            .Clip(clipPadding: false)
            .Padding(vertical: 10)
            .MeasureStart(modify=> modify.View.Parent.LayoutParams.Padding = new())
            .HorizontalAlignment(XHorizontalAlignment.Left);
        });
    }
}
