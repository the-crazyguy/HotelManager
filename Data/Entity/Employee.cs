using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    //TODO: Remove redundant class
    public class Employee : PersonBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string MiddleName { get; set; }
        public override string FullName => $"{base.FirstName} {MiddleName} {base.LastName}";    //QOL field
        public string EGN { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public DateTime Hired { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Fired { get; set; }

    }
}
