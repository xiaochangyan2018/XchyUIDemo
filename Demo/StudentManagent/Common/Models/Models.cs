using XcyUI.models;

namespace XcyDemo.StudentManagent.Common.Models
{
    public struct MenuItem
    {
        public int Id { get; set; }
        public int IconId { get; set; }
        public string Name { get; set; }
        public MenuItem(int id, int iconId,string name)
        {
            Id = id;
            IconId = iconId;
            Name = name;
        }
    }

    public struct UserSumData
    {
        public int IconId { get; set; }
        public XColor Color { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public bool IsUp { get; set; }
        public string upNum { get; set; }
        public string upTitle { get; set; }
    }

    public enum ActivityType
    {
        AddStudent,
        AddGrades,
        AddAttendance,
        AddLeave
    }

    public struct ActivityData
    {
        public ActivityType Type { get; set; }
        public string Desction { get; set; }
        public string Name { get; set; }
        public string DateTime { get; set; }
        public int Status { get; set; }
        public ActivityData(ActivityType type, string desction,string name,string dateTime, int status)
        {
            Type = type;
            Desction = desction;
            Name = name;
            DateTime = dateTime;
            Status = status;
        }
    }

    public class StudentInfo
    {
        public bool IsSelected { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public int Sex { get; set; }
        public string Birthday { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public StudentInfo() { }

        public StudentInfo(string number, string name, int sex, string birthday, string className, string subjectName)
        {
            Number = number;
            Name = name;
            Sex = sex;
            Birthday = birthday;
            ClassName = className;
            SubjectName = subjectName;
        }
    }

    public struct GradesInfo
    {
        public int UserNumber { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public int GradesValue { get; set; }
        public string Renewal { get; set; }
        public string ExamDate { get; set; }
        public GradesInfo(int userNumber,string name,string className,string subjectName,int gradesValue,string renewal,string examDate)
        {
            UserNumber = userNumber;
            Name = name;
            ClassName = className;
            SubjectName = subjectName;
            GradesValue = gradesValue;
            Renewal = renewal;
            ExamDate = examDate;
        }

        public struct GradesReportItem
        {
            public string SubjectName { get; set; }
            public int UserCount { get; set; }
            public float AverageScore { get; set; }
            public float MaxScore { get; set; }
            public float MinScore { get; set; }
            public string PassRate { get; set; }
            public string ExcellenceRate { get; set; }
            public GradesReportItem(string subjectName,int userCount,float averageScore,float maxScore,float minScore,string passRate,string excellenceRate)
            {
                SubjectName = subjectName;
                UserCount = UserCount;
                AverageScore = averageScore;
                MaxScore = maxScore;
                MinScore = minScore;
                PassRate = passRate;
                ExcellenceRate = excellenceRate;
            }
        }
    }
}
