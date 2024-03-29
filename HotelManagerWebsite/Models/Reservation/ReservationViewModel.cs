﻿using HotelManagerWebsite.Models.Admin.Employee;
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
    public class ReservationViewModel : BaseViewModel
    {
        public int RoomId { get; set; }
        public RoomViewModel Room { get; set; }
        public string CreatorId { get; set; }
        public EmployeeViewModel Creator { get; set; }
        public ICollection<CustomerViewModel> Customers { get; set; }

        [Required(ErrorMessage = "Please enter an arrival date")]
        public DateTime Arrival { get; set; }

        [Required(ErrorMessage = "Please enter a departure date")]
        public DateTime Departure { get; set; }
        public bool BreakfastIncluded { get; set; }
        public bool IsAllInclusive { get; set; }
        public double TotalSum { get; set; }
    }
}
