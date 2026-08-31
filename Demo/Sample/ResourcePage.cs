using System.Reflection;
using TextCopy;
using XcyUI.models;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;
using static XcyUI.Controls.Controls;
using XcyUI.Controls;
using XcyUI.expansions;

namespace XcyDemo.Sample
{
    public struct SvgItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    public static class ResourcePage
    {
        private static List<SvgItem> items = new();
        public static void Load()
        {
            if (items.Count == 0)
            {
                Type type = typeof(SvgRes);
                foreach (var item in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    items?.Add(new() { Name = item.Name, Id = (int)(item.GetValue(null) ?? 0) });
                }
            }
        }
        public static XModify View()
        {
            Load();
            return Column(()=>
            {
                var itemsState = StateValueOf(items);
                Row(() =>
                {

                    Text("该图标出至于elementUI 双击选中复制").H3();

                    Input().PrimaryInput().Width(300)
                    .Hint("搜索图标")
                    .KeyPress((builder,info) =>
                    {
                        itemsState.Value = items.Where(n => n.Name.ToLower().Contains(builder.Content().ToLower())).ToList();
                    });
                }).Space(30).Padding(30);
                Flow(itemsState, items =>
                {
                    foreach (var item in items)
                    {
                        Column(() =>
                        {
                            Icon(item.Id).IconSize(30);

                            Text(item.Name, true)
                            .TextAlignment(XAlignment.Center)
                            .Padding(2)
                            .DoubleClick((builder, info) =>
                            {
                                ClipboardService.SetText($"SvgRes.{builder.Content()}");
                                ShowToast($"已复制 {builder.Content()}", SvgRes.SuccessFilled);
                            }, false);
                        })
                        .Height(WRAP)
                        .AspectRatio(1)
                        .VerticalAlignment(XVerticalAlignment.Center)
                        .Padding(10)
                        .Space(10)
                        .DefaultBorder()
                        .Radius(10);
                    }
                })
                .Weight(1)
                .Space(20)
                .Padding(20)
                .Scrollable()
                .MeasureStart(builer=>
                {
                    var itemWidth = 120;
                    var cell = builer.View.ContentRect.Width.AsDp() / (itemWidth+20);
                    builer.Cells(cell);
                });
            })
            .HorizontalAlignment(XHorizontalAlignment.Left);
        }
    }
}
