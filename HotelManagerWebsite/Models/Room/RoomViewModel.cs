using HotelManagerWebsite.Models.Base;
using HotelManagerWebsite.Models.Reservation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Room
{
    public class RoomViewModel : BaseViewModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Capacity { get; set; }
        public string Type { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<ReservationViewModel> Reservations { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a price bigger than {1}")]
        public double AdultBedPrice { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a price bigger than {1}")]
        public double ChildBedPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int RoomNumber { get; set; }

    }
}
