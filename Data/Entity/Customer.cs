using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class Customer : PersonBase
    {
        public bool IsAdult { get; set; }
        public virtual ICollection<CustomerReservation> CustomerReservations { get; set; }
    }
}
