using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    //A generic class for CRUD operations
    public abstract class BaseRepository<T> where T : BaseEntity
    {
        private readonly HotelDb _dbContext;

        public BaseRepository(HotelDb dbContext)
        {
            _dbContext = dbContext;
        }
        
        public virtual async Task<int> Add(T item)
        {
            _dbContext.Add(item);
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> Update(T item)
        {
            _dbContext.Update(item);
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> AddOrUpdate(T item)
        {
            if (item.Id == 0)
            {
                //The Id is 0 so we are adding a new item
                return await Add(item);
            }
            else
            {
                //The Id exists, so we update the item
                return await Update(item);
            }
        }

        public async Task<int> Delete(T item)
        {
            _dbContext.Remove(item);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
