using Data.Entity;
using Data.Repositories;
using HotelManagerWebsite.Models;
using HotelManagerWebsite.Models.Admin.Employee;
using HotelManagerWebsite.Models.Filters;
using HotelManagerWebsite.Models.Reservation;
using HotelManagerWebsite.Models.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class EmployeesAdminController : Controller
    {
        private readonly UserManager<EmployeeUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeesAdminController(UserManager<EmployeeUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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

            //Calculate which users to show on the current page and order them by whether they are active or not
            employeeUsers = employeeUsers.OrderByDescending(item => item.IsActive)
                .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                .Take(model.Pager.ItemsPerPage);

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
                    CreatorId = res.CreatorId,
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
        public IActionResult Details(string id)
        {
            EmployeeUser employeeUser = _userManager.Users.FirstOrDefault(item => item.Id == id);

            if (employeeUser == null)
            {
                return NotFound();
            }

            EmployeeViewModel model = new EmployeeViewModel()
            {
                Id = employeeUser.Id,
                FirstName = employeeUser.FirstName,
                MiddleName = employeeUser.MiddleName,
                LastName = employeeUser.LastName,
                UserName = employeeUser.UserName,
                Email = employeeUser.Email,
                PhoneNumber = employeeUser.PhoneNumber,
                EGN = employeeUser.EGN,
                Reservations = employeeUser.Reservations.Select(res => new ReservationViewModel()
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
                Hired = employeeUser.Hired,
                IsActive = employeeUser.IsActive,
                Fired = employeeUser.Fired
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            return NotFound();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Fire(string id)
        {
            EmployeeUser employeeUser = _userManager.Users.FirstOrDefault(item => item.Id == id);

            if (employeeUser == null)
            {
                return NotFound();
            }

            //If a "Fired" role does not exist, create it
            if (!await _roleManager.RoleExistsAsync(WebConstants.FiredRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(WebConstants.FiredRole));
            }

            bool isAdmin = await _userManager.IsInRoleAsync(employeeUser, WebConstants.AdminRole);
            bool isFired = await _userManager.IsInRoleAsync(employeeUser, WebConstants.FiredRole);

            //We cannot fire other admins or already fired users
            if (!isAdmin && !isFired)
            {
                employeeUser.IsActive = false;
                employeeUser.Fired = DateTime.Now;
                await _userManager.AddToRoleAsync(employeeUser, WebConstants.FiredRole);
            }

            
            return RedirectToAction("Index");
        }
    }
}
