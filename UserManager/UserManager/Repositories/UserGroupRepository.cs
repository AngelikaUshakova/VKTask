using Microsoft.EntityFrameworkCore;
using UserManager.Models;

namespace UserManager.Repositories;

public class UserGroupRepository : IUserGroupRepository
{
    private readonly UserManagerContext _context;

    public UserGroupRepository(UserManagerContext context)
    {
        _context = context;
    }

    public async Task<UserGroup?> GetByCodeAsync(string userGroupCode)
    {
        return await _context.UserGroups.FirstOrDefaultAsync(userGroup => userGroup.Code == userGroupCode);
    }
}