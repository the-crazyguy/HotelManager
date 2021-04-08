using HotelManagerWebsite.Models.Base;
using HotelManagerWebsite.Models.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Room
{
    public class RoomViewModel : BaseViewModel
    {
        public int Capacity { get; set; }
        public string Type { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<ReservationViewModel> Reservations { get; set; }
        public double AdultBedPrice { get; set; }
        public double ChildBedPrice { get; set; }
        public int RoomNumber { get; set; }

    }
}
