using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repositories
{
    public interface ICustomerRepository
    {
        IQueryable<Customer> Items { get; }
        int Add(Customer customer);
        int Update(Customer customer);
        int AddOrUpdate(Customer customer);
        int Delete(Customer customer);
    }
}
