using System;
using System.Threading;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.utils;
using XcyUI.views;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XModify IconButton(int resId, string text, bool isVerticel = false)
        {
            void Content()
            {
                Icon(resId).Size(20);
                Text(text);
            }
            return (isVerticel ? Column(Content) : Row(Content)).PrimaryButton().Space(10);
        }

        public static XModify FisrtIcon(this XModify builder, Action<XModify> func)
        {
            var isFirst = false;
            builder.View.ModifyChild(n =>
            {
                if(!isFirst && n is XIcon)
                {
                    isFirst = true;
                    func.Invoke(new XModify(n));
                }
            });
            return builder;
        }
        public static XModify AsyncButton(string text,string loadingText, Action asyncFun, Action<XState<bool>> preFunc = null)
        {
            var isStartAsyncState = StateValueOf(false);
            return Box(isStartAsyncState, isStart =>
            {
                if (isStart)
                {
                    Row(() =>
                    {
                        Icon(SvgRes.Loading).Size(20).CircleAnim();
                        Text(loadingText);
                    })
                    .Space(10).PrimaryButton().Width(FILL)
                    .HorizontalAlignment(XHorizontalAlignment.Center)
                    .FisrtIcon(n => n.Color(new XBrush(XColors.White, XColors.White.Copy(0), XGradientDirection.Round)));

                    XTask.Run(() =>
                    {
                        Thread.Sleep(1000);
                        asyncFun.Invoke();
                        isStartAsyncState.Value = false;
                    });
                }
                else
                {
                    Text(text).Width(FILL).PrimaryButton(() =>
                    {
                        if (preFunc != null)
                        {
                            preFunc.Invoke(isStartAsyncState);
                        }
                        else
                        {
                            isStartAsyncState.Value = true;
                        }
                    });
                }
            }).Size(WRAP);
        }
    }
}
