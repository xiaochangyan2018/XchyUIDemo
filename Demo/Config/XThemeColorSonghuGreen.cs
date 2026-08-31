using System;
using System.Collections.Generic;
using System.Text;
using XcyUI.models;
using XcyUI.theme;

namespace XcyDemo.Config
{
    public class XThemeColorSonghuGreen : XThemeColor
    {
        public XThemeColorSonghuGreen()
        {
            // 主色调 - 松湖绿
            Primary = new XColor(0, 150, 105);
            PrimaryDark = new XColor(0, 120, 84);
            PrimaryLight1 = new XColor(77, 182, 150);
            PrimaryLight2 = new XColor(128, 203, 180);
            PrimaryLight3 = new XColor(179, 224, 210);
            PrimaryLight4 = new XColor(204, 234, 225);
            PrimaryLight5 = new XColor(230, 245, 240);

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

            Info = new XColor(144, 147, 153);
            InfoDark = new XColor(115, 118, 122);
            InfoLight1 = new XColor(177, 179, 184);
            InfoLight2 = new XColor(200, 201, 204);
            InfoLight3 = new XColor(222, 223, 224);
            InfoLight4 = new XColor(233, 233, 235);
            InfoLight5 = new XColor(244, 244, 245);

            // 文本色
            PrimaryText = new XColor(48, 49, 51);
            RegularText = new XColor(96, 98, 102);
            SecondaryText = new XColor(144, 147, 153);
            PlaceholderText = new XColor(168, 171, 178);
            DisabledText = new XColor(192, 196, 204);

            // 边框色 - 松湖绿调
            DarkerBorder = new XColor(196, 224, 210);
            DarkBorder = new XColor(206, 230, 218);
            BaseBorder = new XColor(214, 234, 224);
            LightBorder = new XColor(222, 238, 230);
            LighterBorder = new XColor(228, 242, 235);
            ExtraLightBorder = new XColor(234, 246, 240);

            // 填充色 - 松湖绿调
            DarkerFill = new XColor(216, 240, 228);
            DarkFill = new XColor(224, 244, 234);
            BaseFill = new XColor(230, 247, 238);
            LightFill = new XColor(236, 250, 243);
            LighterFill = new XColor(240, 252, 246);
            ExtraLightFill = new XColor(244, 253, 248);
            BlankFill = new XColor(248, 254, 251);

            Black = XColors.Black;
            White = XColors.White;
            Transparent = XColors.Transparent;
            DarkBackground = new XColor(220, 242, 232);
            Background = new XColor(234, 248, 240);
            OnBackground = new XColor(242, 251, 246);
        }
    }
}
