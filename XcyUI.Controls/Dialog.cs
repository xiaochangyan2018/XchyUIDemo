using System;
using System.Runtime.CompilerServices;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public struct DialogInfo
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Action Cancel { get; set; }
        public Action Confirm { get; set; }
    }
    public static partial class Controls
    {
        private static XState<bool> visibleDialogState = new XState<bool>();
        private static DialogInfo dialogInfo = new DialogInfo();
        public static void ShowDialog(string title, string content, Action confirm = null, Action cancel = null)
        {
            dialogInfo = new DialogInfo() { Title = title, Content = content, Confirm = confirm, Cancel = cancel };
            visibleDialogState.Value = true;
        }
        public static void DialogView([CallerLineNumber] int key = 0)
        {
            PopupCard(visibleDialogState, card =>
            {
                card.OnMove((b, info) =>
                {
                    RenderImp.GetWindow().MoveWindow();
                });
                var visisbleState = StateValueOf(true);
                var isOut = StateValueOf(false);
                var animateValue = AnimateFloatOf(visisbleState, animate =>
                {
                    animate.OnFinished = () =>
                    {
                        if (isOut.Value)
                        {
                            visibleDialogState.Value = false;
                        }
                        isOut.Value = true;
                    };
                });
                Column(() =>
                {
                    Row(() =>
                    {
                        Icon(SvgRes.WarnTriangleFilled).Size(34).Color(XTheme.Color.Warning);
                        Text(dialogInfo.Title).H2();
                        Spacer(1).Weight(1);
                        Icon(SvgRes.Close).Size(48).IconSize(24).Radius(XTheme.Radius.Middle).Click(() =>
                        {
                            visisbleState.Value = true;
                        });
                    }).Width(FILL).Space(10);
                    Text(dialogInfo.Content);
                    Spacer(20);
                    Row(() =>
                    {
                        Text("取消").SubButton(() =>
                        {
                            dialogInfo.Cancel?.Invoke();
                            visisbleState.Value = true;
                        });
                        Text("确认").DangerButton(() =>
                        {
                            dialogInfo.Confirm?.Invoke();
                            visisbleState.Value = true;
                        });
                    }).Width(FILL).Space(15).HorizontalAlignment(XHorizontalAlignment.Right);
                })
                .InterceptEvent(XEventType.Move)
                .Size(WRAP).Space(10).MinWidth(400)
                .HorizontalAlignment(XHorizontalAlignment.Left)
                .Card().Padding(20)
                .Bind(animateValue, (builder, value) =>
                {
                    value = isOut.Value ? (1 - value) : value;
                    builder.Scale(value).Alpha(value);
                    card.View.RootView().ChildElemnt(0)?.Also(n => n.SetBlurSigma((int)(value * 8)));
                });
            },
            blurSigma: 8, key: key);
        }

        public static void DialogFormView(XState<bool> visibleFormState, Action<XState<bool>> content, [CallerLineNumber] int key = 0)
        {
            PopupCard(visibleFormState, card =>
            {
                card.OnMove((modify, info) =>
                {
                    RenderImp.GetWindow().MoveWindow();
                });
                var visisbleState = StateValueOf(true);
                var isOut = StateValueOf(false);
                var animateValue = AnimateFloatOf(visisbleState, animate =>
                {
                    animate.OnFinished = () =>
                    {
                        if (isOut.Value)
                        {
                            visibleFormState.Value = false;
                        }
                        isOut.Value = true;
                    };
                });
                Box(() =>
                {
                    content.Invoke(visisbleState);
                }).Size(WRAP)
                .Card()
                .Padding(0)
                .Bind(animateValue, (builder, value) =>
                {
                    value = isOut.Value ? (1 - value) : value;
                    builder.Scale(value).Alpha(value);
                    card.View.RootView().ChildElemnt(0)?.Also(n => n.SetBlurSigma((int)(value * 8)));

                });
            },
            blurSigma: 8, key: key);
        }
    }
}
