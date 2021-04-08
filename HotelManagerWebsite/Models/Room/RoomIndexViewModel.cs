using HotelManagerWebsite.Models.Base;
using HotelManagerWebsite.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Room
{
    public class RoomIndexViewModel : BaseIndexViewModel
    {
        public IEnumerable<RoomViewModel> Items { get; set; }
        public RoomFilterViewModel Filter { get; set; }
    }
}
