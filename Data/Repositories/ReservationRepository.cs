using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly HotelDb _dbContext;

        public ReservationRepository(HotelDb dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<Reservation> Items => _dbContext.Reservations;

        public int Add(Reservation reservation)
        {
            _dbContext.Add(reservation);
            return _dbContext.SaveChanges();
        }

        public int Update(Reservation reservation)
        {
            _dbContext.Update(reservation);
            return _dbContext.SaveChanges();
        }

        public int AddOrUpdate(Reservation reservation)
        {
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

        public int Delete(Reservation reservation)
        {
            _dbContext.Remove(reservation);
            return _dbContext.SaveChanges();
        }
    }
}
