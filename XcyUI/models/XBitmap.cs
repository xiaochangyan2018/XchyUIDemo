
using System;

namespace XcyUI.models
{
    public class XBitmap
    {
        public int Width;
        public int Height;
        public IDisposable Cache;
        public byte[] Buffers;
    }

    public enum XScaleType
    {
        Normal,
        FixXY,
        FixCenter
    }
}
