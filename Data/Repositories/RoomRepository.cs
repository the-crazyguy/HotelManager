using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repositories
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        private readonly HotelDb _dbContext;

        public RoomRepository(HotelDb dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Room> Items => _dbContext.Rooms;
    }
}
