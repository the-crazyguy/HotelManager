using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface IRoomRepository
    {
        IQueryable<Room> Items { get; }
        Task<int> Add(Room room);
        Task<int> Update(Room room);
        Task<int> AddOrUpdate(Room room);
        Task<int> Delete(Room room);
    }
}
