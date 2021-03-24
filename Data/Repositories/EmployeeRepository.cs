using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private readonly HotelDb _dbContext;

        public EmployeeRepository(HotelDb dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Employee> Items => _dbContext.Employees;
    }
}
