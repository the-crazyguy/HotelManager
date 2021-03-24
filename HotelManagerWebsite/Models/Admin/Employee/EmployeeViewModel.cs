using HotelManagerWebsite.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Admin.Employee
{
    public class EmployeeViewModel : BasePersonViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }    //TODO: Protect it somehow?
        public string MiddleName { get; set; }
        public override string FullName => $"{base.FirstName} {MiddleName} {base.LastName}";
        public string EGN { get; set; }
        public ICollection<ReservationViewModel> Reservations { get; set; } 
        public DateTime Hired { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Fired { get; set; }
    }
}
