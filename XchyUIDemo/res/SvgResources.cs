using Svg.Skia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using XchyUI.theme;
using XchyUI.utils;

namespace XchyUIDemo.res
{
    public class SvgResources
    {
        public static void Load()
        {
            XThemeManager.SvgResources = new Dictionary<int, object>()
            {

                [CircleProgress] = "<svg t=\"1768307668534\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" p-id=\"8448\" data-spm-anchor-id=\"a313x.search_index.0.i5.3bb43a81RqoIC9\" width=\"48\" height=\"48\"><path d=\"M511.999086 36.573192a475.427722 475.427722 0 0 0-336.163971 811.591693A475.427722 475.427722 0 0 0 848.163057 175.836943 472.319157 472.319157 0 0 0 511.999086 36.573192m0-36.571363A511.999086 511.999086 0 1 1 0 512.000914 511.999086 511.999086 0 0 1 511.999086 0.001829z\" fill=\"#dbdbdb\" p-id=\"8449\" data-spm-anchor-id=\"a313x.search_index.0.i2.3bb43a81RqoIC9\" class=\"\"></path><path d=\"M1023.230173 541.623719h-36.571363c0.585142-9.215984 0.877713-19.199966 0.877712-29.695947a472.209442 472.209442 0 0 0-139.22718-336.163972A472.172871 472.172871 0 0 0 512.181943 36.53662v-36.571363a508.744234 508.744234 0 0 1 362.056496 149.94259 508.341949 508.341949 0 0 1 149.942589 362.056496c0 9.947411-0.292571 19.931393-0.841141 29.659376z\" fill=\"#1296db\" p-id=\"8450\" data-spm-anchor-id=\"a313x.search_index.0.i4.3bb43a81RqoIC9\" class=\"selected\"></path></svg>",
            };
            SKSvg svg = new();
            foreach (var item in XThemeManager.SvgResources.Keys)
            {
                XThemeManager.SvgResources[item] = svg.FromSvg(svg: XThemeManager.SvgResources[item].ToString());
            }
            RenderImp.Invalidate();
        }

        public static int CircleProgress = Guid.NewGuid().GetHashCode();
    }
}
