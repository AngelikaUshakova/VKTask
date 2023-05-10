using Microsoft.EntityFrameworkCore;
using UserManager.Models;

namespace UserManager.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManagerContext _context;

    public UserRepository(UserManagerContext context)
    {
        _context = context;
    }

    public async Task<User?> GetActiveUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(user => user.UserGroup)
            .Include(user => user.UserState)
            .FirstOrDefaultAsync(user => user.Id == id && user.UserStateId == (int)PredefinedUserState.Active);
    }

    public async Task<User[]> GetAllActiveUsersAsync(int pageSize, int pageNumber)
    {
        return await _context.Users
            .Include(user => user.UserGroup)
            .Include(user => user.UserState)
            .Where(user => user.UserStateId == (int)PredefinedUserState.Active)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        try
        {
            var createdUser = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return createdUser.Entity;
        }
        catch (DbUpdateException e) when (e.InnerException?.Message?.Contains("duplicate key") ?? false)
        {
            throw new Exception($"Логин {user.Login} уже используется");
        }
    }

    public async Task<User> UpdateStateAsync(int userId, PredefinedUserState state)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (user == null)
        {
            throw new Exception($"Ошибка при обновлении статуса пользователя {userId}");
        }

        user.UserStateId = (int)state;
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> IsAdminExistAsync()
    {
        return await _context.Users.AnyAsync(user => user.UserGroupId == (int)PredefinedUserGroup.Admin);
    }
}