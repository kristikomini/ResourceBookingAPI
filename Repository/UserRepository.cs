using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Models;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        try
        {
            return await _context
                .Users.Include(u => u.Bookings) // Include the related bookings
                .ToListAsync();
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception("Error fetching users.", ex);
        }
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        try
        {
            return await _context
                .Users.Include(u => u.Bookings) // Include the related bookings
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception("Error fetching user by ID.", ex);
        }
    }

    public async Task<User> CreateUserAsync(User user)
    {
        try
        {
            // Hash the password before saving
            user.Password = HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception("Error creating user.", ex);
        }
    }

    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && VerifyPassword(password, user.Password))
            {
                return user;
            }
            return null;
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception("Error authenticating user.", ex);
        }
    }

    public async Task UpdateUserAsync(User user)
    {
        try
        {
            // Hash the password before saving
            user.Password = HashPassword(user.Password);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception("Error updating user.", ex);
        }
    }

    public async Task DeleteUserAsync(User user)
    {
        try
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception("Error deleting user.", ex);
        }
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

    private bool VerifyPassword(string enteredPassword, string storedHash)
    {
        var enteredHash = HashPassword(enteredPassword);
        return enteredHash == storedHash;
    }
}
