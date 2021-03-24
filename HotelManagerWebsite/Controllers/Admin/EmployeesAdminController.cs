using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Controllers.Admin
{
    public class EmployeesAdminController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesAdminController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public IActionResult Index()
        {
            //1. Initialize pager
            //2. Initialize Filter
            //3. Check if the filter is active
            //4. Query
            //5. Build view model object
            return View();
        }
    }
}
