using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface ICustomerRepository
    {
        IQueryable<Customer> Items { get; }
        Task<int> Add(Customer customer);
        Task<int> Update(Customer customer);
        Task<int> AddOrUpdate(Customer customer);
        Task<int> Delete(Customer customer);
    }
}
