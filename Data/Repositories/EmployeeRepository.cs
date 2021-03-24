using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly HotelDb _dbContext;

        public EmployeeRepository(HotelDb dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Employee> Items => _dbContext.Employees;

        public int Add(Employee employee)
        {
            _dbContext.Add(employee);
            return _dbContext.SaveChanges();
        }

        public int Update(Employee employee)
        {
            _dbContext.Update(employee);
            return _dbContext.SaveChanges();
        }

        public int AddOrUpdate(Employee employee)
        {
            if (employee.Id == 0)
            {
                //The Id is 0 so we are adding a new employee
                return Add(employee);
            }
            else
            {
                //The Id exists, so we update the employee
                return Update(employee);
            }
        }

        public int Delete(Employee employee)
        {
            _dbContext.Remove(employee);
            return _dbContext.SaveChanges();
        }
    }
}
