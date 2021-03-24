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
    }
}
