using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class Reservation : BaseEntity
    {
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }
        public string CreatorId { get; set; }
        public virtual EmployeeUser Creator { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public bool BreakfastIncluded { get; set; }
        public bool IsAllInclusive { get; set; }
        public double TotalSum { get; set; }
    }
}
