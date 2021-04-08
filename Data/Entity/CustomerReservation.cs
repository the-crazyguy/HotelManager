using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class CustomerReservation
    {
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public int ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}
