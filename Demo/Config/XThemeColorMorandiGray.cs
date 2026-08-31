using System;
using System.Collections.Generic;
using System.Text;
using XcyUI.models;
using XcyUI.theme;

namespace XcyDemo.Config
{
    public class XThemeColorMorandiGray : XThemeColor
    {
        public XThemeColorMorandiGray()
        {
            // 主色调 - 莫兰迪灰
            Primary = new XColor(138, 142, 148);
            PrimaryDark = new XColor(110, 114, 120);
            PrimaryLight1 = new XColor(165, 169, 175);
            PrimaryLight2 = new XColor(188, 192, 198);
            PrimaryLight3 = new XColor(210, 213, 218);
            PrimaryLight4 = new XColor(225, 228, 232);
            PrimaryLight5 = new XColor(240, 242, 245);

            // 辅助色（莫兰迪低饱和版）
            Success = new XColor(130, 170, 120);
            SuccessDark = new XColor(105, 140, 96);
            SuccessLight1 = new XColor(165, 195, 155);
            SuccessLight2 = new XColor(190, 212, 182);
            SuccessLight3 = new XColor(212, 226, 206);
            SuccessLight4 = new XColor(226, 236, 222);
            SuccessLight5 = new XColor(238, 245, 236);

            Warning = new XColor(200, 160, 110);
            WarningDark = new XColor(168, 134, 92);
            WarningLight1 = new XColor(220, 188, 148);
            WarningLight2 = new XColor(234, 208, 175);
            WarningLight3 = new XColor(242, 224, 200);
            WarningLight4 = new XColor(247, 234, 216);
            WarningLight5 = new XColor(251, 243, 232);

            Danger = new XColor(205, 130, 130);
            DangerDark = new XColor(170, 105, 105);
            DangerLight1 = new XColor(225, 165, 165);
            DangerLight2 = new XColor(238, 195, 195);
            DangerLight3 = new XColor(245, 218, 218);
            DangerLight4 = new XColor(250, 232, 232);
            DangerLight5 = new XColor(253, 243, 243);

            Info = new XColor(144, 147, 153);
            InfoDark = new XColor(115, 118, 122);
            InfoLight1 = new XColor(172, 175, 180);
            InfoLight2 = new XColor(196, 198, 202);
            InfoLight3 = new XColor(218, 220, 223);
            InfoLight4 = new XColor(232, 233, 235);
            InfoLight5 = new XColor(243, 244, 245);

            // 文本色
            PrimaryText = new XColor(50, 52, 56);
            RegularText = new XColor(96, 98, 102);
            SecondaryText = new XColor(144, 147, 153);
            PlaceholderText = new XColor(168, 171, 178);
            DisabledText = new XColor(192, 196, 204);

            // 边框色 - 莫兰迪灰调
            DarkerBorder = new XColor(200, 203, 208);
            DarkBorder = new XColor(208, 211, 216);
            BaseBorder = new XColor(216, 219, 224);
            LightBorder = new XColor(223, 226, 230);
            LighterBorder = new XColor(230, 232, 236);
            ExtraLightBorder = new XColor(237, 239, 242);

            // 填充色 - 莫兰迪灰调
            DarkerFill = new XColor(218, 220, 224);
            DarkFill = new XColor(224, 226, 230);
            BaseFill = new XColor(230, 232, 236);
            LightFill = new XColor(236, 238, 242);
            LighterFill = new XColor(241, 243, 246);
            ExtraLightFill = new XColor(245, 247, 249);
            BlankFill = new XColor(249, 250, 252);

            Black = XColors.Black;
            White = XColors.White;
            Transparent = XColors.Transparent;
            DarkBackground = new XColor(226, 228, 232);
            Background = new XColor(240, 242, 245);
            OnBackground = new XColor(247, 248, 250);
        }
    }
}
