using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class HotelDb : IdentityDbContext<IdentityUser>
    {
        //TODO: Add DbSets

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //TODO: Add relations
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            //TODO: Create db on your own machine and paste the connection string here
            builder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=HotelDb;Integrated Security=True;");
            builder.UseLazyLoadingProxies();
        }

    }
}
