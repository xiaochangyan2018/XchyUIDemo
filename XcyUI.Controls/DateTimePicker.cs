using System;
using XcyUI.expansions;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.widgets.XCompose;

namespace XcyUI.Controls
{
    public static partial class Controls
    {
        public static XModify DateTimeInput(DateTime? dateTime = null, Action<DateTime> onSelected = null, DateTime? startTime = null, DateTime? endTime = null, int cellHeight = 50)
        {
            var visibleState = StateValueOf(false);
            var dateTimeState = StateValueOf(dateTime, isReset: true);
            return Box(dateTimeState, date =>
            {
                Input(date?.ToString("yyyy-MM-dd")??"")
                .Width(FILL)
                .PrimaryInput()
                .ReadOnly()
                .Padding(right: XTheme.Size.Space16 * 2 + 20)
                .KeyPress((b, info) =>
                {
                    if (info.KeyValue == XKeyValue.Enter)
                    {
                        b.View.Parent.EventParams.EventOrCreate(XEventType.Click)?.Invoke(b.View.Parent, new XEventInfo() { EventType = XEventType.Click });
                    }
                })
                .Click(() => { }, false)
                .SingleLine().InputType(InputType.Date);
                Icon(SvgRes.Calendar).IconSize(20).Alignment(XAlignment.RightCenter).Margin(right:XTheme.Size.Space16);
            })
            .Size(200, WRAP).Popover(visibleState, content: ()=>
            {
                DateTimePicker(dateTimeState.Value ?? DateTime.Now, date =>
                {
                    dateTimeState.Value = date;
                    visibleState.Value = false;
                }, startTime, endTime, cellHeight);

            },defaultEffect:false);
        }
        public static XModify DateTimePicker(DateTime dateTime, Action<DateTime> onSelected = null, DateTime? startTime = null, DateTime? endTime = null, int cellHeight = 50)
        {
            startTime = startTime ?? new DateTime(1900, 1, 1);
            endTime = endTime ?? new DateTime(3000, 12, 31);
            var typeState = StateValueOf(0); // 0 选择天，1选择年，2选择月
            var currentDateTimeState = StateValueOf(dateTime, true);
            var selectedDateTimeState = StateValueOf(dateTime, true);
            var startYearState = StateValueOf(0);
            currentDateTimeState.Join(startYearState);
            string[] months = { "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };

            void DateTitleBar()
            {
                Row(() =>
                {
                    Icon(SvgRes.DArrowLeft).Size(20)
                    .Margin(vertical: 10).Hand()
                    .Click(() =>
                    {
                        if(typeState.Value == 1)
                        {
                            startYearState.Value -= 10;
                        }
                        else
                        {
                            currentDateTimeState.Value = currentDateTimeState.Value.AddYears(-1);
                        }
                       
                    }, defaultEffect: false);

                    // 选择天的时候
                    if (typeState.Value == 0)
                    {
                        Icon(SvgRes.ArrowLeft).Size(20).Hand().Click(() =>
                        {
                            currentDateTimeState.Value = currentDateTimeState.Value.AddMonths(-1);
                        }, defaultEffect: false);
                    }

                    Row(() =>
                    {
                        // 年分
                        Text().H3().Hand()
                        .Bind(currentDateTimeState, (builder, date) =>
                        {
                            var year = date.Year.ToString();
                            if (typeState.Value == 1)
                            {
                                year = $"{startYearState.Value}-{startYearState.Value + 9}";
                            }
                            builder.Content(year);

                        }, needLayout:true)
                        .Click(() =>
                        {
                            var year = currentDateTimeState.Value.Year;
                            startYearState.Value = year - year % 10;
                            typeState.Value = 1;
                        }, defaultEffect: false);

                        // 选择天的时候
                        if (typeState.Value == 0)
                        {
                            // 月份
                            Text().H3().Hand()
                            .Bind(currentDateTimeState, (builder, date) =>
                            {
                                builder.Content(months[date.Month - 1]);
                            }, needLayout: true)
                            .Click(() =>
                            {
                                typeState.Value = 2;
                            }, defaultEffect: false);
                        }
                    }).Weight(1).Space(20).HorizontalAlignment(XHorizontalAlignment.Center);

                    // 选择天的时候
                    if (typeState.Value == 0)
                    {
                        Icon(SvgRes.ArrowRight).Size(20).Hand().Click(() =>
                        {
                            currentDateTimeState.Value = currentDateTimeState.Value.AddMonths(1);
                        }, defaultEffect: false);
                    }

                    Icon(SvgRes.DArrowRight).Size(20).Hand().Click(() =>
                    {
                        if (typeState.Value == 1)
                        {
                            startYearState.Value += 10;
                        }
                        else
                        {
                            currentDateTimeState.Value = currentDateTimeState.Value.AddYears(1);
                        }
                    }, defaultEffect: false);

                }).Size(FILL, WRAP).Space(10).Padding(horizontal: 10);
            }

            void WeekBar()
            {
                string[] weeks = { "日", "一", "二", "三", "四", "五", "六" };
                Spacer(10);
                Row(() =>
                {
                    for (int i = 0; i < weeks.Length; i++)
                    {
                        Text(weeks[i]).Height(cellHeight).TextAlignment(XAlignment.Center);
                    }
                }).Width(FILL).HorizontalAlignment(XHorizontalAlignment.Bisect);
            }

            void SetHoverStyle(XModify builder, bool isCurrent)
            {
                var background = isCurrent ? XTheme.Color.Primary : XColors.Transparent;
                var color = isCurrent ? XTheme.Color.White : XTheme.Color.PrimaryText;
                var hoverColor = isCurrent ? XTheme.Color.White :
                XTheme.Color.Primary;
                builder.Background(background)
                   .Color(color)
                   .HoverColor(hoverColor);
            }

            void SelectYearPanel()
            {
                Spacer(20);
                Flow(startYearState, startYear =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var year = startYear + i;
                        Text($"{year}")
                        .Height(cellHeight)
                        .TextAlignment(XAlignment.Center)
                        .Radius(cellHeight/2)
                        .Hand()
                        .Click(() =>
                        {
                            var num = year - currentDateTimeState.Value.Year;
                            currentDateTimeState.Value = currentDateTimeState.Value.AddYears(num);
                            typeState.Value = 2;
                        }, defaultEffect: false)
                        .Also(builder=>
                        {
                            SetHoverStyle(builder, year == currentDateTimeState.Value.Year);
                        });
                    }
                }).Size(FILL, WRAP).Cells(4).Space(20).Padding(20);
            }

            void SelectMonthPanel()
            {
                Spacer(20);
                Flow(currentDateTimeState, date =>
                {
                    for (int i = 0; i < 12; i++)
                    {
                        var mouth = i + 1;
                        Text($"{months[i]}")
                        .Height(cellHeight)
                        .TextAlignment(XAlignment.Center)
                        .Radius(cellHeight / 2)
                        .Hand()
                        .Click(() =>
                        {
                            var num = mouth - currentDateTimeState.Value.Month;
                            currentDateTimeState.Value = currentDateTimeState.Value.AddMonths(num);
                            typeState.Value = 0;
                        }, defaultEffect: false)
                        .Also(builder =>
                        {
                            SetHoverStyle(builder, mouth == currentDateTimeState.Value.Month);
                        });
                    }
                }).Size(FILL, WRAP).Cells(4).Space(20).Padding(20);
            }

            // 选择天
            void SelectDayPanel()
            {

                Flow(currentDateTimeState, currentMouth =>
                {
                    DateTime firstDayOfMonth = new DateTime(currentMouth.Year, currentMouth.Month, 1);
                    int dayOfWeek = (int)firstDayOfMonth.DayOfWeek;
                    if (dayOfWeek == 0)
                    {
                        dayOfWeek = 7;
                    }
                    DateTime startDate = firstDayOfMonth.AddDays(-dayOfWeek);

                    for (int row = 0; row < 6; row++)
                    {
                        for (int col = 0; col < 7; col++)
                        {
                            DateTime day = startDate.AddDays(row * 7 + col);
                            bool isCurrentMonth = day.Year == currentMouth.Year && day.Month == currentMouth.Month;
                            bool isToday = day.Date == DateTime.Today;
                            var textColor = XTheme.Color.PrimaryText;
                            if (isToday)
                            {
                                textColor = XTheme.Color.Primary;
                            }
                            else if (!isCurrentMonth)
                            {
                                textColor = XTheme.Color.DarkBorder;
                            }
                            var isOutDate = day < startTime || day > endTime;
                            Box(() =>
                            {
                                Text(day.Day.ToString())
                                .Size(cellHeight - 10)
                                .FontWeight(isToday ? XTheme.Weight.Large : XTheme.Weight.Middle)
                                .Color(textColor)
                                .TextAlignment(XAlignment.Center)
                                .Circle()
                                .HoverCursor(XCursorType.Hand)
                                .Click(() =>
                                {
                                    selectedDateTimeState.Value = day;
                                    currentDateTimeState.Value = day;
                                    if (isCurrentMonth)
                                    {
                                        onSelected?.Invoke(day);
                                    }
                                })
                                .Also(builder =>
                                {
                                    var selectDate = selectedDateTimeState.Value;
                                    if (selectDate == day)
                                    {
                                        builder
                                        .Background(XTheme.Color.Primary)
                                        .Color(XTheme.Color.White)
                                        .HoverColor(XTheme.Color.White);
                                    }
                                    else
                                    {
                                        builder
                                       .Background(XTheme.Color.Transparent)
                                       .Color(textColor)
                                       .HoverColor(XTheme.Color.Primary);
                                    }
                                    builder.EnableEvent(!isOutDate).Alpha(isOutDate ? XTheme.Color.DisabledAlpha : 1);
                                });
                            }).Size(WRAP).Height(cellHeight);
                        }
                    }
                }).Size(FILL, WRAP).Cells(7);

            }
            return Column(typeState, type =>
            {
                Spacer(10);
                DateTitleBar();

                if (type == 0)
                {
                    WeekBar();
                    Spacer(1).Width(FILL).Background(XTheme.Color.BaseBorder);
                    SelectDayPanel();
                }
                else if (type == 1)
                {
                    SelectYearPanel();
                }
                else
                {
                    SelectMonthPanel();
                }
            }).Size(400, WRAP).Alignment(XAlignment.TopCenter);
        }
    }
}
