using XcyDemo.Sample.Service;
using XcyUI.Controls;
using XcyUI.models;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.Controls.Controls;
using static XcyUI.widgets.XDIWidget;
using static XcyUI.widgets.XCompose;
using static XcyDemo.Sample.Component.CustomControls;
using XcyUI.GLFW;

namespace XcyDemo.Sample
{
    public static class FormSample
    {
        private static XState<string> errorStringState = new();
        public static XModify View(XState<bool> visibleState)
        {
            var service = Service<UserService>();
            var user = service.GetUserInfo();
            return Column(() =>
            {
                Text("用户注册").H2().Alignment(XAlignment.Center);
                Spacer();
                Column(() =>
                {
                    ResetValidate();
                    LableInput("姓名", user.Name, v => user.Name = v, required: true).Focus();

                    LableNumber("年龄", user.Age, v => user.Age = v, 1, 120);

                    LableDate("出生日期", user.Birthday, v => user.Birthday = v, required: true);

                    LableInput("手机号", user.Phone, v => user.Phone = v, InputType.Phone);

                    LableRadioGroup("性别", [("男", 1), ("女", 2)], user.Sex, v => user.Sex = v);

                    var items = service.HobbyTypes().Select(n => (n, n)).ToList();
                    LableCheckboxGroup("爱好", items, user.Hobbys, v => user.Hobbys = v);

                    LableMuiltInput("自我介绍", lines: 3, user.Profile, v => user.Profile = v);
                    LableMuiltInput("自我介绍", lines: 3, user.Profile, v => user.Profile = v);
                    LableMuiltInput("自我介绍", lines: 3, user.Profile, v => user.Profile = v);
                    LableMuiltInput("自我介绍", lines: 3, user.Profile, v => user.Profile = v);
                }).Weight(1).Scrollable().Space(20).Padding(20);

                Row(() =>
                {
                    Text("取消").SubButton(() =>
                    {
                        visibleState.Value = true;
                    }).Width(150);

                    AsyncButton(
                        text: "注册",
                        loadingText: "注册中...",
                        preFunc: startState => Validate(() => startState.Value = true),
                        asyncFun: () =>
                        {
                            ShowToast("验证成功");
                            visibleState.Value = true;
                        }).Width(150);
                })
                .Width(FILL).Space(50)
                .HorizontalAlignment(XHorizontalAlignment.Center);
            })
            .Space(20)
            .Size(700)
            .Padding(vertical:20)
            .Clip()
            .HorizontalAlignment(XHorizontalAlignment.Left);
        }
    }
}
