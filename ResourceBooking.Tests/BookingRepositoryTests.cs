using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Exceptions;
using ResourceBooking.Models;
using ResourceBooking.Repositories;
using Xunit;

namespace ResourceBooking.Tests;

/// <summary>
/// Exercises the booking overlap rules against a real relational database
/// (SQLite in-memory) so the transaction/overlap logic is covered end to end.
/// </summary>
public class BookingRepositoryTests : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<DataContext> _options;

    public BookingRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<DataContext>().UseSqlite(_connection).Options;

        using var context = new DataContext(_options);
        context.Database.EnsureCreated();

        var type = new ResourceType { TypeName = "Room" };
        context.ResourceTypes.Add(type);
        context.SaveChanges();

        context.Resources.Add(
            new Resource
            {
                ResourceId = 1,
                Name = "Room A",
                ResourceTypeId = type.ResourceTypeId,
            }
        );
        context.Users.Add(
            new User
            {
                UserId = 1,
                Email = "tester@example.com",
                Name = "Test",
                LastName = "User",
                Password = "hashed",
            }
        );
        context.SaveChanges();
    }

    private DataContext NewContext() => new(_options);

    private static Booking Booking(int startDay, int endDay) =>
        new()
        {
            ResourceId = 1,
            UserId = 1,
            StartDate = new DateTime(2030, 1, startDay),
            EndDate = new DateTime(2030, 1, endDay),
        };

    [Fact]
    public async Task CreateBookingAsync_PersistsNonOverlappingBooking()
    {
        var repository = new BookingRepository(NewContext());

        var created = await repository.CreateBookingAsync(Booking(1, 2));

        Assert.True(created.BookingId > 0);
    }

    [Fact]
    public async Task CreateBookingAsync_ThrowsWhenBookingsOverlap()
    {
        var repository = new BookingRepository(NewContext());
        await repository.CreateBookingAsync(Booking(1, 5));

        await Assert.ThrowsAsync<ResourceAlreadyBookedException>(
            () => repository.CreateBookingAsync(Booking(4, 6))
        );
    }

    [Fact]
    public async Task CreateBookingAsync_AllowsAdjacentBooking()
    {
        var repository = new BookingRepository(NewContext());
        await repository.CreateBookingAsync(Booking(1, 2));

        // A booking that starts exactly when the previous one ends does not overlap.
        var created = await repository.CreateBookingAsync(Booking(2, 3));

        Assert.True(created.BookingId > 0);
    }

    [Fact]
    public async Task GetAvailableResourcesAsync_ExcludesBookedResource()
    {
        var repository = new BookingRepository(NewContext());
        await repository.CreateBookingAsync(Booking(10, 12));

        var result = await repository.GetAvailableResourcesAsync(
            new DateTime(2030, 1, 10),
            new DateTime(2030, 1, 12),
            resourceId: null,
            page: 1,
            pageSize: 10
        );

        Assert.Empty(result.Results);
        Assert.Equal(0, result.TotalResults);
    }

    public void Dispose() => _connection.Dispose();
}
