using TextCopy;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using XcyUI.Controls;
using static XcyUI.widgets.XCompose;
using static XcyUI.Controls.Controls;
using XcyDemo.Config;

namespace XcyDemo.Sample
{
    public struct ColorItem
    {
        public string Title { get; set; }
        public List<(XColor,string)> Color { get; set; }
        public ColorItem(string titile, List<(XColor,string)> color)
        {
            Title = titile;
            Color = color;
        }
    }
    public static class ThemePage
    {
        private static XThemeColorSonghuGreen greenColor = new();
        private static XThemeColorSonghuGreenDark greenDarkColor = new();
        private static XThemeColorTransformTeal transFormTeal = new();
        private static XThemeColorWaxPaper waxPaper = new();
        private static XThemeColorDreamyPurple dreamyPurple = new();
        private static XThemeColorSakuraPink sakuraPink = new();
        private static XThemeColorCocoaBrown cocoaBrown = new();
        private static XThemeColorMorandiGray moradiGray = new();
        private static XThemeColorSunsetOrange sunseOrange = new();
        private static List<ColorItem> colors => [
            new("主色调",[
                (XTheme.Color.Primary,"XTheme.Colors.Primary"),
                (XTheme.Color.PrimaryDark,"XTheme.Colors.PrimaryDark"),
                (XTheme.Color.PrimaryLight1,"XTheme.Colors.PrimaryLight1"),
                (XTheme.Color.PrimaryLight2,"XTheme.Colors.PrimaryLight2"),
                (XTheme.Color.PrimaryLight3,"XTheme.Colors.PrimaryLight3"),
                (XTheme.Color.PrimaryLight4,"XTheme.Colors.PrimaryLight4"),
                (XTheme.Color.PrimaryLight5,"XTheme.Colors.PrimaryLight5"),
                ]),
            new("辅助色",[
                (XTheme.Color.Success,"XTheme.Colors.Success"),
                (XTheme.Color.SuccessDark,"XTheme.Colors.SuccessDark"),
                (XTheme.Color.SuccessLight1,"XTheme.Colors.SuccessLight1"),
                (XTheme.Color.SuccessLight2,"XTheme.Colors.SuccessLight2"),
                (XTheme.Color.SuccessLight3,"XTheme.Colors.SuccessLight3"),
                (XTheme.Color.SuccessLight4,"XTheme.Colors.SuccessLight4"),
                (XTheme.Color.SuccessLight5,"XTheme.Colors.SuccessLight5"),
                ]),
            new("警告颜色",[
                (XTheme.Color.Warning,"XTheme.Colors.Warning"),
                (XTheme.Color.WarningDark,"XTheme.Colors.WarningDark"),
                (XTheme.Color.WarningLight1,"XTheme.Colors.WarningLight1"),
                (XTheme.Color.WarningLight2,"XTheme.Colors.WarningLight2"),
                (XTheme.Color.WarningLight3,"XTheme.Colors.WarningLight3"),
                (XTheme.Color.WarningLight4,"XTheme.Colors.WarningLight4"),
                (XTheme.Color.WarningLight5,"XTheme.Colors.WarningLight5"),
                ]),
            new("报警颜色",[
                (XTheme.Color.Danger,"XTheme.Colors.Danger"),
                (XTheme.Color.DangerDark,"XTheme.Colors.DangerDark"),
                (XTheme.Color.DangerLight1,"XTheme.Colors.DangerLight1"),
                (XTheme.Color.DangerLight2,"XTheme.Colors.DangerLight2"),
                (XTheme.Color.DangerLight3,"XTheme.Colors.DangerLight3"),
                (XTheme.Color.DangerLight4,"XTheme.Colors.DangerLight4"),
                (XTheme.Color.DangerLight5,"XTheme.Colors.DangerLight5"),
                ]),
            new("信息颜色",[
                (XTheme.Color.Info,"XTheme.Colors.Info"),
                (XTheme.Color.InfoDark,"XTheme.Colors.InfoDark"),
                (XTheme.Color.InfoLight1,"XTheme.Colors.InfoLight1"),
                (XTheme.Color.InfoLight2,"XTheme.Colors.InfoLight2"),
                (XTheme.Color.InfoLight3,"XTheme.Colors.InfoLight3"),
                (XTheme.Color.InfoLight4,"XTheme.Colors.InfoLight4"),
                (XTheme.Color.InfoLight5,"XTheme.Colors.InfoLight5"),
                ]),
            new("文本颜色",[
                (XTheme.Color.PrimaryText,"XTheme.Colors.PrimaryText"),
                (XTheme.Color.RegularText,"XTheme.Colors.RegularText"),
                (XTheme.Color.SecondaryText,"XTheme.Colors.SecondaryText"),
                (XTheme.Color.PlaceholderText,"XTheme.Colors.PlaceholderText"),
                (XTheme.Color.DisabledText,"XTheme.Colors.DisabledText")
                ]),
             new("边框颜色",[
                (XTheme.Color.DarkerBorder,"XTheme.Colors.DarkerBorder"),
                (XTheme.Color.DarkBorder,"XTheme.Colors.DarkBorder"),
                (XTheme.Color.BaseBorder,"XTheme.Colors.BaseBorder"),
                (XTheme.Color.LightBorder,"XTheme.Colors.LightBorder"),
                (XTheme.Color.LighterBorder,"XTheme.Colors.LighterBorder"),
                (XTheme.Color.ExtraLightBorder,"XTheme.Colors.ExtraLightBorder"),
                ]),
            new("填充颜色",[
                (XTheme.Color.DarkerFill,"XTheme.Colors.DarkerFill"),
                (XTheme.Color.DarkFill,"XTheme.Colors.DarkFill"),
                (XTheme.Color.BaseFill,"XTheme.Colors.BaseFill"),
                (XTheme.Color.LightFill,"XTheme.Colors.LightFill"),
                (XTheme.Color.LighterFill,"XTheme.Colors.LighterFill"),
                (XTheme.Color.ExtraLightFill,"XTheme.Colors.ExtraLightFill"),
                (XTheme.Color.BlankFill,"XTheme.Colors.BlankFill"),
                ]),

             new("其他颜色",[
                (XTheme.Color.Black,"XTheme.Colors.Black"),
                (XTheme.Color.White,"XTheme.Colors.White"),
                (XTheme.Color.Transparent,"XTheme.Colors.Transparent"),
                (XTheme.Color.DarkBackground,"XTheme.Colors.DarkBackground"),
                (XTheme.Color.Background,"XTheme.Colors.Background"),
                (XTheme.Color.OnBackground,"XTheme.Colors.OnBackground"),
                ]),
            ];
        private static XModify SelectStyle(this XModify builder, bool selected)
        {
            return builder
                .Padding(20,10)
                .Radius(XTheme.Radius.Middle)
                .Color(selected ? XTheme.Color.White : XTheme.Color.PrimaryText)
                .Background(selected ? XTheme.Color.PrimaryLight1 : XColor.Empty);
        }
        public static XModify View()
        {
            return Column(() =>
            {
                Flow(XTheme.DarkModeState, isDark =>
                {
                    Text("白天").SelectStyle(XTheme.Color== XTheme.Light).Click(()=> XTheme.ApplyThemeColor(XTheme.Light, false));

                    Text("晚上").SelectStyle(XTheme.Color == XTheme.Dark).Click(() => XTheme.ApplyThemeColor(XTheme.Dark, true));
                    Text("松湖绿").SelectStyle(XTheme.Color == greenColor).Click(() => XTheme.ApplyThemeColor(greenColor));
                    Text("深绿色").SelectStyle(XTheme.Color == greenDarkColor).Click(() => XTheme.ApplyThemeColor(greenDarkColor, true));
                    Text("变革蓝绿").SelectStyle(XTheme.Color == transFormTeal).Click(() => XTheme.ApplyThemeColor(transFormTeal));
                    Text("蜡纸色").SelectStyle(XTheme.Color == waxPaper).Click(() => XTheme.ApplyThemeColor(waxPaper));
                    Text("梦幻紫").SelectStyle(XTheme.Color == dreamyPurple).Click(() => XTheme.ApplyThemeColor(dreamyPurple));
                    Text("樱花粉").SelectStyle(XTheme.Color == sakuraPink).Click(() => XTheme.ApplyThemeColor(sakuraPink));
                    Text("可可棕").SelectStyle(XTheme.Color == cocoaBrown).Click(() => XTheme.ApplyThemeColor(cocoaBrown));
                    Text("莫兰迪灰").SelectStyle(XTheme.Color == moradiGray).Click(() => XTheme.ApplyThemeColor(moradiGray));
                    Text("落日橙").SelectStyle(XTheme.Color == sunseOrange).Click(() => XTheme.ApplyThemeColor(sunseOrange));

                }).Size(FILL, WRAP).Space(20).Padding(20);

                Column(() =>
                {
                    foreach (var item in colors)
                    {
                        Text(item.Title).H3();
                        Row(() =>
                        {
                            foreach (var item in item.Color)
                            {
                                Spacer().Size(100, 80)
                                .Background(item.Item1)
                                .DefaultBorder()
                                .Click(()=>
                                {
                                    ClipboardService.SetText(item.Item2);
                                    ShowToast($"已复制 {item.Item2}", SvgRes.SuccessFilled);
                                }, false);
                            }
                        }).Size(FILL, WRAP).HorizontalAlignment(XHorizontalAlignment.Bisect);
                    }
                    
                })
                .Weight(1).Space(20)
                .Padding(horizontal:20, vertical: 10)
                .Scrollable()
                .HorizontalAlignment(XHorizontalAlignment.Left);
            })
            .Space(20)
            .Padding(10)
            .HorizontalAlignment(XHorizontalAlignment.Left);
        }
    }
}
