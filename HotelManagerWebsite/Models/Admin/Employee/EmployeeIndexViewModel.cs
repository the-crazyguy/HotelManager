using HotelManagerWebsite.Models.Base;
using HotelManagerWebsite.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Admin.Employee
{
    public class EmployeeIndexViewModel : BaseIndexViewModel
    {
        public IEnumerable<Admin.Employee.EmployeeViewModel> Items { get; set; }
        public EmployeeFilterViewModel Filter { get; set; }
    }
}
