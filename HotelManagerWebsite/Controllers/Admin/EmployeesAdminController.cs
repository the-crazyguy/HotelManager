using Data.Entity;
using Data.Repositories;
using HotelManagerWebsite.Models;
using HotelManagerWebsite.Models.Admin.Employee;
using HotelManagerWebsite.Models.Filters;
using HotelManagerWebsite.Models.Utility;
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

        public IActionResult Index(EmployeeIndexViewModel model)
        {
            //1. Initialize pager
            model.Pager = model.Pager ?? new PagerViewModel();
            model.Pager.CurrentPage = model.Pager.CurrentPage <= 0 ? 1 : model.Pager.CurrentPage;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage <= 0 ? 10 : model.Pager.ItemsPerPage;

            //2. Initialize Filter
            model.Filter = model.Filter ?? new EmployeeFilterViewModel();

            //3. Check if the filter is active
            bool emptyUsername = string.IsNullOrWhiteSpace(model.Filter.Username);
            bool emptyFirstName = string.IsNullOrWhiteSpace(model.Filter.Username);
            bool emptyLastName = string.IsNullOrWhiteSpace(model.Filter.Username);
            bool emptyEmail = string.IsNullOrWhiteSpace(model.Filter.Username);

            //4. Query
            IQueryable<Employee> employees = _employeeRepository.Items
                .Where(item =>
                (emptyUsername || item.Username.Contains(model.Filter.Username)) &&
                (emptyFirstName || item.FirstName.Contains(model.Filter.FirstName)) &&
                (emptyLastName || item.LastName.Contains(model.Filter.LastName)) &&
                (emptyEmail || item.Email.Contains(model.Filter.Email)));

            //5. Build view model object
            model.Pager.Pages = (int)Math.Ceiling((double)employees.Count() / model.Pager.ItemsPerPage);

            employees = employees.OrderBy(item => item.Id)
                .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                .Take(model.Pager.ItemsPerPage);

            model.Items = employees.Select(item => new EmployeeViewModel()
            {
                Id = item.Id,
                FirstName = item.FirstName,
                MiddleName = item.MiddleName,
                LastName = item.LastName,
                Username = item.Username,
                Password = item.Password,
                Email = item.Email,
                PhoneNumber = item.PhoneNumber,
                EGN = item.EGN,
                Reservations = item.Reservations.Select(res => new ReservationViewModel()
                {
                    Id = res.Id,
                    RoomId = res.RoomId,
                    Room = new RoomViewModel()
                    {
                        Id = res.Room.Id
                        //TODO: Finish RoomVM
                    },
                    CreatorId = res.CreatorId,
                    Creator = new EmployeeViewModel()
                    {
                        Id = res.Creator.Id,
                        FirstName = res.Creator.FirstName,
                        MiddleName = res.Creator.MiddleName,
                        LastName = res.Creator.LastName,
                        Username = res.Creator.Username,
                        Email = res.Creator.Email,
                        PhoneNumber = res.Creator.PhoneNumber
                    },
                    Customers = res.Customers.Select(c => new CustomerViewModel()
                    {
                        Id = c.Id,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Email = c.Email,
                        PhoneNumber = c.PhoneNumber
                        //TODO: Finish CustomerVM
                    }).ToList(),
                    Arrival = res.Arrival,
                    Departure = res.Departure,
                    BreakfastIncluded = res.BreakfastIncluded,
                    IsAllInclusive = res.IsAllInclusive,
                    TotalSum = res.TotalSum
                }).ToList(),
                Hired = item.Hired,
                IsActive = item.IsActive,
                Fired = item.Fired
            });

            return View(model);
        }
    }
}
