using System;
using System.Collections.Generic;
using System.Text;
using XcyUI.models;

namespace XcyUI.theme
{
    public class XThemeColor
    {
        // 主色调
        public XColor Primary = new XColor(64,158,255);
        public XColor PrimaryDark = new XColor(51, 126, 204); 
        public XColor PrimaryLight1 = new XColor(121, 187, 255); 
        public XColor PrimaryLight2 = new XColor(160, 207, 255);
        public XColor PrimaryLight3 = new XColor(198, 226, 255);
        public XColor PrimaryLight4 = new XColor(217, 236, 255);
        public XColor PrimaryLight5 = new XColor(236, 245, 255);

        // 辅助色
        public XColor Success = new XColor(103, 194, 58);
        public XColor SuccessDark = new XColor(82, 155, 46);
        public XColor SuccessLight1 = new XColor(149, 212, 117);
        public XColor SuccessLight2 = new XColor(179, 225, 157);
        public XColor SuccessLight3 = new XColor(209, 237, 196);
        public XColor SuccessLight4 = new XColor(225, 243, 216);
        public XColor SuccessLight5 = new XColor(240, 249, 235);

        public XColor Warning = new XColor(230, 162, 60);
        public XColor WarningDark = new XColor(184, 130, 48);
        public XColor WarningLight1 = new XColor(238, 190, 119);
        public XColor WarningLight2 = new XColor(243, 209, 158);
        public XColor WarningLight3 = new XColor(248, 227, 197);
        public XColor WarningLight4 = new XColor(250, 236, 216);
        public XColor WarningLight5 = new XColor(253, 246, 236);

        public XColor Danger = new XColor(245, 108, 108);
        public XColor DangerDark = new XColor(196, 86, 86);
        public XColor DangerLight1 = new XColor(248, 152, 152);
        public XColor DangerLight2 = new XColor(250, 182, 182);
        public XColor DangerLight3 = new XColor(252, 211, 211);
        public XColor DangerLight4 = new XColor(253, 226, 226);
        public XColor DangerLight5 = new XColor(254, 240, 240);

        public XColor Info = new XColor(144, 147, 153);
        public XColor InfoDark = new XColor(115, 118, 122);
        public XColor InfoLight1 = new XColor(177, 179, 184);
        public XColor InfoLight2 = new XColor(200, 201, 204);
        public XColor InfoLight3 = new XColor(115, 118, 122);
        public XColor InfoLight4 = new XColor(233, 233, 235);
        public XColor InfoLight5 = new XColor(244, 244, 245);

        // 文本色
        public XColor PrimaryText = new XColor(48, 49, 51);
        public XColor RegularText = new XColor(96, 98, 102);
        public XColor SecondaryText = new XColor(144, 147, 153);
        public XColor PlaceholderText = new XColor(168, 171, 178);
        public XColor DisabledText = new XColor(192, 196, 204);

        public XColor DarkerBorder = new XColor(205, 208, 214);
        public XColor DarkBorder = new XColor(212, 215, 222);
        public XColor BaseBorder = new XColor(220, 223, 230);
        public XColor LightBorder = new XColor(228, 231, 237);
        public XColor LighterBorder = new XColor(235, 238, 245);
        public XColor ExtraLightBorder = new XColor(242, 246, 252);

        public XColor DarkerFill = new XColor(230, 232, 235);
        public XColor DarkFill = new XColor(235, 237, 240);
        public XColor BaseFill = new XColor(240, 242, 245);
        public XColor LightFill = new XColor(245, 247, 250);
        public XColor LighterFill = new XColor(250, 250, 250);
        public XColor ExtraLightFill = new XColor(250, 252, 255);
        public XColor BlankFill = new XColor(255, 255, 255);

        public XColor Black = XColors.Black;
        public XColor White = XColors.White;
        public XColor Transparent = XColors.Transparent;
        public XColor DarkBackground = new XColor(242, 243, 245);
        public XColor Background = new XColor(255, 255, 255);
        public XColor OnBackground = new XColor(255, 255, 255);

        // 交互状态色
        private XColor _actionColor => XTheme.DarkModeState.Value ? XColors.White : XColors.Black;
        public XColor Hover => _actionColor.Copy(0.06f);
        public XColor Pressed => _actionColor.Copy(0.12f);
        public XColor Selected => _actionColor.Copy(0.03f);

        public float DisabledAlpha = 0.5f;        
    }

    public class XThemeDarkColors : XThemeColor
    {
       public XThemeDarkColors()
        {
            // 主色
            Primary = new XColor(64, 158, 255);
            PrimaryDark = new XColor(64, 158, 255);
            PrimaryLight1 = new XColor(51, 126, 204);
            PrimaryLight2 = new XColor(40, 99, 163);
            PrimaryLight3 = new XColor(30, 74, 122);
            PrimaryLight4 = new XColor(20, 50, 82);
            PrimaryLight5 = new XColor(10, 25, 41);

            // 成功色
            Success = new XColor(103, 194, 58);
            SuccessDark = new XColor(103, 194, 58);
            SuccessLight1 = new XColor(82, 155, 46);
            SuccessLight2 = new XColor(61, 116, 34);
            SuccessLight3 = new XColor(41, 78, 23);
            SuccessLight4 = new XColor(20, 39, 11);
            SuccessLight5 = new XColor(10, 20, 5);


            // 警告色
            Warning = new XColor(230, 162, 60);
            WarningDark = new XColor(230, 162, 60);
            WarningLight1 = new XColor(184, 130, 48);
            WarningLight2 = new XColor(138, 98, 36);
            WarningLight3 = new XColor(92, 65, 24);
            WarningLight4 = new XColor(46, 33, 12);
            WarningLight5 = new XColor(23, 16, 6);

            // 危险色
            Danger = new XColor(245, 108, 108);
            DangerDark = new XColor(245, 108, 108);
            DangerLight1 = new XColor(196, 86, 86);
            DangerLight2 = new XColor(147, 65, 65);
            DangerLight3 = new XColor(98, 43, 43);
            DangerLight4 = new XColor(49, 22, 22);
            DangerLight5 = new XColor(25, 11, 11);

            // 信息色
            Info = new XColor(177, 179, 184);
            InfoDark = new XColor(144, 147, 153);
            InfoLight1 = new XColor(115, 118, 122);
            InfoLight2 = new XColor(86, 89, 92);
            InfoLight3 = new XColor(57, 60, 63);
            InfoLight4 = new XColor(38, 40, 43);
            InfoLight5 = new XColor(28, 30, 33);

            // 文本色（黑夜模式专用）
            PrimaryText = new XColor(255, 255, 255);
            RegularText = new XColor(224, 224, 224);
            SecondaryText = new XColor(189, 189, 189);
            PlaceholderText = new XColor(120, 120, 120);
            DisabledText = new XColor(100, 100, 100);

            // 边框色
            DarkerBorder = new XColor(80, 80, 80);
            DarkBorder = new XColor(68, 68, 68);
            BaseBorder = new XColor(56, 56, 56);
            LightBorder = new XColor(44, 44, 44);
            LighterBorder = new XColor(32, 32, 32);
            ExtraLightBorder = new XColor(20, 20, 20);

            // 背景填充色
            DarkerFill = new XColor(18, 18, 18);
            DarkFill = new XColor(24, 24, 24);
            BaseFill = new XColor(30, 30, 30);
            LightFill = new XColor(36, 36, 36);
            LighterFill = new XColor(42, 42, 42);
            ExtraLightFill = new XColor(48, 48, 48);
            BlankFill = new XColor(54, 54, 54);

            // 页面背景（黑夜模式核心）
            DarkBackground = new XColor(18, 18, 18);
            Background = new XColor(24, 24, 24);
            OnBackground = new XColor(30, 30, 30);
        }
    }

    public class XThemeSizes
    {
        public int H1 = 32;   // 主标题（保留你原来的，高分屏刚好）
        public int H2 = 26;   // 区块标题
        public int H3 = 22;   // 子标题
        public int Body = 18; // 正文（核心：16→18，高分屏最清晰）
        public int Caption = 16; // 辅助文字
        public int Small = 14; // 标签小字
        public int ScrollbarWidth = 10;
        public int ScrollbarMinHeight = 30;
        public int ScrollbarMargin = 2;
        public int Border = 1;
        public int Space10 = 10;
        public int Space12 = 12;
        public int Space16 = 16;
        public int Space20 = 20;
        public int Space24 = 24;
        public int Space32 = 32;
    }

    public class XThemeWeights
    {
        public float Large = 700f;
        public float Middle = 500f;
        public float Low = 300f;
    }

    public class XThemeRadius
    {
        public int Large = 12;
        public int Middle = 8;
        public int Low = 6;
        public float Circle = 0.5f;
    }

    public class XThemeShadows
    {

        public XShadow Card = new XShadow()
        {
            Dy = 2,
            Blur = 24,
            Color = XColors.Black.Copy(0.2f)
        };

        public XShadow MinCard = new XShadow()
        {
            Dy = 2,
            Blur = 6,
            Color = XColors.Black.Copy(0.08f)
        };

        public XShadow Input = new XShadow()
        {
            Blur = 2,
            Color = new XColor(74, 144, 226).Copy(0.2f)
        };

        public XShadow Button = new XShadow()
        {
            Dx = 2,
            Dy = 2,
            Blur = 8,
            Color = XColors.Black.Copy(0.06f)
        };

        public XShadow PrimaryButton = new XShadow()
        {
            Dx = 2,
            Dy = 2,
            Blur = 8,
            Color = new XColor(74, 144, 226).Copy(0.06f)
        };

        public XShadow ButtonHover = new XShadow()
        {
            Dx = 2,
            Dy = 2,
            Blur = 8,
            Color = XColors.Black.Copy(0.18f)
        };

        public XShadow ButtonPress = new XShadow()
        {
            Dx = 2,
            Dy = 2,
            Blur = 8,
            Color = XColors.Black.Copy(0.18f),
            Inset = true
        };
    }
}
