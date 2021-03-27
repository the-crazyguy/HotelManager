using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class EmployeeUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public virtual string FullName => $"{FirstName} {MiddleName} {LastName}";
        public string EGN { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public DateTime Hired { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Fired { get; set; }
    }
}
