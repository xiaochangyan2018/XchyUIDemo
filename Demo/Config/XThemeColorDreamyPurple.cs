using System;
using System.Collections.Generic;
using System.Text;
using XcyUI.models;
using XcyUI.theme;

namespace XcyDemo.Config
{
    public class XThemeColorDreamyPurple : XThemeColor
    {
        public XThemeColorDreamyPurple()
        {
            // 主色调 - 梦幻紫
            Primary = new XColor(124, 92, 255);
            PrimaryDark = new XColor(99, 74, 204);
            PrimaryLight1 = new XColor(158, 133, 255);
            PrimaryLight2 = new XColor(185, 165, 255);
            PrimaryLight3 = new XColor(210, 196, 255);
            PrimaryLight4 = new XColor(228, 218, 255);
            PrimaryLight5 = new XColor(242, 236, 255);

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

            Info = new XColor(150, 145, 165);
            InfoDark = new XColor(120, 116, 132);
            InfoLight1 = new XColor(178, 173, 190);
            InfoLight2 = new XColor(202, 198, 212);
            InfoLight3 = new XColor(222, 219, 230);
            InfoLight4 = new XColor(234, 232, 240);
            InfoLight5 = new XColor(244, 242, 248);

            // 文本色
            PrimaryText = new XColor(48, 49, 51);
            RegularText = new XColor(96, 98, 102);
            SecondaryText = new XColor(144, 147, 153);
            PlaceholderText = new XColor(168, 171, 178);
            DisabledText = new XColor(192, 196, 204);

            // 边框色 - 紫调
            DarkerBorder = new XColor(205, 200, 220);
            DarkBorder = new XColor(212, 208, 226);
            BaseBorder = new XColor(220, 216, 232);
            LightBorder = new XColor(226, 224, 238);
            LighterBorder = new XColor(232, 230, 242);
            ExtraLightBorder = new XColor(238, 237, 246);

            // 填充色 - 紫调
            DarkerFill = new XColor(222, 216, 238);
            DarkFill = new XColor(228, 224, 242);
            BaseFill = new XColor(234, 230, 246);
            LightFill = new XColor(238, 236, 248);
            LighterFill = new XColor(242, 240, 250);
            ExtraLightFill = new XColor(246, 245, 252);
            BlankFill = new XColor(250, 249, 254);

            Black = XColors.Black;
            White = XColors.White;
            Transparent = XColors.Transparent;
            DarkBackground = new XColor(232, 228, 246);
            Background = new XColor(242, 240, 252);
            OnBackground = new XColor(248, 246, 254);
        }
    }
}
