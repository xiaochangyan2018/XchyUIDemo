using System;
using XcyUI.models;

namespace XcyUI.Components.Utils
{
    public class ColorUtils
    {
        public static void ColorToHsv(XColor color, out float h, out float s, out float v)
        {
            float r = color.Red/255f;
            float g = color.Green/255f;
            float b = color.Blue/255f;

            float max = Math.Max(Math.Max(r, g), b);
            float min = Math.Min(Math.Min(r, g), b);
            float delta = max - min;

            h = 0;
            s = 0;
            v = max;

            if (delta > 0.0001f)
            {
                s = delta / max;

                if (max == r)
                    h = 60 * ((g - b) / delta);
                else if (max == g)
                    h = 60 * (2 + (b - r) / delta);
                else // max == b
                    h = 60 * (4 + (r - g) / delta);

                if (h < 0) h += 360;
            }
        }

        /// <summary>
        /// HSV 转 NanoVG颜色
        /// </summary>
        public static XColor HsvToColor(float h, float s, float v)
        {
            h = ClampAngle(h);
            s = Clamp01(s);
            v = Clamp01(v);

            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60f) % 2 - 1));
            float m = v - c;

            float r = 0, g = 0, b = 0;

            if (h >= 0 && h < 60)
            { r = c; g = x; b = 0; }
            else if (h >= 60 && h < 120)
            { r = x; g = c; b = 0; }
            else if (h >= 120 && h < 180)
            { r = 0; g = c; b = x; }
            else if (h >= 180 && h < 240)
            { r = 0; g = x; b = c; }
            else if (h >= 240 && h < 300)
            { r = x; g = 0; b = c; }
            else if (h >= 300 && h < 360)
            { r = c; g = 0; b = x; }

            return new XColor(
                (byte)((r + m)*255),
                (byte)((g + m) * 255),
                (byte)((b + m)*255)
            );
        }

        public static void PointToSV(float x, float y, float width, float height, out float s, out float v)
        {
            float px = x;
            float py = y;

            s = Math.Clamp(px / width, 0, 1);
            v = Math.Clamp(1 - py / height, 0, 1);
        }
        public static XPoint SVToPoint(float s, float v, XRect rect)
        {
            float x = rect.Left + s * rect.Width;
            float y = rect.Top + (1 - v) * rect.Height;
            return new XPoint((int)x, (int)y);
        }

        public static float YToHue(float y, float height)
        {
            return Math.Clamp(y / height * 360f, 0, 360);
        }

        public static float HueToY(float h, float height)
        {
            return h / 360f * height;
        }

        private static float Clamp01(float f) => Math.Clamp(f, 0, 1);
        private static float ClampAngle(float h) => Math.Clamp(h, 0, 360);
    }
}
