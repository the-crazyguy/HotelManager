using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    class Reservation : BaseEntity
    {
        public Room ReservedRoom { get; set; }
        public Employee EmployeeCreator { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public bool BreakfastIncluded { get; set; }
        public bool IsAllInclusive { get; set; }
        public double TotalSum { get; set; }
    }
}
