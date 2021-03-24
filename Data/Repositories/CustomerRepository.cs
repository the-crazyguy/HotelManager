using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly HotelDb _dbContext;

        public CustomerRepository(HotelDb dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Customer> Items => _dbContext.Customers;

        public int Add(Customer customer)
        {
            _dbContext.Add(customer);
            return _dbContext.SaveChanges();
        }

        public int Update(Customer customer)
        {
            _dbContext.Update(customer);
            return _dbContext.SaveChanges();
        }

        public int AddOrUpdate(Customer customer)
        {
            if (customer.Id == 0)
            {
                //The Id is 0 so we are adding a new customer
                return Add(customer);
            }
            else
            {
                //The Id exists, so we update the customer
                return Update(customer);
            }
        }

        public int Delete(Customer customer)
        {
            _dbContext.Remove(customer);
            return _dbContext.SaveChanges();
        }
    }
}
