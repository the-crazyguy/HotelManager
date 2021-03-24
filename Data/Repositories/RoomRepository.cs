using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelDb _dbContext;

        public RoomRepository(HotelDb dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Room> Items => _dbContext.Rooms;

        public int Add(Room room)
        {
            _dbContext.Add(room);
            return _dbContext.SaveChanges();
        }

        public int Update(Room room)
        {
            _dbContext.Update(room);
            return _dbContext.SaveChanges();
        }

        public int AddOrUpdate(Room room)
        {
            if (room.Id == 0)
            {
                //The Id is 0 so we are adding a new room
                return Add(room);
            }
            else
            {
                //The Id exists, so we update the room
                return Update(room);
            }
        }

        public int Delete(Room room)
        {
            _dbContext.Remove(room);
            return _dbContext.SaveChanges();
        }
    }
}
