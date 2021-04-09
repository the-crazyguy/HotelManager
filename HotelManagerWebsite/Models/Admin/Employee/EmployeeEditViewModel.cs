using Data.Entity;
using HotelManagerWebsite.Models.Base;
using HotelManagerWebsite.Models.Reservation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Admin.Employee
{
    //TODO: Rework or remove
    public class EmployeeEditViewModel : BasePersonViewModel
    {
        [Required(ErrorMessage = "Please enter a password for the employee")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter a middlename for the employee")]
        public string MiddleName { get; set; }
        public override string FullName => $"{base.FirstName} {MiddleName} {base.LastName}";

        [Phone(ErrorMessage = "Please enter EGN for the employee")]
        [MinLength(10)]
        [MaxLength(10)]
        public string EGN { get; set; }
        public ICollection<ReservationViewModel> Reservations { get; set; }
        public DateTime Hired { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Fired { get; set; }
    }
}
