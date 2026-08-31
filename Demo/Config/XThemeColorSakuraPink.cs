using System;
using System.Collections.Generic;
using System.Text;
using XcyUI.models;
using XcyUI.theme;

namespace XcyDemo.Config
{
    public class XThemeColorSakuraPink : XThemeColor
    {
        public XThemeColorSakuraPink()
        {
            // 主色调 - 樱花粉
            Primary = new XColor(255, 140, 160);
            PrimaryDark = new XColor(230, 112, 133);
            PrimaryLight1 = new XColor(255, 170, 185);
            PrimaryLight2 = new XColor(255, 195, 205);
            PrimaryLight3 = new XColor(255, 218, 225);
            PrimaryLight4 = new XColor(255, 232, 237);
            PrimaryLight5 = new XColor(255, 243, 246);

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

            Info = new XColor(155, 147, 152);
            InfoDark = new XColor(125, 118, 123);
            InfoLight1 = new XColor(180, 173, 178);
            InfoLight2 = new XColor(202, 197, 200);
            InfoLight3 = new XColor(222, 218, 220);
            InfoLight4 = new XColor(234, 232, 233);
            InfoLight5 = new XColor(244, 243, 244);

            // 文本色
            PrimaryText = new XColor(48, 49, 51);
            RegularText = new XColor(96, 98, 102);
            SecondaryText = new XColor(144, 147, 153);
            PlaceholderText = new XColor(168, 171, 178);
            DisabledText = new XColor(192, 196, 204);

            // 边框色 - 粉调
            DarkerBorder = new XColor(220, 208, 212);
            DarkBorder = new XColor(226, 216, 220);
            BaseBorder = new XColor(232, 224, 227);
            LightBorder = new XColor(237, 230, 233);
            LighterBorder = new XColor(242, 236, 238);
            ExtraLightBorder = new XColor(246, 242, 244);

            // 填充色 - 粉调
            DarkerFill = new XColor(232, 220, 224);
            DarkFill = new XColor(238, 228, 232);
            BaseFill = new XColor(242, 234, 237);
            LightFill = new XColor(246, 240, 242);
            LighterFill = new XColor(249, 244, 246);
            ExtraLightFill = new XColor(251, 247, 248);
            BlankFill = new XColor(253, 250, 251);

            Black = XColors.Black;
            White = XColors.White;
            Transparent = XColors.Transparent;
            DarkBackground = new XColor(240, 230, 234);
            Background = new XColor(250, 244, 246);
            OnBackground = new XColor(253, 249, 250);
        }
    }
}
