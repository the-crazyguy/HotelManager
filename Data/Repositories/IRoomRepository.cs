using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repositories
{
    public interface IRoomRepository
    {
        IQueryable<Room> Items { get; }
        int Add(Room room);
        int Update(Room room);
        int AddOrUpdate(Room room);
        int Delete(Room room);
    }
}
