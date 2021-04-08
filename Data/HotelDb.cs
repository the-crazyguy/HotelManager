using Data.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class HotelDb : IdentityDbContext<EmployeeUser>
    {
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<EmployeeUser> EmployeeUsers { get; set; }
        public virtual DbSet<CustomerReservation> CustomerReservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Reservation between EmpoyeeUser and Reservation
            modelBuilder.Entity<EmployeeUser>()
                .HasMany(eu => eu.Reservations)
                .WithOne(r => r.Creator)
                .HasForeignKey(r => r.CreatorId);

            //Relation between Room and Reservation
            modelBuilder.Entity<Room>()
                .HasOne(room => room.Reservation)
                .WithOne(reservation => reservation.Room)
                .HasForeignKey<Reservation>(reservation => reservation.RoomId);

            //Relation between Reservation and Customer
            modelBuilder.Entity<CustomerReservation>()
                .HasKey(cr => new { cr.CustomerId, cr.ReservationId });

            modelBuilder.Entity<CustomerReservation>()
                .HasOne(cr => cr.Customer)
                .WithMany(c => c.CustomerReservations)
                .HasForeignKey(cr => cr.CustomerId);

            modelBuilder.Entity<CustomerReservation>()
                .HasOne(cr => cr.Reservation)
                .WithMany(r => r.CustomerReservations)
                .HasForeignKey(cr => cr.ReservationId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            //Create db on your own machine and paste the connection string here if needs be
            builder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=HotelDb;Integrated Security=True;MultipleActiveResultSets=true");
            builder.UseLazyLoadingProxies();
        }

    }
}
