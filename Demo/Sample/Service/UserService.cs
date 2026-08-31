using System;
using System.Collections.Generic;
using System.Text;
using XcyDemo.Sample.Model;

namespace XcyDemo.Sample.Service
{
    public class UserService
    {
        public List<string> HobbyTypes()
        {
            return ["篮球", "羽毛球", "桌球", "看美女"];
        }
        public UserInfo GetUserInfo()
        {
            return new UserInfo()
            {
                Name = "张三",
                Age = 30,
                Birthday = DateTime.Parse("1999-01-02"),
                Sex = 2,
                Phone = "19809890981",
                Hobbys = []
            };
        }
    }
}
