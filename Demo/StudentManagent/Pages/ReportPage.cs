using XcyDemo.StudentManagent.Services;
using XcyUI.Controls;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static XcyDemo.StudentManagent.Components.XCommonComponents;
using static XcyDemo.StudentManagent.Theme.XStyleBuilder;
using static XcyUI.Controls.Controls;
using static XcyUI.widgets.XDIWidget;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Demo.StudentManagent.Pages
{
    public class ReportPage
    {
        public static XModify View()
        {
            return Column(() =>
            {
                TopBar("数据统计");
                Flow(() =>
                {
                    StudentGradesBarChart().Height(350).DCard();
                    ClassGradesBarChart().Height(350).DCard();
                    AttendanceTrendLineChart().Height(350).DCard();
                    StudentRaderChart().Height(350).DCard();
                    GradesReport().Colspan(2).DCard();
                })
                .Size(FILL)
                .Weight(1)
                .Cells(2)
                .MeasureStart(builder =>
                {
                    var width = builder.View.Width;
                    builder.Cells(width >= 1280 ? 2 : 1);
                })
                .Padding(20)
                .Scrollable()
                .Space(20);
            });
        }

        private static XModify StudentGradesBarChart()
        {
            return Column(() =>
            {
                string[] xLables = ["0-59", "60-69", "70-79", "80-89", "90-100"];

                XColor[] colors = [XTheme.Color.Danger, XTheme.Color.WarningDark, XTheme.Color.Warning, XTheme.Color.Success, XTheme.Color.Primary];

                float[] values = [12, 35, 86, 78, 60];

                Text("学生成绩分布").FontSize(24);
                Row(() =>
                {
                    Column(() =>
                    {
                        for (int i = 0; i < xLables.Length; i++)
                        {
                            Row(() =>
                            {
                                Spacer(20).Background(colors[i].Copy(0.5f)).Border(colors[i], 1);
                                Text(xLables[i].ToString());
                            }).Space(10);
                        }
                    }).Size(WRAP, FILL)
                    .Padding(20).Space(10)
                    .HorizontalAlignment(XHorizontalAlignment.Left)
                    .VerticalAlignment(XVerticalAlignment.Center);

                    var visible = StateValueOf(true);
                    var animateValue = AnimateFloatOf(visible, animate => animate.Duration = 500);
                    Box(animateValue, value =>
                    {
                        var sum = values.Sum();
                        float[] newValues = new float[5];
                        for (int i = 0; i < newValues.Length; i++)
                        {
                            newValues[i] = i == values.Length - 1 ? sum + (values[i] - sum) * value : values[i] * value;
                        }
                        PieChart("学习成绩", xLables, newValues, colors);
                    }).Weight(1);

                }).Width(FILL).Weight(1);
            })
            .Padding(20)
            .HorizontalAlignment(XHorizontalAlignment.Left);
        }

        private static XModify ClassGradesBarChart()
        {
            return Column(() =>
            {
                Row(() =>
                {
                    Text("各班成绩对比").FontSize(24);
                    Spacer().Weight(1);
                    Spacer(20)
                    .Background(XTheme.Color.Primary.Copy(0.5f))
                    .Border(XTheme.Color.Primary, 1);
                    Text("平均分");
                }).Space(10).Padding(10).Width(FILL);
                var visible = StateValueOf(true);
                var animateValue = AnimateFloatOf(visible, animate => animate.Duration = 500);
                Box(animateValue, value =>
                {
                    float[] values = [82, 78, 90, 76, 84];
                    float[] newValues = new float[5];
                    for (int i = 0; i < newValues.Length; i++)
                    {
                        newValues[i] = 60 + (values[i] - 60) * value;
                    }
                    string[] xLables = ["计算机001", "计算机002", "计算机003", "计算机004", "计算机005"];
                    Box(() =>
                    {
                        int[] yAxis = [100, 95, 90, 85, 80, 75, 70, 65, 60];
                        YAxis(yAxis);
                        VerticalBars("平均分", yAxis, xLables, newValues).Margin(left: 40);
                    }).Margin(bottom: 20);

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
                var visible = StateValueOf(true);
                var animateValue = AnimateFloatOf(visible, animate => animate.Duration = 500);
                Row(() =>
                {
                    Text("出勤趋势").FontSize(24);
                    Spacer().Weight(1);
                    Spacer(20)
                    .Background(XTheme.Color.Success.Copy(0.5f))
                    .Border(XTheme.Color.Success, 1);
                    Text("出勤人数");
                }).Space(10).Padding(10).Width(FILL);
                Box(animateValue, value =>
                {
                    int[] yAxis = [250, 240, 230, 220, 210, 200];
                    YAxis(yAxis);
                    float[] values = [242, 245, 240, 248, 238, 220, 200];
                    object[] xLables = ["周一", "周二", "周三", "周四", "周五", "周六", "周日"];

                    float[] newValues = new float[7];
                    for (int i = 0; i < newValues.Length; i++)
                    {
                        newValues[i] = 200 + (values[i] - 200) * value;
                    }
                    LineArea(yAxis, newValues).Margin(left: 40);

                    XAxis(xLables).Margin(left: 40);
                }).Weight(1).MeasureEnd(builder =>
                {
                    var height = builder.View.Height;
                });
            })
            .Padding(20)

            .HorizontalAlignment(XHorizontalAlignment.Left);
        }

        private static XModify StudentRaderChart()
        {
            return Column(() =>
            {
                var visible = StateValueOf(true);
                var animateValue = AnimateFloatOf(visible, animate => animate.Duration = 500);
                Row(() =>
                {
                    Text("学生综合表现").FontSize(24);
                    Spacer().Weight(1);
                    Spacer(20)
                    .Background(XTheme.Color.Primary.Copy(0.5f))
                    .Border(XTheme.Color.Primary, 2);
                    Text("班级平均");
                }).Space(10).Padding(10).Width(FILL);

                Box(animateValue, value =>
                {
                    RadarChart(
                        title: "班级平均",
                        yAxis: [100, 90, 80, 70, 60],
                        xAxis: ["高等数学", "大学物理", "计算机基础", "英语", "思想政治"],
                        values: [78, 75, 82, 77, 80]).Margin(5).Scale(value);
                }).Weight(1);
            })
            .Padding(20)

            .HorizontalAlignment(XHorizontalAlignment.Left);
        }

        private static XModify GradesReport()
        {
            var service = Service<UserService>();
            return Column(() =>
            {
                Row(() =>
                {
                    Text("成绩详情统计").FontSize(24);
                    Spacer().Weight(1);
                    Text("查看全部").Color(XTheme.Color.Primary).HoverColor(XTheme.Color.PrimaryDark).HoverCursor(XCursorType.Hand);
                }).Space(10).Padding(10).Width(FILL);
                Spacer(20);

                var itemsState = StateValueOf(service.GradesReportItems);
                DataGrid(itemsState, new()
                {
                    new("科目", -1, v => v.SubjectName),
                    new("参考人数", -1, v => v.UserCount),
                    new("平均分", -1, v => v.AverageScore),
                    new("最高分", -1, v => v.MaxScore),
                    new("最低分", -1, v => v.MinScore),
                    new("及格率", -1, v => v.PassRate),
                    new("优秀率", -1, v => v.ExcellenceRate)
                }).Height(WRAP);
            })
            .Height(WRAP)
            .Padding(20)
            .HorizontalAlignment(XHorizontalAlignment.Left);
        }
    }
}
