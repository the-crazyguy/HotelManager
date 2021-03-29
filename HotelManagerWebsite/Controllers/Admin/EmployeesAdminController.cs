using Data.Entity;
using Data.Repositories;
using HotelManagerWebsite.Models;
using HotelManagerWebsite.Models.Admin.Employee;
using HotelManagerWebsite.Models.Filters;
using HotelManagerWebsite.Models.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Controllers.Admin
{
    public class EmployeesAdminController : Controller
    {
        private readonly UserManager<EmployeeUser> _userManager;

        public EmployeesAdminController(UserManager<EmployeeUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index(EmployeeIndexViewModel model)
        {
            //1. Initialize pager
            #region Pagination
            model.Pager = model.Pager ?? new PagerViewModel();
            model.Pager.CurrentPage = model.Pager.CurrentPage <= 0 ? 1 : model.Pager.CurrentPage;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage <= 0 ? 10 : model.Pager.ItemsPerPage;
            #endregion

            //2. Initialize filter, 3. Check if the filter is active, 4. Query filtered items
            #region Filter
            //2. Initialize Filter
            model.Filter = model.Filter ?? new EmployeeFilterViewModel();

            //3. Check if the filter is active
            bool emptyUsername = string.IsNullOrWhiteSpace(model.Filter.Username);
            bool emptyFirstName = string.IsNullOrWhiteSpace(model.Filter.FirstName);
            bool emptyMiddleName = string.IsNullOrWhiteSpace(model.Filter.MiddleName);
            bool emptyLastName = string.IsNullOrWhiteSpace(model.Filter.LastName);
            bool emptyEmail = string.IsNullOrWhiteSpace(model.Filter.Email);

            //4. Query
            IQueryable<EmployeeUser> employeeUsers = _userManager.Users
                .Where(item =>
                (emptyUsername || item.UserName.Contains(model.Filter.Username)) &&
                (emptyFirstName || item.FirstName.Contains(model.Filter.FirstName)) &&
                (emptyMiddleName || item.MiddleName.Contains(model.Filter.MiddleName)) &&
                (emptyLastName || item.LastName.Contains(model.Filter.LastName)) &&
                (emptyEmail || item.Email.Contains(model.Filter.Email)));
            #endregion

            //5. Build view model object
            //Calculate total pages
            model.Pager.Pages = (int)Math.Ceiling((double)employeeUsers.Count() / model.Pager.ItemsPerPage);

            //Calculate which users to show on the current page
            employeeUsers = employeeUsers.OrderBy(item => item.Id)
                .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                .Take(model.Pager.ItemsPerPage);

            //TODO: Add Room, Creator and Customers
            //Make viewmodels from the EmployeeUser items to show in the View
            model.Items = employeeUsers.Select(item => new EmployeeViewModel()
            {
                Id = item.Id,
                FirstName = item.FirstName,
                MiddleName = item.MiddleName,
                LastName = item.LastName,
                UserName = item.UserName,
                Email = item.Email,
                PhoneNumber = item.PhoneNumber,
                EGN = item.EGN,
                Reservations = item.Reservations.Select(res => new ReservationViewModel() 
                {
                    Id = res.Id,
                    RoomId = res.RoomId,
                    //Room
                    CreatorId = res.CreatorId,
                    //Creator
                    //Customers
                    Arrival = res.Arrival,
                    Departure = res.Departure,
                    BreakfastIncluded = res.BreakfastIncluded,
                    IsAllInclusive = res.IsAllInclusive,
                    TotalSum = res.TotalSum
                }).ToList(),
                Hired = item.Hired,
                isActive = item.IsActive,
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
                
            }
            else
            {
                //The employee exists, so we edit it
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

            _employeeRepository.AddOrUpdate(new Employee()
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
                Reservations = model.Reservations,
                Hired = model.Hired,
                IsActive = model.IsActive,
                Fired = model.Fired
            });

            return RedirectToAction("Index");
        }
    }
}
