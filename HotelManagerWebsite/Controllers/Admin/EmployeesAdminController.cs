﻿using Data.Entity;
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
        private readonly IPasswordHasher<EmployeeUser> _passwordHasher;

        public EmployeesAdminController(UserManager<EmployeeUser> userManager, RoleManager<IdentityRole> roleManager, IPasswordHasher<EmployeeUser> passwordHasher)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _passwordHasher = passwordHasher;
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                EmployeeUser user = new EmployeeUser()
                {
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    EGN = model.EGN,
                    Hired = DateTime.Now,
                    Reservations = new List<Reservation>(),
                    IsActive = true
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //Assign role
                    EmployeeUser createdUser = await _userManager.FindByEmailAsync(user.Email);
                    await _userManager.AddToRoleAsync(createdUser, WebConstants.EmployeeRole);

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            EmployeeUser user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                //If the user wasn't found, return to index
                return RedirectToAction("Index");
            }

            EmployeeEditViewModel model = new EmployeeEditViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EGN = user.EGN,
                Hired = user.Hired,
                IsActive = user.IsActive,
                Fired = user.Fired,
                Reservations = user.Reservations
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(EmployeeEditViewModel model)
        {
            EmployeeUser user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(model);
            }

            user.FirstName = model.FirstName;
            user.MiddleName = model.MiddleName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.EGN = model.EGN;
            user.Hired = model.Hired;
            user.IsActive = model.IsActive;
            user.Fired = model.Fired;
            user.Reservations = model.Reservations;
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            IdentityResult result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
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
