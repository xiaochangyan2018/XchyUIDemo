using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XcyDemo.Sample.Model
{
    public class UserInfo
    {
        public string Name { get; set; }
        
        public int Age { get; set; }
        
        public DateTime Birthday { get; set; }
        public string Phone { get; set; }
        public int Sex { get; set; }
        public List<string> Hobbys { get; set; }

        public string Profile { get; set; }

    }
}
