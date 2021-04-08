using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public override async Task<int> Add(Room item)
        {
            item.IsAvailable = true;
            _dbContext.Add(item);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
