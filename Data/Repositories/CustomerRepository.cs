using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository 
    {
        private readonly HotelDb _dbContext;

        public CustomerRepository(HotelDb dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Customer> Items => _dbContext.Customers;
    }
}
