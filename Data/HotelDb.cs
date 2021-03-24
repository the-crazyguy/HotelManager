using Data.Entity;
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
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //TODO: Work out if these are correct
            //Reservation between Empoyee and Reservation
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Reservations)
                .WithOne(r => r.Creator)
                .HasForeignKey(r => r.CreatorId);

            //TODO: Should this be even here? One-to-one relationship
            //Relation between Room and Reservation
            modelBuilder.Entity<Room>()
                .HasOne(room => room.Reservation)
                .WithOne(reservation => reservation.Room)
                .HasForeignKey<Reservation>(reservation => reservation.RoomId);

            //Relation between Reservation and Customer
            modelBuilder.Entity<Reservation>()
                .HasMany(reservation => reservation.Customers)
                .WithOne(customer => customer.Reservation)
                .HasForeignKey(customer => customer.ReservationId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            //TODO: Create db on your own machine and paste the connection string here
            builder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=HotelDb;Integrated Security=True;");
            builder.UseLazyLoadingProxies();
        }

    }
}
