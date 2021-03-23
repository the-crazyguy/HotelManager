using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class Employee : PersonBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string MiddleName { get; set; }
        public string EGN { get; set; }
        public DateTime Hired { get; set; }
        public bool IsActive { get; set; }
        public DateTime Fired { get; set; }

    }
}
