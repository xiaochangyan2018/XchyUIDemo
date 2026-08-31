using XcyUI.models;
using XcyUI.theme;

namespace XcyDemo.Config
{
    public class XThemeColorSunsetOrange : XThemeColor
    {
        public XThemeColorSunsetOrange()
        {
            // 主色调 - 落日橙
            Primary = new XColor(250, 140, 60);
            PrimaryDark = new XColor(220, 112, 40);
            PrimaryLight1 = new XColor(252, 170, 110);
            PrimaryLight2 = new XColor(254, 195, 150);
            PrimaryLight3 = new XColor(255, 218, 185);
            PrimaryLight4 = new XColor(255, 232, 208);
            PrimaryLight5 = new XColor(255, 243, 230);

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

            Info = new XColor(153, 148, 142);
            InfoDark = new XColor(122, 118, 114);
            InfoLight1 = new XColor(180, 176, 172);
            InfoLight2 = new XColor(202, 199, 196);
            InfoLight3 = new XColor(222, 220, 218);
            InfoLight4 = new XColor(234, 233, 231);
            InfoLight5 = new XColor(244, 243, 242);

            // 文本色
            PrimaryText = new XColor(48, 49, 51);
            RegularText = new XColor(96, 98, 102);
            SecondaryText = new XColor(144, 147, 153);
            PlaceholderText = new XColor(168, 171, 178);
            DisabledText = new XColor(192, 196, 204);

            // 边框色 - 暖橙调
            DarkerBorder = new XColor(220, 208, 196);
            DarkBorder = new XColor(226, 216, 206);
            BaseBorder = new XColor(232, 224, 216);
            LightBorder = new XColor(237, 230, 224);
            LighterBorder = new XColor(242, 236, 230);
            ExtraLightBorder = new XColor(246, 242, 238);

            // 填充色 - 暖橙调
            DarkerFill = new XColor(232, 216, 200);
            DarkFill = new XColor(238, 226, 214);
            BaseFill = new XColor(242, 234, 226);
            LightFill = new XColor(246, 240, 234);
            LighterFill = new XColor(249, 244, 240);
            ExtraLightFill = new XColor(251, 248, 245);
            BlankFill = new XColor(253, 251, 249);

            Black = XColors.Black;
            White = XColors.White;
            Transparent = XColors.Transparent;
            DarkBackground = new XColor(240, 228, 216);
            Background = new XColor(250, 244, 238);
            OnBackground = new XColor(253, 250, 247);
        }
    }
}
