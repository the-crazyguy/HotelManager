using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entity;
using Data.Repositories;
using HotelManagerWebsite.Models.Filters;
using HotelManagerWebsite.Models.Reservation;
using HotelManagerWebsite.Models.Room;
using HotelManagerWebsite.Models.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagerWebsite.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class RoomsController : Controller
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IReservationRepository _reservationRepository;

        public RoomsController(IRoomRepository roomRepository, IReservationRepository reservationRepository)
        {
            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
        }
        public IActionResult Index(RoomIndexViewModel model)
        {
            //1. Initialize pager
            #region Pagination
            model.Pager = model.Pager ?? new PagerViewModel();
            model.Pager.CurrentPage = model.Pager.CurrentPage <= 0 ? 1 : model.Pager.CurrentPage;
            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage <= 0 ? 10 : model.Pager.ItemsPerPage;
            #endregion

            #region Filter
            //2. Initialize Filter
            model.Filter = model.Filter ?? new RoomFilterViewModel();


            //TODO:Fix Filters
            //3. Check if the filter is active
            bool emptyCapacity = (model.Filter.Capacity<=0);
            bool emptyType = string.IsNullOrWhiteSpace(model.Filter.Type);
            bool emptyIsAvailable = !model.Filter.IsAvailable;

            //4. Query
            IQueryable<Room> rooms = _roomRepository.Items
                .Where(item =>
                (emptyCapacity || (item.Capacity == model.Filter.Capacity)) &&
                (emptyType || (item.Type == model.Filter.Type)) &&
                (emptyIsAvailable || (item.IsAvailable == model.Filter.IsAvailable)));
            #endregion Filter

            //5. Build view model object
            //Calculate total pages
            model.Pager.Pages = (int)Math.Ceiling((double)rooms.Count() / model.Pager.ItemsPerPage);

            //Calculate which rooms to show on the current page and order them by whether they are available or not
            rooms = rooms.OrderByDescending(item => item.IsAvailable).ThenBy(item=> item.RoomNumber)
                .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                .Take(model.Pager.ItemsPerPage);

            model.Items = rooms.Select(item => new RoomViewModel()
            {
                Id = item.Id,
                Capacity = item.Capacity,
                Type = item.Type,
                IsAvailable = item.IsAvailable,
                //Reservation
                AdultBedPrice = item.AdultBedPrice,
                ChildBedPrice = item.ChildBedPrice,
                RoomNumber = item.RoomNumber
            });

            return View(model);
        }

        
        [HttpGet]
        public IActionResult Details(int id)
        {
            Room room = _roomRepository.Items.SingleOrDefault(item => item.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            RoomViewModel model = new RoomViewModel()
            {
                Id = room.Id,
                Capacity = room.Capacity,
                Type = room.Type,
                IsAvailable = room.IsAvailable,
                Reservations = _reservationRepository.Items.Select(cr => new ReservationViewModel()
                {
                    Id = cr.Id,
                    CreatorId = cr.CreatorId,
                    //Creator
                    /*Customers = _customerRepository.Items.Select(crr => new CustomerViewModel()
                    {
                        FirstName = cr.Customer.FirstName,
                        LastName = cr.Customer.LastName,
                        Email = cr.Customer.Email
                    }).ToList(),*/
                    Arrival = cr.Arrival,
                    Departure = cr.Departure,
                    BreakfastIncluded = cr.BreakfastIncluded,
                    IsAllInclusive = cr.IsAllInclusive,
                    TotalSum = cr.TotalSum

                }).ToList(),
                AdultBedPrice = room.AdultBedPrice,
                ChildBedPrice = room.ChildBedPrice,
                RoomNumber = room.RoomNumber
            };

            return View(model);

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            Room room = _roomRepository.Items.SingleOrDefault(item => item.Id == id);

            RoomEditViewModel model;

            if (room == null)
            {
                model = new RoomEditViewModel();
            }
            else
            {
                model = new RoomEditViewModel()
                {
                    Id = room.Id,
                    Capacity = room.Capacity,
                    Type = room.Type,
                    AdultBedPrice = room.AdultBedPrice,
                    ChildBedPrice = room.ChildBedPrice,
                    RoomNumber = room.RoomNumber,
                    IsAvailable = room.IsAvailable
                };
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoomEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _roomRepository.AddOrUpdate(new Room()
            {
                Id = model.Id,
                Capacity = model.Capacity,
                Type = model.Type,
                AdultBedPrice = model.AdultBedPrice,
                ChildBedPrice = model.ChildBedPrice,
                RoomNumber = model.RoomNumber,
                IsAvailable = model.IsAvailable
            });

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Room room = _roomRepository.Items.SingleOrDefault(item => item.Id == id);

            if (room == null)
            {
                return NotFound();
            }
            else
            {
                await _roomRepository.Delete(room);
                return RedirectToAction("Index");
            }
        }

    }
}
