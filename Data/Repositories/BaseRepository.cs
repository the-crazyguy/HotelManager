using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

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
        
        public int Add(T item)
        {
            _dbContext.Add(item);
            return _dbContext.SaveChanges();
        }

        public int Update(T item)
        {
            _dbContext.Update(item);
            return _dbContext.SaveChanges();
        }

        public int AddOrUpdate(T item)
        {
            if (item.Id == 0)
            {
                //The Id is 0 so we are adding a new customer
                return Add(item);
            }
            else
            {
                //The Id exists, so we update the customer
                return Update(item);
            }
        }

        public int Delete(T item)
        {
            _dbContext.Remove(item);
            return _dbContext.SaveChanges();
        }
    }
}
