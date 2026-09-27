using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Models;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserRepository(DataContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context
            .Users.Include(u => u.Bookings) // Include the related bookings
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context
            .Users.Include(u => u.Bookings) // Include the related bookings
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        // Hash the (plain-text) password before saving.
        user.Password = _passwordHasher.HashPassword(user, user.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email.ToLower() == email.ToLower()
        );

        if (user is null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        // Transparently upgrade the stored hash if the hashing parameters have changed.
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.Password = _passwordHasher.HashPassword(user, password);
            await _context.SaveChangesAsync();
        }

        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        // Re-hash the incoming (plain-text) password before saving.
        user.Password = _passwordHasher.HashPassword(user, user.Password);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
