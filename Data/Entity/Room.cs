﻿using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class Room : BaseEntity
    {
        public int Capacity { get; set; }
        public string Type { get; set; }
        public bool IsAvailable { get; set; }
        public virtual Reservation Reservation { get; set; }
        public double AdultBedPrice { get; set; }
        public double ChildBedPrice { get; set; }
        public int RoomNumber { get; set; }
    }
}
