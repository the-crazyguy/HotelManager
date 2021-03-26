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

        [HttpGet]
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
            bool emptyFirstName = string.IsNullOrWhiteSpace(model.Filter.FirstName);
            bool emptyMiddleName = string.IsNullOrWhiteSpace(model.Filter.MiddleName);
            bool emptyLastName = string.IsNullOrWhiteSpace(model.Filter.LastName);
            bool emptyEmail = string.IsNullOrWhiteSpace(model.Filter.Email);

            //4. Query
            IQueryable<Employee> employees = _employeeRepository.Items
                .Where(item =>
                (emptyUsername || item.Username.Contains(model.Filter.Username)) &&
                (emptyFirstName || item.FirstName.Contains(model.Filter.FirstName)) &&
                (emptyMiddleName || item.MiddleName.Contains(model.Filter.MiddleName)) &&
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

        [HttpGet]
        public IActionResult Details(int id)
        {
            Employee employee = _employeeRepository.Items.FirstOrDefault(item => item.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            EmployeeViewModel model = new EmployeeViewModel()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                LastName = employee.LastName,
                Username = employee.Username,
                Password = employee.Password,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                EGN = employee.EGN,
                Reservations = employee.Reservations.Select(res => new ReservationViewModel()
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
                Hired = employee.Hired,
                IsActive = employee.IsActive,
                Fired = employee.Fired

            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            Employee employee = _employeeRepository.Items.FirstOrDefault(item => item.Id == id);
            EmployeeEditViewModel model;

            if (employee == null)
            {
                //Employee wasn't found so we want to add one
                model = new EmployeeEditViewModel()
                {
                    Id = 0
                };
            }
            else
            {
                model = new EmployeeEditViewModel()
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    MiddleName = employee.MiddleName,
                    LastName = employee.LastName,
                    Username = employee.Username,
                    Password = employee.Password,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    EGN = employee.EGN,
                    Reservations = employee.Reservations,
                    Hired = employee.Hired,
                    IsActive = employee.IsActive,
                    Fired = employee.Fired
                };
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Employee employee = new Employee()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EGN = model.EGN,
                Reservations = (model.Id == 0 ? new List<Reservation>() : model.Reservations),
                Hired = (model.Id == 0 ? DateTime.Now : model.Hired),
                IsActive = model.IsActive,
                Fired = model.Fired
            };

            _employeeRepository.AddOrUpdate(employee);

            return RedirectToAction("Index");
        }
    }
}
