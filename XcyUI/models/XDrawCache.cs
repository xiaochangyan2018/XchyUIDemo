using System;

namespace XcyUI.models
{
    public class XDrawCache
    {
        public IDisposable CacheData;
        public bool EnableCache;
        public int BlurSigma;
        public bool IsRefreshCache;
        public bool CacheShadow;
        public XCacheType CacheType = XCacheType.Pictrue;

        public float Alpha = -1;
        public float ScaleX = -1;
        public float ScaleY = -1;
        public XPoint ScalePoint;
        public float Degrees = -1;
        public XPoint DegreesPoint;
        public int TranslateX=-1;
        public int TranslateY=-1;

        public void Clear()
        {
            CacheData?.Dispose();
            CacheData = null;
        }
    }

    public enum XCacheType
    {
        Bitmap,
        Pictrue
    }
}
