using Data.Entity;
using Data.Repositories;
using HotelManagerWebsite.Models;
using HotelManagerWebsite.Models.Admin.Employee;
using HotelManagerWebsite.Models.Reservation;
using HotelManagerWebsite.Models.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class ReservationsController : Controller
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationsController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
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
                //TEMPORARY
                Customers = new List<CustomerViewModel>(),
                Arrival = reservation.Arrival,
                Departure = reservation.Departure,
                BreakfastIncluded = reservation.BreakfastIncluded,
                IsAllInclusive = reservation.IsAllInclusive,
                TotalSum = reservation.TotalSum
            };

            return View(model);
        }


    }
}
