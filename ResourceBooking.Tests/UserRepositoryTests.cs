using System.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Models;
using Xunit;

namespace ResourceBooking.Tests;

/// <summary>
/// Verifies that passwords are hashed on write and that authentication only
/// succeeds for the correct credentials.
/// </summary>
public class UserRepositoryTests : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<DataContext> _options;
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public UserRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<DataContext>().UseSqlite(_connection).Options;

        using var context = new DataContext(_options);
        context.Database.EnsureCreated();
    }

    private DataContext NewContext() => new(_options);

    private static User NewUser(string email, string password) =>
        new()
        {
            Email = email,
            Name = "Test",
            LastName = "User",
            Password = password,
        };

    [Fact]
    public async Task CreateUserAsync_StoresHashedPassword()
    {
        var repository = new UserRepository(NewContext(), _passwordHasher);

        var created = await repository.CreateUserAsync(NewUser("hash@example.com", "PlainText123"));

        Assert.NotEqual("PlainText123", created.Password);
        Assert.NotEmpty(created.Password);
    }

    [Fact]
    public async Task AuthenticateUserAsync_ReturnsUser_ForValidCredentials()
    {
        using (var context = NewContext())
        {
            var repository = new UserRepository(context, _passwordHasher);
            await repository.CreateUserAsync(NewUser("valid@example.com", "Correct123"));
        }

        using (var context = NewContext())
        {
            var repository = new UserRepository(context, _passwordHasher);
            var user = await repository.AuthenticateUserAsync("valid@example.com", "Correct123");

            Assert.NotNull(user);
            Assert.Equal("valid@example.com", user!.Email);
        }
    }

    [Fact]
    public async Task AuthenticateUserAsync_ReturnsNull_ForWrongPassword()
    {
        using (var context = NewContext())
        {
            var repository = new UserRepository(context, _passwordHasher);
            await repository.CreateUserAsync(NewUser("wrong@example.com", "Correct123"));
        }

        using (var context = NewContext())
        {
            var repository = new UserRepository(context, _passwordHasher);
            var user = await repository.AuthenticateUserAsync("wrong@example.com", "NotThePassword");

            Assert.Null(user);
        }
    }

    public void Dispose() => _connection.Dispose();
}
