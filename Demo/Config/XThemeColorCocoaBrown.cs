using System;
using System.Collections.Generic;
using System.Text;
using XcyUI.models;
using XcyUI.theme;

namespace XcyDemo.Config
{
    public class XThemeColorCocoaBrown : XThemeColor
    {
        public XThemeColorCocoaBrown()
        {
            // 主色调 - 可可棕
            Primary = new XColor(139, 105, 20);
            PrimaryDark = new XColor(110, 84, 16);
            PrimaryLight1 = new XColor(170, 135, 60);
            PrimaryLight2 = new XColor(196, 165, 98);
            PrimaryLight3 = new XColor(218, 195, 145);
            PrimaryLight4 = new XColor(232, 215, 178);
            PrimaryLight5 = new XColor(245, 234, 215);

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

            Info = new XColor(150, 145, 135);
            InfoDark = new XColor(120, 116, 108);
            InfoLight1 = new XColor(178, 173, 165);
            InfoLight2 = new XColor(202, 198, 192);
            InfoLight3 = new XColor(222, 219, 215);
            InfoLight4 = new XColor(234, 232, 229);
            InfoLight5 = new XColor(244, 242, 240);

            // 文本色
            PrimaryText = new XColor(50, 45, 35);
            RegularText = new XColor(95, 90, 80);
            SecondaryText = new XColor(145, 140, 130);
            PlaceholderText = new XColor(170, 165, 158);
            DisabledText = new XColor(195, 192, 185);

            // 边框色 - 棕调
            DarkerBorder = new XColor(210, 200, 180);
            DarkBorder = new XColor(218, 210, 194);
            BaseBorder = new XColor(225, 218, 205);
            LightBorder = new XColor(232, 226, 215);
            LighterBorder = new XColor(238, 233, 224);
            ExtraLightBorder = new XColor(244, 240, 233);

            // 填充色 - 棕调
            DarkerFill = new XColor(222, 212, 192);
            DarkFill = new XColor(228, 220, 204);
            BaseFill = new XColor(234, 228, 215);
            LightFill = new XColor(240, 235, 225);
            LighterFill = new XColor(244, 240, 233);
            ExtraLightFill = new XColor(248, 245, 240);
            BlankFill = new XColor(251, 249, 246);

            Black = XColors.Black;
            White = XColors.White;
            Transparent = XColors.Transparent;
            DarkBackground = new XColor(228, 218, 198);
            Background = new XColor(242, 236, 224);
            OnBackground = new XColor(249, 246, 240);
        }
    }
}
