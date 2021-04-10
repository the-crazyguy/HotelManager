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
    public class EmployeeEditViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please enter a first name for the employee")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a middle name for the employee")]
        public string MiddleName { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        [Required(ErrorMessage = "Please enter a last name for the employee")]
        public string LastName { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number for the employee")]
        [MinLength(10)]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email for the employee")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password for the employee")]
        public string Password { get; set; }

        [Phone(ErrorMessage = "Please enter EGN for the employee")]
        [MinLength(10)]
        [MaxLength(10)]
        public string EGN { get; set; }
        public ICollection<Data.Entity.Reservation> Reservations { get; set; }
        public DateTime Hired { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Fired { get; set; }
    }
}
