using System;
using System.Collections.Generic;
using System.Text;
using XcyDemo.StudentManagent.Common.Models;
using XcyUI.Controls;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;
using static XcyUI.Controls.Controls;
using static XcyDemo.Compoment.Compoment;
using XcyUI.models;
using XcyUI.GLFW;
using XcyUI.Desktop;

namespace XcyDemo.Sample
{
    public static class MuiltPage
    {
        public static XModify View()
        {
            return Column(() =>
            {
                var visibleState = StateValueOf(false);
                Form(visibleState);
                Row(() =>
                {
                    Text("窗口内弹窗").PrimaryButton(() =>
                    {
                        visibleState.Value = true;
                    });
                    Text("打开新窗口").PrimaryButton(() =>
                    {
                        Desktop.OpenWindow(1, new XWindowParams()
                        {
                            Width = 800,
                            Height = 800,
                            Compose = () =>
                            {
                                Text("新窗口").H1();
                            }
                        });
                    });
                    Text("打开模态窗口").PrimaryButton(() =>
                    {
                        Desktop.OpenWindow(2, new XWindowParams()
                        {
                            Width = 800,
                            Height = 800,
                            Modal = true,
                            Compose = () =>
                            {
                                Text("模态窗口").H1();
                            }
                        });
                    });
                    Text("打开无标题窗口").PrimaryButton(() =>
                    {
                        Desktop.OpenWindow(3, new XWindowParams()
                        {
                            Width = 800,
                            Height = 800,
                            HideTitleBar = true,
                            Compose = () =>
                            {
                                Column(() =>
                                {
                                    Text("无标题窗口").H1();
                                    Text("关闭").PrimaryButton(() =>
                                    {
                                        XApplication.CloseWindow();
                                    });
                                }).Space(20)
                                .VerticalAlignment(XVerticalAlignment.Center)
                                .OnMove((modify, info) => XApplication.MoveWindow());

                            }
                        });
                    });
                    Text("打开悬浮透明窗口").PrimaryButton(() =>
                    {
                        Desktop.OpenWindow(4, new XWindowParams()
                        {
                            //HideTitleBar = true,
                            Decorated = false,
                            IsTransparent = true,
                            Floating = true,
                            Width = 800,
                            Height = 800,
                            Compose = () =>
                            {
                                Column(() =>
                                {
                                    Row(() =>
                                    {
                                        CloseButton();
                                    }).Size(FILL, 60).HorizontalAlignment(XHorizontalAlignment.Center);

                                    Text("悬浮透明窗口").Tabindex(100).SubButton(() =>
                                    {
                                    }).Alignment(XAlignment.Center);
                                })
                                .Space(10)
                                .OnMove((modify, info) => XApplication.MoveWindow())
                                .Card()
                                .Shadow(new XShadow(0, 0, XColors.Red, 24))
                                .VerticalAlignment(XVerticalAlignment.Center)
                                .Clip()
                                .Circle()
                                .FadeIn()
                                .Padding(0)
                                .Margin(48);
                            }
                        });
                    });
                }).Space(10);
            }).Padding(10).Space(10).HorizontalAlignment(XHorizontalAlignment.Left);
        }
        public static void Form(XState<bool> visibleState)
        {
            DialogFormView(visibleState, state =>
            {
                FormSample.View(state);
            });
        }
    }
}
