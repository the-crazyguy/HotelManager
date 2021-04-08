using HotelManagerWebsite.Models.Base;
using HotelManagerWebsite.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Customer
{
    public class CustomerIndexViewModel : BaseIndexViewModel
    {
        public IEnumerable<CustomerViewModel> Items { get; set; }
        public EmployeeFilterViewModel Filter { get; set; }
    }
}
