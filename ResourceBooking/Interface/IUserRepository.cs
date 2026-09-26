using ResourceBooking.Models;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<User> CreateUserAsync(User user);
    Task<User?> AuthenticateUserAsync(string email, string password);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
}
