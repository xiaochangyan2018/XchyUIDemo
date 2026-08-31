using System;
using System.Collections.Generic;
using System.Text;
using XcyUI.models;
using XcyUI.theme;

namespace XcyDemo.Config
{
    public class XThemeColorTransformTeal : XThemeColor
    {
        public XThemeColorTransformTeal()
        {
            // 主色调 - 变革蓝绿
            Primary = new XColor(26, 94, 82);
            PrimaryDark = new XColor(18, 75, 65);
            PrimaryLight1 = new XColor(78, 130, 120);
            PrimaryLight2 = new XColor(120, 160, 152);
            PrimaryLight3 = new XColor(168, 196, 190);
            PrimaryLight4 = new XColor(198, 218, 214);
            PrimaryLight5 = new XColor(228, 240, 238);

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

            Info = new XColor(138, 152, 150);
            InfoDark = new XColor(110, 124, 122);
            InfoLight1 = new XColor(165, 178, 176);
            InfoLight2 = new XColor(192, 202, 200);
            InfoLight3 = new XColor(212, 220, 218);
            InfoLight4 = new XColor(228, 234, 232);
            InfoLight5 = new XColor(240, 244, 243);

            // 文本色
            PrimaryText = new XColor(48, 49, 51);
            RegularText = new XColor(96, 98, 102);
            SecondaryText = new XColor(144, 147, 153);
            PlaceholderText = new XColor(168, 171, 178);
            DisabledText = new XColor(192, 196, 204);

            // 边框色 - 蓝绿调
            DarkerBorder = new XColor(192, 210, 206);
            DarkBorder = new XColor(202, 218, 214);
            BaseBorder = new XColor(212, 224, 222);
            LightBorder = new XColor(220, 230, 228);
            LighterBorder = new XColor(228, 236, 234);
            ExtraLightBorder = new XColor(236, 242, 240);

            // 填充色 - 蓝绿调
            DarkerFill = new XColor(212, 228, 225);
            DarkFill = new XColor(220, 234, 231);
            BaseFill = new XColor(228, 238, 236);
            LightFill = new XColor(234, 242, 240);
            LighterFill = new XColor(240, 246, 244);
            ExtraLightFill = new XColor(244, 248, 247);
            BlankFill = new XColor(248, 251, 250);

            Black = XColors.Black;
            White = XColors.White;
            Transparent = XColors.Transparent;
            DarkBackground = new XColor(224, 238, 236);
            Background = new XColor(238, 246, 245);
            OnBackground = new XColor(246, 250, 249);
        }
    }
}
