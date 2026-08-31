using XcyDemo.Animate;
using XcyDemo.Config;
using XcyDemo.Images;
using XcyDemo.Sample;
using XcyUI.Demo.StudentManagent.Pages;
using XcyUI.expansions;
using XcyUI.GLFW;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.utils;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyDemo.Compoment.Compoment;
using static XcyUI.widgets.XCompose;

namespace XcyDemo
{
    public static class MainPage
    {
        private static XState<bool> pageAnimateVisibleStae = new();
        public static void View()
        {
            Column(() =>
            {
                TopBar();
                Box(Router.RouterState, router =>
                {
                    if (router == Router.StudentManagent || Router.PreRouterId == Router.StudentManagent)
                    {
                        StudentMainPage.View().ZIndex(router == Router.StudentManagent ? 2 : 0).PageAnimate(Router.StudentManagent);
                    }
                    if (router == Router.DataGrid || Router.PreRouterId == Router.DataGrid)
                    {
                        DataGridTest.View().ZIndex(router == Router.DataGrid ? 2 : 0).PageAnimate(Router.DataGrid);
                    }
                    if (router == Router.MuiltWindow || Router.PreRouterId == Router.MuiltWindow)
                    {
                        MuiltPage.View().ZIndex(router == Router.MuiltWindow ? 2 : 0).PageAnimate(Router.MuiltWindow);
                    }
                    if (router == Router.XTheme || Router.PreRouterId == Router.XTheme)
                    {
                        ThemePage.View().ZIndex(router == Router.XTheme ? 2 : 0).PageAnimate(Router.XTheme);
                    }
                    if (router == Router.Resources || Router.PreRouterId == Router.Resources)
                    {
                        ResourcePage.View().ZIndex(router == Router.Resources ? 2 : 0).PageAnimate(Router.Resources);
                    }
                }).Weight(1).Tabindex(100);
            }).Background(XTheme.Color.DarkBackground);
        }

        public static XModify PageAnimate(this XModify modify, int routerId)
        {
            var animateValue = AnimateFloatOf(pageAnimateVisibleStae, a =>
            {
                a.Duration = 500;
            });
            
            return modify.Bind(animateValue, (modiy, value) =>
            {
                if (!pageAnimateVisibleStae.Value) return;
                if (routerId == Router.PreRouterId)
                {
                    modiy.Alpha(-1).BlurSigma((int)(18 * (value)));
                    if (value > 0.98)
                    {
                        modiy.View.Removed();
                        Router.PreRouterId = -1;
                    }
                }
                else
                {
                    var width = modiy.View.Width;
                    var isRight = Router.RouterState.Value < Router.PreRouterId;
                    modiy.BlurSigma(0).Alpha(value).Translate(isRight ? width * (1 - value) : -(width * (1 - value)));
                    if (value > 0.98)
                    {
                        modiy.EnableCache(false);
                    }
                }
            })
            .Also(n =>
            {
                if (routerId == Router.RouterState.Value && Router.PreRouterId != -1 && pageAnimateVisibleStae.Value)
                {
                    n.Alpha(0);
                }
            });
        }

        private static void TopBar()
        {
            Row(() =>
            {
                Spacer(20);
                Icon(ImgRes.Logo).Size(30).Color(XColor.Empty);
                Spacer(20);
                Text("XcyUI示例").FontSize(XTheme.Size.H3);
                Spacer().Weight(1);
                Box(() =>
                {
                    var rectState = StateValueOf(XRect.Empty);
                    var animateStart = StateValueOf(false);
                    Row(() =>
                    {
                        foreach (var item in Router.Routers)
                        {
                            Text(item.Item2)
                            .Height(FILL)
                            .Padding(10)
                            .Radius(XTheme.Radius.Low)
                            .FontWeight(XTheme.Weight.Middle)
                            .LayoutEnd(n =>
                            {
                                if (item.Item1 == Router.RouterState.Value)
                                {
                                    rectState.Value = n.View.RenderRect;
                                }
                            })
                            .TextAlignment(XAlignment.Center)
                            .Click((modify, info) =>
                            {
                                rectState.Value = modify.View.RenderRect;
                                animateStart.Value = true;
                                if (Router.RouterState.Value != item.Item1)
                                {
                                    pageAnimateVisibleStae.Value = false;
                                    pageAnimateVisibleStae.Post(true);
                                    Router.PreRouterId = Router.RouterState.Value;
                                    Router.RouterState.Value = item.Item1;
                                }
                            });
                        }
                    }).Space(10).Height(FILL);

                    Spacer().Size(WRAP, 2)
                    .Alignment(XAlignment.LeftBottom)
                    .Background(XTheme.Color.PrimaryText)
                    .AnimateWidthTo(animateStart, rectState, XAlignment.BottomCenter);

                }).Size(WRAP, FILL).Padding(5);

                //Spacer().Weight(1);

                MinimizeButton();
                MaximizeButton();
                CloseButton();
            })
            .Size(FILL, 52)
            .DoubleClick((modify, info) => XApplication.ToggleMaximize(), false)
            .OnMove((modify, info) => XApplication.MoveWindow());
        }
    }
}
