using XcyUI.models;
using XcyUI.theme;

namespace XcyDemo.Config
{
    public class XThemeColorWaxPaper : XThemeColor
    {
        public XThemeColorWaxPaper()
        {
            // 主色调 - 蜡纸色
            Primary = new XColor(208, 192, 160);
            PrimaryDark = new XColor(184, 168, 136);
            PrimaryLight1 = new XColor(220, 206, 180);
            PrimaryLight2 = new XColor(230, 218, 196);
            PrimaryLight3 = new XColor(238, 228, 212);
            PrimaryLight4 = new XColor(244, 236, 224);
            PrimaryLight5 = new XColor(250, 245, 238);

            // 辅助色
            Success = new XColor(103, 194, 58);
            SuccessDark = new XColor(82, 155, 46);
            SuccessLight1 = new XColor(149, 212, 117);
            SuccessLight2 = new XColor(179, 225, 157);
            SuccessLight3 = new XColor(209, 237, 196);
            SuccessLight4 = new XColor(225, 243, 216);
            SuccessLight5 = new XColor(240, 249, 235);

            Warning = new XColor(230, 162, 60);
            WarningDark = new XColor(184, 130, 48);
            WarningLight1 = new XColor(238, 190, 119);
            WarningLight2 = new XColor(243, 209, 158);
            WarningLight3 = new XColor(248, 227, 197);
            WarningLight4 = new XColor(250, 236, 216);
            WarningLight5 = new XColor(253, 246, 236);

            Danger = new XColor(245, 108, 108);
            DangerDark = new XColor(196, 86, 86);
            DangerLight1 = new XColor(248, 152, 152);
            DangerLight2 = new XColor(250, 182, 182);
            DangerLight3 = new XColor(252, 211, 211);
            DangerLight4 = new XColor(253, 226, 226);
            DangerLight5 = new XColor(254, 240, 240);

            Info = new XColor(160, 156, 148);
            InfoDark = new XColor(130, 126, 120);
            InfoLight1 = new XColor(185, 182, 175);
            InfoLight2 = new XColor(205, 203, 198);
            InfoLight3 = new XColor(222, 220, 216);
            InfoLight4 = new XColor(234, 233, 230);
            InfoLight5 = new XColor(244, 243, 241);

            // 文本色
            PrimaryText = new XColor(60, 56, 50);
            RegularText = new XColor(100, 96, 90);
            SecondaryText = new XColor(145, 142, 136);
            PlaceholderText = new XColor(170, 168, 162);
            DisabledText = new XColor(195, 193, 188);

            // 边框色 - 暖米调
            DarkerBorder = new XColor(215, 208, 195);
            DarkBorder = new XColor(222, 216, 206);
            BaseBorder = new XColor(228, 223, 214);
            LightBorder = new XColor(234, 230, 222);
            LighterBorder = new XColor(240, 237, 230);
            ExtraLightBorder = new XColor(245, 242, 237);

            // 填充色 - 暖米调
            DarkerFill = new XColor(228, 222, 210);
            DarkFill = new XColor(234, 229, 220);
            BaseFill = new XColor(240, 236, 228);
            LightFill = new XColor(244, 241, 235);
            LighterFill = new XColor(248, 245, 240);
            ExtraLightFill = new XColor(250, 248, 244);
            BlankFill = new XColor(252, 250, 248);

            Black = XColors.Black;
            White = XColors.White;
            Transparent = XColors.Transparent;
            DarkBackground = new XColor(232, 226, 214);
            Background = new XColor(245, 242, 235);
            OnBackground = new XColor(250, 248, 243);
        }
    }
}
