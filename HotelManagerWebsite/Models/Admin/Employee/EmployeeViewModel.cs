using HotelManagerWebsite.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Admin.Employee
{
    public class EmployeeViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        public string UserName { get; set; }
        //TODO: Add password?
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string EGN { get; set; }
        public ICollection<ReservationViewModel> Reservations { get; set; }
        public DateTime Hired { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Fired { get; set; }
    }
}
