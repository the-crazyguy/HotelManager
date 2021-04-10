using HotelManagerWebsite.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Filters
{
    public class ReservationFilterViewModel : FilterViewModel
    {
        public string CreatorName { get; set; }
        public DateTime? AfterDate { get; set; }
        public DateTime? BeforeDate { get; set; }
    }
}
