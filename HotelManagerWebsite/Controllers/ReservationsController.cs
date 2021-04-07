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
                RoomId = item.RoomId,
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
                //If there are no free rooms or customers, do not allow a reservation to be made
                if (_roomRepository.Items.Where(item => item.IsAvailable == true).Count() == 0 || _customerRepository.Items.Count() == 0)
                {
                    return RedirectToAction("Index");
                }

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
                    Rooms = _roomRepository.Items.Where(item => item.IsAvailable == true).Select(item => new RoomPair()
                    {
                        Id = item.Id,
                        RoomNumber = item.RoomNumber
                    }).ToList(),
                    SelectedCustomerIds = new List<int>()
                };
            }
            else
            {
                //If there are no free rooms or customers, do not allow a reservation to be made
                if (_roomRepository.Items.Where(item => item.IsAvailable == true || item.Id == reservation.Room.Id).Count() == 0 ||
                    _customerRepository.Items.Count() == 0)
                {
                    return RedirectToAction("Index");
                }

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
                    Rooms = _roomRepository.Items.Where(item => item.IsAvailable == true || item.Id == reservation.RoomId).Select(item => new RoomPair()
                    {
                        Id = item.Id,
                        RoomNumber = item.RoomNumber
                    }).ToList()
                };
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReservationEditViewModel model)
        {
            //TODO: Fix Invalid ModelState
            if (!ModelState.IsValid)
            {
                //Repopulate the customer and room pairs to prevent crashing
                model.Customers = _customerRepository.Items.Select(item => new CustomerPair()
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName
                }).ToList();

                model.Rooms = _roomRepository.Items.Where(item => item.IsAvailable == true || item.Id == model.RoomId).Select(item => new RoomPair()
                {
                    Id = item.Id,
                    RoomNumber = item.RoomNumber
                }).ToList();

                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

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

            //WARNING: The website gets stuck after redirecting from the post request 
            //TODO: Fix the problem
            await _reservationRepository.AddOrUpdate(reservation);

            return RedirectToAction("Index");
        }

        //Vacate the room but keep the reservation in the db
        [HttpGet]
        public IActionResult MarkAsDone(int id)
        {
            Reservation reservation = _reservationRepository.Items.FirstOrDefault(item => item.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            VacateRoom(reservation.RoomId);

            return RedirectToAction("Index");
        }

        private void VacateRoom(int roomId)
        {
            Room room = _roomRepository.Items.FirstOrDefault(item => item.Id == roomId);

            if (room == null)
            {
                return;
            }

            room.IsAvailable = true;

            _roomRepository.Update(room);
        }

        //Vacate the room and delete the reservation
        [HttpGet]
        public IActionResult Delete(int id)
        {
            Reservation reservation = _reservationRepository.Items.FirstOrDefault(item => item.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            VacateRoom(reservation.RoomId);
            _reservationRepository.Delete(reservation);

            return RedirectToAction("Index");
        }

    }
}
