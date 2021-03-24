using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repositories
{
    public interface IEmployeeRepository
    {
        IQueryable<Employee> Items { get; }
        int Add(Employee employee);
        int Update(Employee employee);
        int AddOrUpdate(Employee employee);
        int Delete(Employee employee);
    }
}
