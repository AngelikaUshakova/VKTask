using UserManager.Models;

namespace UserManager.Repositories;

public interface IUserGroupRepository
{
    Task<UserGroup?> GetByCodeAsync(string userGroupCode);
}