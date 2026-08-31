using XcyUI.Controls;
using XcyUI.Controls.Utils;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.Controls.Controls;
using static XcyUI.widgets.XCompose;

namespace XcyDemo.Sample.Component
{
    public static class CustomControls
    {
        private static Dictionary<int, Func<string>> validator = new();
        public static void ResetValidate()
        {
            validator.Clear();
        }
        public static void Validate(Action func)
        {
            bool isSuccss = true;
            foreach (var item in validator)
            {
                string error = item.Value.Invoke();
                if (!string.IsNullOrEmpty(error))
                {
                    isSuccss = false;
                    ShowToast(error);
                    break;
                }
            }
            if (isSuccss)
            {
                func.Invoke();
            }
        }
        private static string GetInputErrorString(string lable, string text, InputType? inputType = null, bool required = false,int maxLength = 1000)
        {
            if (required && string.IsNullOrEmpty(text))
            {
                return $"请输入{lable}";
            }
            else if(!string.IsNullOrEmpty(text) && text.Length > maxLength)
            {
                return $"{lable}的字符长度不能大于${maxLength}";
            }
            else if (inputType != null && !InputRegex.Validate(inputType.Value, text))
            {
                return $"请输入正确的{lable}";
            }
            else
            {
                return "";
            }
        }
        private static string GetNullErrorString(string lable, string text, bool required = false)
        {
            if (required && string.IsNullOrEmpty(text))
            {
                return $"请输入{lable}";
            }
            else
            {
                return "";
            }
        }
        private static string GetNumberErrorString(string lable, int value, int? min = null,int? max = null)
        {
            if (min != null && value < min.Value)
            {
                return $"{lable}不能小于{min.Value}";
            }
            else if (max != null && value > max.Value)
            {
                return $"{lable}不能大于{max.Value}";
            }
            else
            {
                return "";
            }
        }
        private static string GetDateErrorString(string lable, DateTime value, DateTime? min = null, DateTime? max = null, bool required = false)
        {
            if(required && value == DateTime.MinValue)
            {
                return $"请选择{lable}";
            } 
            else if (min != null && value < min.Value)
            {
                return $"{lable}不能小于{min.Value}";
            }
            else if (max != null && value > max.Value)
            {
                return $"{lable}不能大于{max.Value}";
            }
            else
            {
                return "";
            }
        }
        public static XModify LableInput(string lable, string value = "", Action<string>? valueChanged = null, InputType? inputType = null, bool required = false, int maxLength = 1000)
        {
            return Column(() =>
            {
                Text(() =>
                {
                    if (required)
                    {
                        Span("* ").Color(XColors.Red);
                    }
                    Span(lable);
                }).Width(100);
                if (inputType != null)
                {
                    Input(value?.ToString() ?? "").PrimaryInput().Width(FILL)
                    .InputType(inputType.Value)
                    .Bind<string>((b,v) =>
                    {
                        valueChanged?.Invoke(v);
                        validator[b.View.GetHashCode()] = () => GetInputErrorString(lable, v,inputType, required,maxLength);
                    })
                    .Also(b=>
                    {
                        validator[b.View.GetHashCode()] = () => GetInputErrorString(lable, b.Content(), inputType, required, maxLength);
                    });
                }
                else
                {
                    Input(value?.ToString() ?? "").PrimaryInput().Width(FILL)
                    .Bind<string>((b, v) =>
                    {
                        valueChanged?.Invoke(v);
                        validator[b.View.GetHashCode()] = () => GetInputErrorString(lable, v, inputType, required, maxLength);
                    })
                    .Also(b =>
                    {
                        validator[b.View.GetHashCode()] = () => GetInputErrorString(lable, b.Content(), inputType, required, maxLength);
                    });
                }
            }).Size(FILL, WRAP).Space(10).HorizontalAlignment(XHorizontalAlignment.Left);
        }

        public static XModify LableNumber(string label, int value, Action<int> valueChanged, int min = 0, int max = int.MaxValue, float step = 1)
        {
            return LableNumber(label, value, v => valueChanged.Invoke((int)v), min, max, step, 0);
        }

        public static XModify LableNumber(string lable, float value, Action<float> valueChanged, int? min = null, int? max = null, float step = 1, int precision = 2)
        {
            return Column(() =>
            {
                Text(lable).Width(100);
                NumberInput(value, step, precision).Weight(1)
                .Bind<int>((b, v) =>
                {
                    valueChanged?.Invoke(v);
                    validator[b.View.GetHashCode()] = () => GetNumberErrorString(lable, v, min, max);
                });
            }).Size(FILL, WRAP).Space(10).HorizontalAlignment(XHorizontalAlignment.Left);
        }

        public static XModify LableDate(string lable, DateTime? value, Action<DateTime> valueChanged, DateTime? min = default, DateTime? max = default, bool required = false)
        {
            return Column(() =>
            {
                Text(() =>
                {
                    if (required)
                    {
                        Span("* ").Color(XColors.Red);
                    }
                    Span(lable);
                }).Width(100);
                DateTimeInput(value, startTime: min, endTime: max).Width(FILL)
                 .Bind<DateTime>((b, v) =>
                 {
                     valueChanged?.Invoke(v);
                     validator[b.View.GetHashCode()] = () => GetDateErrorString(lable, string.IsNullOrEmpty(b.Content())?DateTime.MinValue:v, min, max, required);
                 })
                 .Also(n =>
                 {
                     n.FirstText(b =>
                     {
                         validator[b.View.GetHashCode()] = () => GetNullErrorString(lable, b.Content(), required);
                     });
                 });
            }).Size(FILL, WRAP).HorizontalAlignment(XHorizontalAlignment.Left).Space(10);
        }

        public static XModify LableRadioGroup<T>(string lable, List<(string, T)> items, T value, Action<T> valueChanged)
        {
            return LableContent(lable, () =>
            {
                RadioGroup(items, value, valueChanged);
            });
        }

        public static XModify LableCheckboxGroup<T>(string lable, List<(string, T)> items, List<T> value, Action<List<T>> valueChanged)
        {
            return LableContent(lable, () =>
            {
                CheckboxGroup(items, value, valueChanged);
            });
        }

        public static XModify LableMuiltInput<T>(string lable, int lines, T value, Action<T> valueChanged)
        {
            return LableContent(lable, () =>
            {
                Input().PrimaryInput().Lines(lines).Width(FILL).Bind(valueChanged);
            });
        }


        public static XModify LableContent(string title, Action func)
        {
            return Column(() =>
            {
                Text(title).Width(100).Alignment(XAlignment.LeftTop);
                func.Invoke();
            }).Size(FILL, WRAP).Space(10).HorizontalAlignment(XHorizontalAlignment.Left);
        }
    }
}
