using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repositories
{
    public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
    {
        private readonly HotelDb _dbContext;

        public ReservationRepository(HotelDb dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Reservation> Items => _dbContext.Reservations;

        public override int Add(Reservation reservation)
        {
            _dbContext.Add(reservation);

            foreach (var item in reservation.CustomerReservations)
            {
                _dbContext.CustomerReservations.Add(item);
            }

            //Mark the room as taken
            _dbContext.Rooms.Find(reservation.RoomId).IsAvailable = false;

            return _dbContext.SaveChanges();
        }

        public override int Update(Reservation reservation)
        {
            _dbContext.Update(reservation);

            foreach (var item in reservation.CustomerReservations)
            {
                _dbContext.CustomerReservations.Add(item);
            }

            //Mark the room as taken if it wasn't already
            _dbContext.Rooms.Find(reservation.RoomId).IsAvailable = false;

            return _dbContext.SaveChanges();
        }

        public override int AddOrUpdate(Reservation reservation)
        {
            _dbContext.CustomerReservations.RemoveRange(
                _dbContext.CustomerReservations.Where(cr => cr.ReservationId == reservation.Id)
                );

            if (reservation.Id == 0)
            {
                //The Id is 0 so we are adding a new reservation
                return Add(reservation);
            }
            else
            {
                //The Id exists, so we update the reservation
                return Update(reservation);
            }
        }
    }
}
