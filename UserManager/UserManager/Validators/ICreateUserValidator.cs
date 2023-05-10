using UserManager.Models;

namespace UserManager.Validators;

public interface ICreateUserValidator
{
    Task<UserGroup> Validate(CreateUserDto userToCreate);
}