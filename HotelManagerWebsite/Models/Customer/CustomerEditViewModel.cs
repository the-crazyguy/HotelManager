using HotelManagerWebsite.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Customer
{
    public class CustomerEditViewModel : BaseEditViewModel
    {
        [Required(ErrorMessage = "Please enter a first name for the customer")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name for the customer")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Please enter a phone number for the customer")]
        [Phone(ErrorMessage = "Please enter a 10 digit phone number")]
        [MinLength(10)]
        [MaxLength(10)]
        [Display(Name = "Phone Number")]
        public string PhoneNumer { get; set; }

        [Required(ErrorMessage = "Please enter an e-mail for the customer")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter whether the customer in an adult or not")]
        [Display(Name = "Adult")]
        public bool IsAdult { get; set; }
    }
}
