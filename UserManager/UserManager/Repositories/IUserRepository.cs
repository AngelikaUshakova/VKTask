using UserManager.Models;

namespace UserManager.Repositories;

public interface IUserRepository
{
    Task<User?> GetActiveUserByIdAsync(int id);

    Task<User[]> GetAllActiveUsersAsync(int pageSize, int pageNumber);

    Task<User> CreateUserAsync(User user);

    Task<User> UpdateStateAsync(int userId, PredefinedUserState state);

    Task<bool> IsAdminExistAsync();
}