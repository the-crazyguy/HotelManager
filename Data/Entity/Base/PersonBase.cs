using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity.Base
{
    public abstract class PersonBase : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual string FullName => $"{FirstName} {LastName}";    //QOL field
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        
    }
}
