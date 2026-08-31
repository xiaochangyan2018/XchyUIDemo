using XcyUI.expansions;
using XcyUI.models;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;
using static XcyUI.widgets.XDIWidget;
using XcyUI.theme;
using static XcyUI.Controls.Controls;
using XcyDemo.StudentManagent.Pages;
using XcyDemo.StudentManagent.State;
using XcyUI.Controls;
using XcyDemo.StudentManagent.Services;
using XcyDemo.StudentManagent.Common.Models;
using System.Diagnostics;

namespace XcyUI.Demo.StudentManagent.Pages
{
    public static class StudentMainPage
    {
        private static XState<bool> menuState = new();
        public static XModify View()
        {
            return Box(() =>
            {
                Row(() =>
                {
                    Menus();
                    Box(StateRepository.SelectedMenuItem, menu =>
                    {
                        var id = menu.Id == 0 ? 1 : menu.Id;
                        switch (id)
                        {
                            case 1:
                                HomePage.View();
                                break;
                            case 2:
                                StudentListPage.View().FadeIn();
                                break;
                            case 3:
                                GradesPage.View().FadeIn();
                                break;
                            case 4:
                                StudentListPage.OtherSimple("出勤管理").FadeIn();
                                break;
                            case 5:
                                ReportPage.View();
                                break;
                            case 6:
                                var colorState = StateValueOf(XColors.Red);
                                ColorPicker(colorState.Value, color =>
                                {
                                    colorState.Value = color;
                                });
                                break;
                        }
                    })
                    .Weight(1).Margin(10).Margin(left: 0)
                    .Background(XTheme.Color.LighterFill)
                    .Radius(XTheme.Radius.Large).Clip();
                }).Size(FILL)
                .Background(XTheme.Color.LighterBorder);

                Box(menuState, isShow =>
                {
                    if (isShow)
                    {
                        LeftMenus();
                    }
                }).ContentAlignment(XAlignment.LeftTop);
            });
        }

        private static XModify LeftMenus()
        {
            var isCollapseState = StateValueOf(false, true);
            var visibleState = StateValueOf(true, true);
            var animateState = AnimateFloatOf(visibleState);
            var parentWidth = StateValueOf(0);
            Spacer(FILL).Click(() =>
            {
                isCollapseState.Value = true;
                visibleState.Value = true;
            }, false)
            .Bind(animateState,(modify, value)=>
            {
                if (visibleState.Value)
                {
                    var start = isCollapseState.Value? 0f: 0.12f;
                    var end = isCollapseState.Value ? 0.12f : 0f;
                    value = start + (int)((end - start) * value);
                    modify.Background(XTheme.Color.Black.Copy(value));
                }
            });           
            return Column(StateRepository.SelectedMenuItem, menu =>
            {
                Row(() =>
                {
                    Text("软件管理模板").H3().SingleLine().TextAlignment(XAlignment.Center).Weight(1);
                    Icon(SvgRes.Operation).Size(40).IconSize(30).Radius(10)
                    .Click(() =>
                    {
                        isCollapseState.Value = !isCollapseState.Value;
                        visibleState.Value = true;
                    });

                }).Padding(10).Width(FILL);

                foreach (var item in Service<UserService>().MenuItems)
                {
                    var id = menu.Id == 0 ? 1 : menu.Id;
                    MenuItem(item, id == item.Id).Click(() =>
                    {
                        StateRepository.SelectedMenuItem.Value = item;
                        isCollapseState.Value = true;
                        visibleState.Value = true;
                    });
                }
            })
            .Width(0)
            .Bind(animateState, (buidler, value) =>
            {
                if (visibleState.Value)
                {
                    var start = buidler.View.Width.AsDp();
                    var end = isCollapseState.Value ? 70 : 280;
                    buidler.Width(start + (int)((end - start) * value));
                    if(isCollapseState.Value && value > 0.98)
                    {
                        menuState.Value = false;
                    }
                }
            }, needLayout: true)
            .HorizontalAlignment(XHorizontalAlignment.Left)
            .Background(XTheme.Color.LighterBorder)
            .Shadow();
        }

        private static XModify Menus()
        {
            var isCollapseState = StateValueOf(false);
            var isUserClick = StateValueOf(false);
            var visibleState = StateValueOf(false);
            var animateState = AnimateFloatOf(visibleState);
            var isHand = StateValueOf(false);
            var parentWidth = StateValueOf(0);
            return Column(StateRepository.SelectedMenuItem, menu =>
            {
                Row(() =>
                {
                    Text("软件管理模板").H3().SingleLine().TextAlignment(XAlignment.Center).Weight(1);
                    Icon(SvgRes.Operation).Size(40).IconSize(30).Radius(10)
                    .Click((modify, info)=>
                    {
                        if(isCollapseState.Value && modify.View.RootView().Width <= 850.AsPx())
                        {
                            menuState.Value = true;
                            return;
                        }
                        isCollapseState.Value = !isCollapseState.Value;
                        visibleState.Value = true;
                        isUserClick.Value = isCollapseState.Value;
                    });

                }).Padding(10).Width(FILL);

                foreach (var item in Service<UserService>().MenuItems)
                {
                    var id = menu.Id == 0 ? 1 : menu.Id;
                    MenuItem(item, id == item.Id).Click(() =>
                    {
                        StateRepository.SelectedMenuItem.Value = item;
                    });
                }
            })
            .Width(280).MinWidth(70).MaxWidth(800)
            .MeasureStart(builder =>
            {
                if (visibleState.Value || parentWidth.Value == builder.View.Parent.Width || isUserClick.Value)
                {
                    return;
                }
                parentWidth.Value = builder.View.Parent.Width;
                if (builder.View.Parent.Width <= 850.AsPx())
                {
                    builder.Width(70);
                    isCollapseState.Value = true;
                } 
                else if(builder.View.Parent.Width >= 1280.AsPx())
                {
                    builder.Width(280);
                    isCollapseState.Value = false;
                    menuState.Value = false;
                }
            })
            .Bind(animateState, (buidler, value) =>
            {
                if (visibleState.Value)
                {
                    var start = buidler.View.Width.AsDp();
                    var end = isCollapseState.Value ? 70 : 280;
                    buidler.Width(start + (int)((end - start) * value));
                }
            }, needLayout: true)
            .Resize(right: true)
            .OnResize(builder => isCollapseState.Value = builder.View.Width <= 140.AsPx())
            .OnUp((builder, _) => visibleState.Value = isCollapseState.Value)
            .HorizontalAlignment(XHorizontalAlignment.Left)
            .Background(XTheme.Color.LighterBorder);
        }

        private static XModify MenuItem(MenuItem item,bool isSelected)
        {
            return Row(() =>
            {
                var color = isSelected ? XTheme.Color.Primary : XTheme.Color.PrimaryText;
                Icon(item.IconId).Size(30).Color(color);
                Text(item.Name).TextSuffix("...").FontSize(20).Color(color).SingleLine().Weight(1);
            }).Width(FILL).Padding(20).Space(10);
        }
    }
}
