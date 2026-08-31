using System;
using System.Collections.Generic;
using System.Text;
using XcyUI.models;
using XcyUI.theme;

namespace XcyDemo.Config
{
    public class XThemeColorSonghuGreenDark : XThemeColor
    {
        public XThemeColorSonghuGreenDark()
        {
            Primary = new XColor(46, 204, 150);
            PrimaryDark = new XColor(28, 175, 126);
            PrimaryLight1 = new XColor(82, 220, 173);
            PrimaryLight2 = new XColor(120, 230, 190);
            PrimaryLight3 = new XColor(160, 240, 210);
            PrimaryLight4 = new XColor(195, 245, 225);
            PrimaryLight5 = new XColor(225, 250, 240);

            // 辅助色
            Success = new XColor(103, 194, 58);
            SuccessDark = new XColor(82, 155, 46);
            SuccessLight1 = new XColor(140, 208, 100);
            SuccessLight2 = new XColor(168, 218, 135);
            SuccessLight3 = new XColor(196, 228, 170);
            SuccessLight4 = new XColor(218, 236, 198);
            SuccessLight5 = new XColor(236, 246, 226);

            Warning = new XColor(230, 162, 60);
            WarningDark = new XColor(184, 130, 48);
            WarningLight1 = new XColor(240, 185, 95);
            WarningLight2 = new XColor(245, 205, 130);
            WarningLight3 = new XColor(248, 222, 168);
            WarningLight4 = new XColor(250, 234, 198);
            WarningLight5 = new XColor(252, 244, 228);

            Danger = new XColor(245, 108, 108);
            DangerDark = new XColor(196, 86, 86);
            DangerLight1 = new XColor(250, 140, 140);
            DangerLight2 = new XColor(252, 170, 170);
            DangerLight3 = new XColor(253, 200, 200);
            DangerLight4 = new XColor(254, 220, 220);
            DangerLight5 = new XColor(254, 238, 238);

            Info = new XColor(130, 150, 140);
            InfoDark = new XColor(100, 120, 110);
            InfoLight1 = new XColor(155, 175, 165);
            InfoLight2 = new XColor(180, 197, 188);
            InfoLight3 = new XColor(205, 218, 210);
            InfoLight4 = new XColor(225, 235, 228);
            InfoLight5 = new XColor(240, 246, 242);

            // 文本色（深色模式反白）
            PrimaryText = new XColor(255, 255, 255);
            RegularText = new XColor(218, 226, 222);
            SecondaryText = new XColor(165, 180, 172);
            PlaceholderText = new XColor(130, 148, 138);
            DisabledText = new XColor(95, 112, 102);

            // 边框色 - 深绿色调
            DarkerBorder = new XColor(68, 100, 84);
            DarkBorder = new XColor(58, 90, 75);
            BaseBorder = new XColor(50, 82, 68);
            LightBorder = new XColor(42, 74, 60);
            LighterBorder = new XColor(36, 66, 54);
            ExtraLightBorder = new XColor(30, 58, 46);

            // 填充色 - 深绿色调
            DarkerFill = new XColor(58, 92, 76);
            DarkFill = new XColor(50, 82, 68);
            BaseFill = new XColor(42, 74, 60);
            LightFill = new XColor(36, 66, 54);
            LighterFill = new XColor(30, 58, 46);
            ExtraLightFill = new XColor(26, 52, 40);
            BlankFill = new XColor(22, 46, 36);

            Black = XColors.Black;
            White = XColors.White;
            Transparent = XColors.Transparent;
            DarkBackground = new XColor(18, 44, 34);
            Background = new XColor(24, 54, 42);
            OnBackground = new XColor(32, 64, 50);
        }
    }
}
