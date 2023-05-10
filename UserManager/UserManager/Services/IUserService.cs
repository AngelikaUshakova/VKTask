using UserManager.Models;

namespace UserManager.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);

    Task<User[]> GetAllAsync(int size, int page);

    Task<User> CreateUserAsync(CreateUserDto user);

    Task<bool> DeleteUserAsync(int id);
}