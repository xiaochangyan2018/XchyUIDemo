using XcyUI.expansions;
using XcyUI.theme;

namespace XcyUI.models
{
    public struct XColor
    {
        public static readonly XColor Empty;

        public XColor(byte red, byte green, byte blue)
        {
            Green = green;
            Red = red;
            Blue = blue;
            Alpha = 255;
        }
        public XColor(byte red, byte green, byte blue, byte alpha)
        {
            Green = green;
            Red = red;
            Blue = blue;
            Alpha = alpha;
        }
        public byte Green;
        public byte Red;
        public byte Alpha;
        public byte Blue;
        public bool IsEmpty => Green == 0 && Red == 0 && Blue == 0 && Alpha == 0;

        public XColor Copy(float alpha) => new XColor(Red, Green, Blue, (byte)(Alpha * alpha));

        public XColor Copy(byte alpha) => new XColor(Red, Green, Blue, alpha);

        public int Value => (int)(((uint)Alpha << 24) | ((uint)Red << 16) | ((uint)Green << 8) | (uint)Blue);

        public string Hex => "#" + Alpha.ToString("X").PadLeft(2, '0') + Red.ToString("X").PadLeft(2, '0') + Green.ToString("X").PadLeft(2, '0') + Blue.ToString("X").PadLeft(2, '0');
    }
    public struct XBrush
    {
        public static readonly XBrush Empty;
        public XColor StartColor;
        public XColor EndColor;
        public XGradientDirection Direction;
        public bool IsEmpty => StartColor.IsEmpty && EndColor.IsEmpty;
        public XBrush(XColor start)
        {
            StartColor = start;
            EndColor = XColor.Empty;
            Direction = XGradientDirection.Horizontal;
        }
        
        public XBrush(XColor start, XColor end,XGradientDirection direction)
        {
            StartColor = start;
            EndColor = end;
            Direction = direction;
        }
        public XBrush Copy(XColor startColor)
        {
            var brush = this;
            brush.StartColor = startColor;
            return brush;
        }

        public XBrush Copy(float alpha)
        {
            var brush = this;
            brush.StartColor = StartColor.Copy(alpha);
            if (!brush.EndColor.IsEmpty)
            {
                brush.EndColor = EndColor.Copy(alpha);
            }
            return brush;
        }
    }

    public enum XGradientDirection
    {
        Horizontal,
        Vertical,
        DiagonalBottom,
        DiagonalTop,
        Radial,
        Round
    }

    public struct XBorder
    {
        public readonly static XBorder Empty;
        public XBrush Color;
        public XSpace Size;
        public XDashType DashType;
        public XBorder(XBrush color, XSpace size, XDashType type)
        {
            Color = color;
            Size = size;
            DashType = type;
        }
        public XBorder Copy(XColor color)
        {
            var border = this;
            border.Color = new XBrush() { StartColor = color };
            return border;
        }
    }

    public struct XShadow
    {
        public static XShadow Empty;
        public bool IsEmpty => Blur == 0 || Color.IsEmpty;
        public int Dx;
        public int Dy;
        public XColor Color;
        public int Blur;
        public bool Inset;
        public XShadow(int x,int y,XColor color, int blur)
        {
            Dx = x;
            Dy = y;
            Color = color;
            Blur = blur;
            Inset = false;
        }
        public int ShadowHashCode()
        {
            return (Dx, Dy, Color, Blur, Inset).GetHashCode();
        }
    }
    public enum XDashType
    {
        Solid,
        Dash,
        Dot,
        DashDot
    }

    public class XFont
    {
        public XFont()
        {
            Name = XTheme.DefaultFontName;
            Color = new XBrush()
            {
                StartColor = XTheme.Color.PrimaryText
            };
            Size = XTheme.Size.Body.AsPx();
            Weight = XTheme.Weight.Middle;
        }
        public string Path;
        public string Name;
        public XBrush Color;
        public int Size;
        public float Weight;
        public int LineHeight;
        public bool Italic;
        public bool Underline;
        public bool DeleteLine;

        public int FontHasCode()
        {
            return (Path, Name, Color, Size, Weight, Italic, Underline, DeleteLine).GetHashCode();
        }
        public XFont Copy()
        {
            return new XFont()
            {
                Path = Path,
                Name = Name,
                Color = Color,
                Size = Size,
                Weight = Weight,
                Italic = Italic,
            };
        }
    }
}
