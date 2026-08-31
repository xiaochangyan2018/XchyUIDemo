using System;
using System.Collections.Generic;
using System.Text;
using XcyDemo.StudentManagent.Common.Models;
using XcyUI.Controls;
using XcyUI.models;
using XcyUI.theme;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XcyDemo.StudentManagent.Common.Convert
{
    public static class Converts
    {
        public static (int,XColor,string) ToType(this ActivityType type)
        {
            int iconId;
            XColor color;
            string text;
            switch (type)
            {
                case ActivityType.AddStudent:
                    iconId = SvgRes.UserFilled;
                    color = XTheme.Color.Primary;
                    text = "添加学生";
                    break;
                case ActivityType.AddGrades:
                    iconId = SvgRes.Management;
                    color = XTheme.Color.Success;
                    text = "成绩录入";
                    break;
                case ActivityType.AddAttendance:
                    iconId = SvgRes.Calendar;
                    color = XTheme.Color.Warning;
                    text = "出勤记录";
                    break;
                default:
                    iconId = SvgRes.WarningFilled;
                    color = XTheme.Color.Danger;
                    text = "请假申请";
                    break;
            }
            return (iconId, color, text);
        }
    }
}
