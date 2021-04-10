using Data.Entity;
using Data.Repositories;
using HotelManagerWebsite.Controllers;
using HotelManagerWebsite.Models.Customer;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace HotelManagerUnitTest
{
    public class Tests
    {
        private Mock<ICustomerRepository> _mockRepo;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<ICustomerRepository>();
        }

        [Test]
        public void Index_ShouldReturn_CorrectModel()
        {
            //Arrange
            var model = new CustomerIndexViewModel();

            var fakeCustomers = new List<Customer> {
            new Customer
            {
                Id = 0,
                FirstName = "Toshko",
                LastName = "Kukata",
                Email = "toshko@toshko.com",
                IsAdult = true,
                PhoneNumber = "0999994959",
                CustomerReservations = new List<CustomerReservation>()
            },
            new Customer
            {
                Id = 1,
                FirstName = "Toshko23",
                LastName = "Kukat345a",
                Email = "tos2134hko@toshko.com",
                IsAdult = true,
                PhoneNumber = "09324594959",
                CustomerReservations = new List<CustomerReservation>()
            },
            new Customer
            {
                Id = 2,
                FirstName = "Tasdasfsadfko",
                LastName = "Kertertertta",
                Email = "toshkdfggdfg@toshko.com",
                IsAdult = true,
                PhoneNumber = "092349959",
                CustomerReservations = new List<CustomerReservation>()
            }
            };
            _mockRepo.SetupGet(m => m.Items).Returns(fakeCustomers.AsQueryable());
            var controller = new CustomersController(_mockRepo.Object);

            //Act
            var result = controller.Index(model) as ViewResult;

            //Assert
            Assert.IsAssignableFrom<CustomerIndexViewModel>(result.Model);
        }
    }
}