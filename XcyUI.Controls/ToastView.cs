using System.Runtime.CompilerServices;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public struct ToastInfo
    {
        public int ResId { get; set; }
        public string Message { get; set; }
    }
    public static partial class Controls
    {
        private static XState<bool> visibleToastState = new XState<bool>();
        private static ToastInfo toastInfo = new ToastInfo();
        public static void ShowToast(string message, int resId = 0)
        {
            RenderImp.PostToQueue(() =>
            {
                toastInfo = new ToastInfo() { Message = message, ResId = resId };
                visibleToastState.Value = true;
            });
        }
        public static void ToastView([CallerLineNumber] int key = 0)
        {
            PopupCard(visibleToastState, builder =>
            {
                builder.View.Key = "toast_"+builder.View.Key;
                var visisbleState = StateValueOf(true);
                var isOut = StateValueOf(false);
                var animateValue = AnimateFloatOf(visisbleState, animate =>
                {
                    animate.Duration = 500;
                    animate.OnFinished = () =>
                    {
                        if (!isOut.Value)
                        {
                            visisbleState.Value = false;
                            isOut.Value = true;
                            XTask.RunDelayed(() =>
                            {
                                visisbleState.Value = true;
                            }, 2000);
                        }
                        else
                        {
                            visibleToastState.Value = false;
                        }
                    };
                });
                Row(() =>
                {
                    var resId = toastInfo.ResId == 0? SvgRes.InfoFilled: toastInfo.ResId;
                    Icon(resId).Size(24).Color(XTheme.Color.Success);
                    Text(toastInfo.Message).Color(XTheme.Color.Success);
                })
                .Space(10)
                .Padding(15)
                .Alignment(XAlignment.TopCenter)
                .Background(XTheme.Color.SuccessLight5)
                .Radius(XTheme.Radius.Low)
                .Shadow(XTheme.Shadow.MinCard)
                .Bind(animateValue, (b, value) =>
                {
                    b.Translate(-1, isOut.Value ? (40 - 40 * value) : 40 * value).Alpha(isOut.Value ? (1 - value) : value);
                });
            },
            disableOutClick: false,
            outSideClick: (_, info) => visibleToastState.Value = false,
            key: key);
        }
    }
}
