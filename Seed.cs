using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResourceBooking.Data;
using ResourceBooking.Models;

namespace ResourceBooking
{
    public class Seed
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public Seed(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Seeds the database with initial data if there are no existing data
        public void SeedDataContext()
        {
            // EnableRetryOnFailure uses a retrying execution strategy, which forbids
            // user-initiated transactions unless they run inside the strategy itself.
            var strategy = _context.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!_context.Users.Any())
                    {
                        var resourceTypes = new List<ResourceType>
                        {
                            new ResourceType { TypeName = "Car" },
                            new ResourceType { TypeName = "Meeting Room" },
                        };
                        _context.ResourceTypes.AddRange(resourceTypes);
                        _context.SaveChanges();

                        var users = new List<User>
                        {
                            new User
                            {
                                Email = "john.doe@example.com",
                                Name = "John",
                                LastName = "Doe",
                                Password = HashPassword("password123"),
                            },
                            new User
                            {
                                Email = "jane.doe@example.com",
                                Name = "Jane",
                                LastName = "Doe",
                                Password = HashPassword("password123"),
                            },
                        };
                        _context.Users.AddRange(users);
                        _context.SaveChanges();

                        var resources = new List<Resource>
                        {
                            new Resource
                            {
                                Name = "Toyota Corolla",
                                ResourceTypeId = resourceTypes
                                    .Single(rt => rt.TypeName == "Car")
                                    .ResourceTypeId,
                            },
                            new Resource
                            {
                                Name = "Conference Room 101",
                                ResourceTypeId = resourceTypes
                                    .Single(rt => rt.TypeName == "Meeting Room")
                                    .ResourceTypeId,
                            },
                        };
                        _context.Resources.AddRange(resources);
                        _context.SaveChanges();

                        var bookings = new List<Booking>
                        {
                            new Booking
                            {
                                ResourceId = resources
                                    .Single(r => r.Name == "Toyota Corolla")
                                    .ResourceId,
                                UserId = users
                                    .Single(u => u.Email == "john.doe@example.com")
                                    .UserId,
                                StartDate = DateTime.Now,
                                EndDate = DateTime.Now.AddDays(1),
                            },
                            new Booking
                            {
                                ResourceId = resources
                                    .Single(r => r.Name == "Conference Room 101")
                                    .ResourceId,
                                UserId = users
                                    .Single(u => u.Email == "jane.doe@example.com")
                                    .UserId,
                                StartDate = DateTime.Now.AddDays(1),
                                EndDate = DateTime.Now.AddDays(2),
                            },
                        };
                        _context.Bookings.AddRange(bookings);
                        _context.SaveChanges();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
