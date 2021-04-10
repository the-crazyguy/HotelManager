using Data.Entity;
using Data.Repositories;
using HotelManagerWebsite.Models;
using HotelManagerWebsite.Models.Admin.Employee;
using HotelManagerWebsite.Models.Customer;
using HotelManagerWebsite.Models.Filters;
using HotelManagerWebsite.Models.Reservation;
using HotelManagerWebsite.Models.Room;
using HotelManagerWebsite.Models.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            //2. Filter
            model.Filter = model.Filter ?? new ReservationFilterViewModel();

            //Check for empty filters
            bool emptyCreatorName = string.IsNullOrWhiteSpace(model.Filter.CreatorName);
            bool emptyAfterDate = model.Filter.AfterDate == null;
            bool emptyBeforeDate = model.Filter.BeforeDate == null;

            //3. Get Reservations from db
            List<Reservation> reservations = _reservationRepository.Items.Include("CustomerReservations").AsEnumerable()
                .Where(item =>
                //Get all reservations whose Arrival date is between the after and before filters
                ((emptyAfterDate || Nullable.Compare<DateTime>(item.Arrival, model.Filter.AfterDate) >= 0) && (emptyBeforeDate || Nullable.Compare<DateTime>(item.Arrival, model.Filter.BeforeDate) <= 0)) &&
                (emptyCreatorName || item.Creator.FullName.Contains(model.Filter.CreatorName))
                ).ToList();

            //4. Build view model objects
            //Calculate total pages
            model.Pager.Pages = (int)Math.Ceiling((double)reservations.Count() / model.Pager.ItemsPerPage);

            //Calculate which reservations to show on the current page
            reservations = reservations.OrderBy(item => item.Id)
                .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                .Take(model.Pager.ItemsPerPage).ToList();   //TODO: Remove .ToList()

            //Make viewmodels from the Reservation items to show in the View
            model.Items = reservations.Select(item => new ReservationViewModel() 
            {
                Id = item.Id,
                RoomId = item.RoomId,
                CreatorId = item.CreatorId,
                Creator = new EmployeeViewModel()
                {
                    Id = item.Creator.Id,
                    FirstName = item.Creator.FirstName,
                    MiddleName = item.Creator.MiddleName,
                    LastName = item.Creator.LastName
                },
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
                RoomId = reservation.RoomId,
                CreatorId = reservation.CreatorId,
                Creator = new EmployeeViewModel()
                {
                    Id = reservation.Creator.Id,
                    FirstName = reservation.Creator.FirstName,
                    MiddleName = reservation.Creator.MiddleName,
                    LastName = reservation.Creator.LastName
                },
                Customers = reservation.CustomerReservations.Select(item=> new CustomerViewModel()
                {
                    Id = item.CustomerId,
                    FirstName = item.Customer.FirstName,
                    LastName = item.Customer.LastName
                }).ToList(),
                Arrival = reservation.Arrival,
                Departure = reservation.Departure,
                BreakfastIncluded = reservation.BreakfastIncluded,
                IsAllInclusive = reservation.IsAllInclusive,
                TotalSum = reservation.TotalSum
            };

            return View(model);
        }

        private int oldRoomId;

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

                //Keep the old room id
                oldRoomId = reservation.RoomId;

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
            //If the model state is invalid OR Arrival is earlier than now OR later than or the same date as departure, return the view for resubmission
            if (!ModelState.IsValid ||
                DateTime.Compare(model.Arrival, DateTime.Now) < 0 ||
                DateTime.Compare(model.Arrival, model.Departure) >= 0)
            {
                //Repopulate the customer and room pairs for the dropdowns/lists
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

            //Caluclate TotalSum
            #region Sum Calculation

            IQueryable<Customer> selectedCustomers = _customerRepository.Items.Where(item => model.SelectedCustomerIds.Contains(item.Id));
            
            int adults = selectedCustomers.Where(item => item.IsAdult).Count();
            int children = selectedCustomers.Where(item => !item.IsAdult).Count();
            double adultBedPrice = _roomRepository.Items.FirstOrDefault(room => room.Id == model.RoomId).AdultBedPrice;
            double childBedPrice = _roomRepository.Items.FirstOrDefault(room => room.Id == model.RoomId).ChildBedPrice;

            int days = (model.Departure - model.Arrival).Days;

            double totalSum = CalculateTotalSum(adults, adultBedPrice, children, childBedPrice, model.IsAllInclusive, model.BreakfastIncluded, days);
            #endregion

            var user = await _userManager.GetUserAsync(User);

            //Make a Reservation object
            Reservation reservation = new Reservation()
            {
                Id = model.Id,
                RoomId = model.RoomId,
                CreatorId = user.Id,
                Arrival = model.Arrival,
                Departure = model.Departure,
                BreakfastIncluded = model.BreakfastIncluded,
                IsAllInclusive = model.IsAllInclusive,
                TotalSum = totalSum,
                CustomerReservations = model.SelectedCustomerIds.Select(customerId => new CustomerReservation() 
                {
                    CustomerId = customerId,
                    ReservationId = model.Id
                }).ToList()
            };

            //Check if the reservation's room has been changed
            if (oldRoomId != 0 && oldRoomId != reservation.RoomId)
            {
                //Vacate the room that was previously assigned to this reservation
                await VacateRoom(oldRoomId);
                oldRoomId = 0;
            }

            await _reservationRepository.AddOrUpdate(reservation);

            return RedirectToAction("Index");
        }

        private double CalculateTotalSum(int adults, double adultBedPrice, int children, double childBedPrice, bool isAllInclusive, bool breakfastIncluded, int days)
        {
            double pricePerDayAdults = adults * adultBedPrice;
            double pricePerDayChildren = children * childBedPrice;
            double mealsPrice = 0;

            if (breakfastIncluded && isAllInclusive == false)
            {
                mealsPrice = WebConstants.BreakfastPrice;
            }
            else if (isAllInclusive)
            {
                mealsPrice = WebConstants.AllMealsPrice;
            }

            double totalRoomPricePerDay = pricePerDayAdults + pricePerDayChildren + mealsPrice;

            double totalSum = totalRoomPricePerDay * days;

            return totalSum;
        }

        //Vacate the room but keep the reservation in the db
        [HttpGet]
        public async Task<IActionResult> MarkAsDone(int id)
        {
            Reservation reservation = _reservationRepository.Items.FirstOrDefault(item => item.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            await VacateRoom(reservation.RoomId);

            return RedirectToAction("Index");
        }

        private async Task VacateRoom(int roomId)
        {
            Room room = _roomRepository.Items.FirstOrDefault(item => item.Id == roomId);

            if (room == null)
            {
                return;
            }

            room.IsAvailable = true;

            await _roomRepository.Update(room);
        }

        //Vacate the room and delete the reservation
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Reservation reservation = _reservationRepository.Items.FirstOrDefault(item => item.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            await VacateRoom(reservation.RoomId);

            await _reservationRepository.Delete(reservation);

            return RedirectToAction("Index");
        }

    }
}
