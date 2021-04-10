using Data.Entity;
using HotelManagerWebsite.Models.Base;
using HotelManagerWebsite.Models.Customer;
using HotelManagerWebsite.Models.Room;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Reservation
{
    public class ReservationEditViewModel : BaseEditViewModel
    {
        public string CreatorId { get; set; }
        public EmployeeUser Creator { get; set; }
        public int RoomId { get; set; }
        public ICollection<RoomPair> Rooms { get; set; }
        public ICollection<CustomerPair> Customers { get; set; }
        public ICollection<int> SelectedCustomerIds { get; set; }

        [Required(ErrorMessage = "Please enter an arrival date")]
        public DateTime Arrival { get; set; }

        [Required(ErrorMessage = "Please enter a departure date")]
        public DateTime Departure { get; set; }
        public bool BreakfastIncluded { get; set; }
        public bool IsAllInclusive { get; set; }
    }
}
