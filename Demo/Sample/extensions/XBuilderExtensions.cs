using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using XcyUI.Controls;
using XcyUI.widgets;
using XcyUI.widgets.extensions;

namespace XcyDemo.Sample.extensions
{
    public static class XBuilderExtensions
    {
        public static XModify Bind(this XModify builder, JsonNode node, InputType type, string properName)
        {
            return builder.Content(node[properName]?.ToString()).TextChanged((builder, text) =>
            {
                SetDynamicProperty(node, type, properName, text);
            });
        }

        public static void SetDynamicProperty(JsonNode node,InputType type, string properName, string text)
        {
            if (type == InputType.Number && int.TryParse(text, out int intValue))
            {
                node[properName] = intValue;
                return;
            }

            if (type == InputType.Decimal && double.TryParse(text, out double doubleValue))
            {
                node[properName] = doubleValue;
                return;
            }
            if (type == InputType.Date && DateTime.TryParse(text, out DateTime date))
            {
                node[properName] = date;
                return;
            }
            node[properName] = text;
        }
    }
}
