using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Base
{
    public class PersonBase : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        
    }
}
