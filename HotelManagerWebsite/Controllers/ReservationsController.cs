using Data.Entity;
using Data.Repositories;
using HotelManagerWebsite.Models;
using HotelManagerWebsite.Models.Admin.Employee;
using HotelManagerWebsite.Models.Customer;
using HotelManagerWebsite.Models.Reservation;
using HotelManagerWebsite.Models.Room;
using HotelManagerWebsite.Models.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class ReservationsController : Controller
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly UserManager<EmployeeUser> _userManager;

        public ReservationsController(IReservationRepository reservationRepository, 
            ICustomerRepository customerRepository,
            IRoomRepository roomRepository,
            UserManager<EmployeeUser> userManager)
        {
            _reservationRepository = reservationRepository;
            _customerRepository = customerRepository;
            _roomRepository = roomRepository;
            _userManager = userManager;
        }

        public IActionResult Index(ReservationIndexViewModel model)
        {
            //1. Initialize Pager
            #region Pagination
            model.Pager = model.Pager ?? new PagerViewModel();
            model.Pager.CurrentPage = model.Pager.CurrentPage <= 0 ? 1 : model.Pager.CurrentPage;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage <= 0 ? 10 : model.Pager.ItemsPerPage;
            #endregion

            //2. Get Reservations from db
            IQueryable<Reservation> reservations = _reservationRepository.Items;

            //3. Build view model objects
            //Calculate total pages
            model.Pager.Pages = (int)Math.Ceiling((double)reservations.Count() / model.Pager.ItemsPerPage);

            //Calculate which reservations to show on the current page
            reservations = reservations.OrderBy(item => item.Id)
                .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                .Take(model.Pager.ItemsPerPage);

            //TODO: Add Room, Creator and Customers
            //Make viewmodels from the Reservation items to show in the View
            model.Items = reservations.Select(item => new ReservationViewModel() 
            {
                Id = item.Id,
                RoomId = item.Id,
                //Room
                CreatorId = item.CreatorId,
                Creator = new EmployeeViewModel()
                {
                    Id = item.Creator.Id,
                    FirstName = item.Creator.FirstName,
                    MiddleName = item.Creator.MiddleName,
                    LastName = item.Creator.LastName
                },
                //Customers
                Arrival = item.Arrival,
                Departure = item.Departure,
                BreakfastIncluded = item.BreakfastIncluded,
                IsAllInclusive = item.IsAllInclusive,
                TotalSum = item.TotalSum
            });

            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Reservation reservation = _reservationRepository.Items.FirstOrDefault(item => item.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            ReservationViewModel model = new ReservationViewModel()
            {
                Id = reservation.Id,
                RoomId = reservation.Id,
                //Room
                CreatorId = reservation.CreatorId,
                Creator = new EmployeeViewModel()
                {
                    Id = reservation.Creator.Id,
                    FirstName = reservation.Creator.FirstName,
                    MiddleName = reservation.Creator.MiddleName,
                    LastName = reservation.Creator.LastName
                },
                //TODO: TEMPORARY
                Customers = new List<CustomerViewModel>(),
                Arrival = reservation.Arrival,
                Departure = reservation.Departure,
                BreakfastIncluded = reservation.BreakfastIncluded,
                IsAllInclusive = reservation.IsAllInclusive,
                TotalSum = reservation.TotalSum
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            Reservation reservation = _reservationRepository.Items.FirstOrDefault(item => item.Id == id);

            ReservationEditViewModel model;

            if (reservation == null)
            {
                //No Reservation found, so we create a new one
                model = new ReservationEditViewModel()
                {
                    Id = 0,
                    Customers = _customerRepository.Items.Select(item => new CustomerPair()
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName
                    }).ToList(),
                    Rooms = _roomRepository.Items.Select(item => new RoomPair()
                    {
                        Id = item.Id,
                        Type = item.Type
                    }).ToList()
                };
            }
            else
            {
                //A Reservation was found, so we edit it
                model = new ReservationEditViewModel()
                {
                    Id = reservation.Id,
                    RoomId = reservation.RoomId,
                    CreatorId = reservation.CreatorId,
                    Arrival = reservation.Arrival,
                    Departure = reservation.Departure,
                    BreakfastIncluded = reservation.BreakfastIncluded,
                    IsAllInclusive = reservation.IsAllInclusive,
                    TotalSum = reservation.TotalSum,
                    Customers = _customerRepository.Items.Select(item => new CustomerPair()
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName
                    }).ToList(),
                    SelectedCustomerIds = reservation.CustomerReservations.Select(cr => cr.CustomerId).ToList(),
                    Rooms = _roomRepository.Items.Select(item => new RoomPair()
                    {
                        Id = item.Id,
                        Type = item.Type
                    }).ToList()
                };
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReservationEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            //TODO: Get the room from roomRepo and mark it as taken

            Reservation reservation = new Reservation()
            {
                Id = model.Id,
                RoomId = model.RoomId,
                CreatorId = user.Id,
                Arrival = model.Arrival,
                Departure = model.Departure,
                BreakfastIncluded = model.BreakfastIncluded,
                IsAllInclusive = model.IsAllInclusive,
                TotalSum = model.TotalSum,
                CustomerReservations = model.SelectedCustomerIds.Select(customerId => new CustomerReservation() 
                {
                    CustomerId = customerId,
                    ReservationId = model.Id
                }).ToList()
            };

            _reservationRepository.AddOrUpdate(reservation);

            return RedirectToAction("Index");
        }

    }
}
