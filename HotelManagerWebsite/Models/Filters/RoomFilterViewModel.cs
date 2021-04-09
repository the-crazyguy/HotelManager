using HotelManagerWebsite.Models.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Filters
{
    public class RoomFilterViewModel
    {
        public int Capacity { get; set; }
        public string Type { get; set; }
        #endregion
        public bool IsAvailable { get; set; }
    }
}
