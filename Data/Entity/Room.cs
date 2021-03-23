using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class Room : BaseEntity
    {
        public int Capacity { get; set; }
        public string Type { get; set; }    //TODO: Make it enum
        public bool IsAvailable { get; set; }
        public double AdultBedPrice { get; set; }
        public double ChildBedPrice { get; set; }
        public int RoomId { get; set; }
    }
}
