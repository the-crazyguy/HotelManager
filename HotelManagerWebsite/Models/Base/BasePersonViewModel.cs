using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Base
{
    public abstract class BasePersonViewModel : BaseViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual string FullName => $"{FirstName} {LastName}";
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
