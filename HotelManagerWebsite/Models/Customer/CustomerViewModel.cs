using Data.Entity;
using HotelManagerWebsite.Models.Base;
using HotelManagerWebsite.Models.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Customer
{
    public class CustomerViewModel : BasePersonViewModel
    {
        public bool IsAdult { get; set; }
        public ICollection<ReservationViewModel> Reservations { get; set; }
    }

}
