using XcyUI.Controls;
using XcyUI.models;
using XcyUI.theme;
using XcyUI.widgets;
using XcyUI.widgets.extensions;
using static XcyUI.Controls.Controls;
using static XcyUI.widgets.XCompose;

namespace XcyDemo.Sample
{
    public class UserInfo1
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public UserInfo1(string date, string name, string state, int age, string city, string address, string zip)
        {
            Date = date;
            Name = name;
            State = state;
            Age = age;
            City = city;
            Address = address;
            Zip = zip;
        }
    }
    public static class DataGridTest
    {
        public static XModify View()
        {

            return Column(() =>
            {
                var list = new List<UserInfo1>();
                Random random = new Random();
                var startDate = DateTime.Now.AddMonths(-2);
                for (int i = 0; i < 1000; i++)
                {
                    list.Add(new(startDate.AddDays(i).ToString("yyyy-MM-dd"), "Tom" + (i % 2 == 0 ? "1" : i == 3 ? "2" : "3"), "California", random.Next(20, 50), "Los Angeles" + i, "No. 189, Grove St, Los Angeles" + i, "CA 90036" + i));
                }
                var datasState = StateValueOf(list);
                DataGrid(datasState,
                [
                    new("",100, v=> ""){ SelectItemsState = new(new()), Fixed = Fixed.Left },
                    new("Date",200, v=> v.Date){Fixed = Fixed.Left, IsResize = true, IsFilter = true, FilterType = FilterType.Date},
                    new("Name",200, v=> v.Name){IsSort = true, IsFilter = true, IsResize = true, FilterType = FilterType.Selecter},
                    new("State",200, v=> v.State)
                    {
                        IsResize = true,
                        CellContent = (cell, v)=>
                        {
                            Text(v.State).Padding(10,5).Radius(50)
                            .Background(XTheme.Color.Success)
                            .Color(XTheme.Color.White);
                        }
                    },
                    new("Age",130, v=> v.Age){ IsSort = true,IsResize = true,IsFilter = true,FilterType = FilterType.Number },
                    new("City",200, v=> v.City){ IsSort = true,IsResize = true },
                    new("Address",400, v=> v.Address) {IsResize = true, IsFilter = true},
                    new("Address",400, v=> v.Address){IsResize = true},
                    new("Zip",200, v=> v.Zip){IsResize = true},
                    new("Operations",150, v=> "")
                    {
                        Fixed = Fixed.Right,
                        CellContent = (cell, data) => {
                            Text("Remove")
                            .FontWeight(XTheme.Weight.Middle)
                            .Alignment(XAlignment.LeftCenter)
                            .Color(XTheme.Color.Primary)
                            .HoverColor(XTheme.Color.PrimaryLight2)
                            .HoverCursor(XCursorType.Hand)
                            .Click(()=>
                            {
                                list.Remove(data);
                                datasState.Send(list);
                            }, false);
                        }
                    },
                ], isGridBorder: true, isAnimate: true, modify: (modify, index) =>
                {
                    modify.Background(XTheme.Color.Background);
                    if (index >= 0)
                    {
                        modify.HoverBackgroundAllColor(XTheme.Color.LightFill);
                    }
                }).Size(FILL);

            })
            .Size(FILL)
            .Padding(20)
            .DefaultBorder()
            .VerticalAlignment(XVerticalAlignment.Center);
        }
    }
}
