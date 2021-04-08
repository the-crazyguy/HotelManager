using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entity;
using Data.Repositories;
using HotelManagerWebsite.Models;
using HotelManagerWebsite.Models.Customer;
using HotelManagerWebsite.Models.Filters;
using HotelManagerWebsite.Models.Reservation;
using HotelManagerWebsite.Models.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagerWebsite.Controllers
{
    [Authorize(Roles = "Admin,Employees")]
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IActionResult Index(CustomerIndexViewModel model)
        {
            //1. Initialize pager
            #region Pagination
            model.Pager = model.Pager ?? new PagerViewModel();
            model.Pager.CurrentPage = model.Pager.CurrentPage <= 0 ? 1 : model.Pager.CurrentPage;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage <= 0 ? 10 : model.Pager.ItemsPerPage;
            #endregion

            #region Filter
            //2. Initialize Filter
            model.Filter = model.Filter ?? new EmployeeFilterViewModel();

            //TODO:Add more filter fields
            //3. Check if the filter is active
            bool emptyFirstName = string.IsNullOrWhiteSpace(model.Filter.FirstName);
            bool emptyLastName = string.IsNullOrWhiteSpace(model.Filter.LastName);
            bool emptyEmail = string.IsNullOrWhiteSpace(model.Filter.Email);

            //4. Query
            IQueryable<Customer> customers = _customerRepository.Items
                .Where(item =>
                (emptyFirstName || item.FirstName.Contains(model.Filter.FirstName)) &&
                (emptyLastName || item.LastName.Contains(model.Filter.LastName)) &&
                (emptyEmail || item.Email.Contains(model.Filter.Email)));
            #endregion

            //5. Build view model object
            //Calculate total pages
            model.Pager.Pages = (int)Math.Ceiling((double)customers.Count() / model.Pager.ItemsPerPage);

            //Calculate which customers to show on the current page and order them by whether they are adults or not
            customers = customers.OrderByDescending(item => item.IsAdult)
                .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                .Take(model.Pager.ItemsPerPage);

            model.Items = customers.Select(item => new CustomerViewModel()
            {
                Id = item.Id,
                FirstName = item.FirstName,
                LastName = item.LastName,
                PhoneNumber = item.PhoneNumber,
                Email = item.Email,
                IsAdult = item.IsAdult,
                Reservations = item.CustomerReservations.Select(cr => new ReservationViewModel()
                {
                    Id = cr.Reservation.Id,
                    RoomId = cr.Reservation.RoomId,
                    //Room 
                    CreatorId = cr.Reservation.CreatorId,
                    //Creator
                    //Customers
                    Arrival = cr.Reservation.Arrival,
                    Departure = cr.Reservation.Departure,
                    BreakfastIncluded = cr.Reservation.BreakfastIncluded,
                    IsAllInclusive = cr.Reservation.IsAllInclusive,
                    TotalSum = cr.Reservation.TotalSum
                }).ToList()
            });

            return View(model);
        }
        
        [HttpGet]
        public IActionResult Details(int id)
        {
            Customer customer = _customerRepository.Items.SingleOrDefault(item => item.Id == id);

            if(customer==null)
            {
                return NotFound();
            }

            CustomerViewModel model = new CustomerViewModel()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                IsAdult = customer.IsAdult,
                Reservations = customer.CustomerReservations.Select(cr => new ReservationViewModel()
                {
                    Id = cr.Reservation.Id,
                    RoomId = cr.Reservation.RoomId,
                    //Room
                    CreatorId = cr.Reservation.CreatorId,
                    //Creator
                    Customers = _customerRepository.Items.Select(crr => new CustomerViewModel()
                    {
                        FirstName = cr.Customer.FirstName,
                        LastName = cr.Customer.LastName,
                        Email = cr.Customer.Email 
                    }).ToList(),
                    Arrival = cr.Reservation.Arrival,
                    Departure = cr.Reservation.Departure,
                    BreakfastIncluded = cr.Reservation.BreakfastIncluded,
                    IsAllInclusive = cr.Reservation.IsAllInclusive,
                    TotalSum = cr.Reservation.TotalSum
                }).ToList()
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult EditInitial()
        {
            CustomerEditViewModel model;

            model = new CustomerEditViewModel();

            return View(model);
        }

        [HttpGet]
        public IActionResult EditCheck(CustomerEditViewModel customerToCheck)
        {
            Customer customer = _customerRepository.Items.SingleOrDefault(item => item.Email == customerToCheck.Email);

            CustomerEditViewModel model;

            if(customer==null)
            {
                model = new CustomerEditViewModel()
                {
                    Email = customerToCheck.Email
                };
            }
            else
            {
                model = new CustomerEditViewModel()
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    PhoneNumer = customer.PhoneNumber,
                    Email = customer.Email,
                    IsAdult = customer.IsAdult
                };
            }

            return RedirectToAction("Edit", model);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            Customer customer = _customerRepository.Items.SingleOrDefault(item => item.Id == id);

            CustomerEditViewModel model;

            if(customer==null)
            {
                model = new CustomerEditViewModel();
            }
            else
            {
                model = new CustomerEditViewModel()
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    PhoneNumer = customer.PhoneNumber,
                    Email = customer.Email,
                    IsAdult = customer.IsAdult
                };
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerEditViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            await _customerRepository.AddOrUpdate(new Customer()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumer,
                Email = model.Email,
                IsAdult = model.IsAdult
            });

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Customer customer = _customerRepository.Items.SingleOrDefault(item => item.Id == id);

            if(customer==null)
            {
                return NotFound();
            }
            else
            {
                await _customerRepository.Delete(customer);
                return RedirectToAction("Index");
            }
        }
    }
}
