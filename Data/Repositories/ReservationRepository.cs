using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override async Task<int> Add(Reservation reservation)
        {
            await _dbContext.AddAsync(reservation);

            foreach (var item in reservation.CustomerReservations)
            {
                await _dbContext.CustomerReservations.AddAsync(item);
            }

            //Mark the room as taken
            _dbContext.Rooms.FindAsync(reservation.RoomId).Result.IsAvailable = false;

            return await _dbContext.SaveChangesAsync();
        }

        public override async Task<int> Update(Reservation reservation)
        {
            _dbContext.Update(reservation);

            foreach (var item in reservation.CustomerReservations)
            {
                await _dbContext.CustomerReservations.AddAsync(item);
            }

            //Mark the room as taken if it wasn't already
            _dbContext.Rooms.FindAsync(reservation.RoomId).Result.IsAvailable = false;

            return await _dbContext.SaveChangesAsync();
        }

        public override async Task<int> AddOrUpdate(Reservation reservation)
        {
            _dbContext.CustomerReservations.RemoveRange(
                _dbContext.CustomerReservations.Where(cr => cr.ReservationId == reservation.Id)
                );

            if (reservation.Id == 0)
            {
                //The Id is 0 so we are adding a new reservation
                return await Add(reservation);
            }
            else
            {
                //The Id exists, so we update the reservation
                return await Update(reservation);
            }
        }
    }
}
