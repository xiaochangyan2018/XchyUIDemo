using System.Drawing;
using XcyDemo.StudentManagent.Common.Convert;
using XcyDemo.StudentManagent.Common.Models;
using XcyDemo.StudentManagent.Services;
using XcyUI.Controls;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyDemo.StudentManagent.Components.XCommonComponents;
using static XcyDemo.StudentManagent.Theme.XStyleBuilder;
using static XcyUI.Controls.Controls;
using static XcyUI.widgets.XDIWidget;
using static XcyUI.widgets.XCompose;

namespace XcyDemo.StudentManagent.Pages
{
    public static class HomePage
    {
        public static XModify View()
        {
            var service = Service<UserService>();
            return Column(() =>
            {
                TopBar("首页");
                Flow(() =>
                {
                    foreach (var data in service.UserSumDatas)
                    {
                        UserSumCard(data).Height(150).DCard().FadeIn();
                    }
                    StudentGradesBarChart().Height(350).Colspan(2).DCard();
                    AttendanceTrendLineChart().Height(350).Colspan(2).DCard();
                    MessageList().Height(WRAP).Colspan(4).DCard();
                })
                .Size(FILL).Weight(1)
                .Cells(4)
                .MeasureStart(builder =>
                {
                    var width = builder.View.Width;
                    builder.Cells(width >= 1280 ? 4 : width >= 768 ? 2 : 1);
                })
                .Padding(20)
                .Scrollable()
                .Space(20);
            });
        }


        private static XModify UserSumCard(UserSumData data)
        {
            return Column(() =>
            {
                Row(() =>
                {
                    Icon(data.IconId).Size(80).IconSize(50).Color(data.Color).Background(data.Color.Copy(0.5f)).Circle();
                    Text(() =>
                    {
                        Span(data.Title);
                        BreakLine();
                        Span(data.Value).H2();
                    }).TextAlignment(XAlignment.Center);
                }).Space(10).Width(FILL);

                Row(() =>
                {
                    Icon(data.IsUp ? SvgRes.Top : SvgRes.Bottom)
                    .Color(data.Color)
                    .IconSize(24);
                    Text(data.upNum).Color(data.Color);
                    Text(data.upTitle);
                }).Width(FILL).Space(10);
            }).Space(10).Padding(left: 30)
            .VerticalAlignment(XVerticalAlignment.Center);
        }



        private static XModify StudentGradesBarChart()
        {
            return Column(() =>
            {
                Row(() =>
                {
                    Text("学生成绩分布").FontSize(24);
                    Spacer().Weight(1);
                    Spacer(20)
                    .Background(XTheme.Color.Danger.Copy(0.5f))
                    .Border(XTheme.Color.Danger);
                    Text("学生人数");
                }).Space(10).Padding(10).Width(FILL);
                var visible = StateValueOf(true, true);
                var animateValue = AnimateFloatOf(visible, animate => animate.Duration = 800);
                Box(animateValue, value =>
                {
                    int[] yAxis = [100, 80, 60, 40, 20, 0];
                    YAxis([100, 80, 60, 40, 20, 0]);
                    float[] values = [12, 35, 86, 78, 60];
                    float[] newValues = new float[5];
                    for (int i = 0; i < newValues.Length; i++)
                    {
                        newValues[i] = values[i] * value;
                    }
                    string[] xLables = ["0-59", "60-69", "70-79", "80-89", "90-100"];

                    VerticalBars("学生人数", yAxis, xLables, newValues, (builder, i) =>
                    {
                        var color = i == 0 ? XTheme.Color.Danger : i == 1 ? XTheme.Color.WarningDark : i == 2 ? XTheme.Color.Warning : i == 3 ? XTheme.Color.Success : XTheme.Color.Primary;
                        builder
                        .Background(color.Copy(0.5f))
                        .HoverBackgroundColor(color.Copy(0.6f))
                        .Border(color, 1);
                    }).Margin(left: 40);
                    XAxis(xLables).Margin(left: 40);
                }).Weight(1);
            })
            .Padding(20)
            .HorizontalAlignment(XHorizontalAlignment.Left);
        }

        private static XModify AttendanceTrendLineChart()
        {
            return Column(() =>
            {
                var visible = StateValueOf(true, true);
                var animateValue = AnimateFloatOf(visible, animate => animate.Duration = 800);
                Row(() =>
                {
                    Text("出勤趋势").FontSize(24);
                    Spacer().Weight(1);
                    Spacer(20)
                    .Background(XTheme.Color.Success.Copy(0.5f))
                    .Border(XTheme.Color.Success);
                    Text("出勤人数");
                }).Space(10).Padding(10).Width(FILL);
                Box(animateValue, value =>
                {
                    int[] yAxis = [250, 240, 230, 220, 210, 200];
                    YAxis(yAxis);
                    float[] values = [242, 245, 240, 248, 238, 220, 200];
                    string[] xLables = ["周一", "周二", "周三", "周四", "周五", "周六", "周日"];

                    float[] newValues = new float[7];
                    for (int i = 0; i < newValues.Length; i++)
                    {
                        newValues[i] = 200 + (values[i] - 200) * value;
                    }
                    Lines(yAxis, newValues).Margin(left: 40);
                    Circels("出勤人数", yAxis, xLables, newValues).Margin(left: 40);
                    XAxis(xLables).Margin(left: 40);
                }).Weight(1).MeasureEnd(builder =>
                {
                    var height = builder.View.Height;
                });
            })
            .Padding(20)

            .HorizontalAlignment(XHorizontalAlignment.Left);
        }

        private static XModify MessageList()
        {
            var service = Service<UserService>();
            return Column(() =>
            {
                Row(() =>
                {
                    Text("最近活动").FontSize(24);
                    Spacer().Weight(1);
                    Text("查看全部").Color(XTheme.Color.Primary).HoverColor(XTheme.Color.PrimaryDark).HoverCursor(XCursorType.Hand);
                }).Space(10).Padding(10).Width(FILL);
                var activtyState = StateValueOf(service.Activitys);
                DataGrid(activtyState,
                [
                    new("活动类型", 200, v=> v.Type)
                    {
                        CellContent = (cell, data)=>
                        {
                            Row(() =>
                            {
                                var typeData = data.Type.ToType();
                                Icon(typeData.Item1).Size(40).IconSize(24).Background(typeData.Item2.Copy(0.5f)).Color(typeData.Item2).Circle();
                                 Text(typeData.Item3);
                            }).Space(10);
                        }
                    },
                    new("描述", -3, v => v.Desction),
                    new("操作人", -1, v => v.Name),
                    new("时间", -2, v => v.DateTime),
                    new("状态", 150, v => v.Status)
                    {
                        Alignment = XHorizontalAlignment.Center,
                        CellContent = (cell, data)=>
                        {
                            var color = data.Status == 3 ? XTheme.Color.Success : data.Status == 2 ? XTheme.Color.Warning : XTheme.Color.Danger;
                            string status = data.Status == 3 ? "已完成" : data.Status == 2 ? "处理中" : "待审批";
                            Text(status)
                            .Background(color.Copy(0.1f))
                            .Radius(30)
                            .Alignment(XAlignment.Center)
                            .Padding(10)
                            .Color(color);
                        }
                    },
                ]).Height(WRAP); // 高度自适应有问题
            }).Padding(20).HorizontalAlignment(XHorizontalAlignment.Left);
        }
    }
}
