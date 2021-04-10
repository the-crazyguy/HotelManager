using HotelManagerWebsite.Models.Base;
using HotelManagerWebsite.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Reservation
{
    public class ReservationIndexViewModel : BaseIndexViewModel
    {
        public IEnumerable<ReservationViewModel> Items { get; set; }
        public ReservationFilterViewModel Filter { get; set; }
    }
}
