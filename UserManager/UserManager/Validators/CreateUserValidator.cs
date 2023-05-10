using System.ComponentModel.DataAnnotations;
using UserManager.Models;
using UserManager.Repositories;

namespace UserManager.Validators;

public class CreateUserValidator : ICreateUserValidator
{
    private readonly IUserRepository _userRepository;
    private readonly IUserGroupRepository _userGroupRepository;

    public CreateUserValidator(IUserRepository userRepository, IUserGroupRepository userGroupRepository)
    {
        _userRepository = userRepository;
        _userGroupRepository = userGroupRepository;
    }

    public async Task<UserGroup> Validate(CreateUserDto userToCreate)
    {
        if (string.IsNullOrEmpty(userToCreate.Login))
        {
            throw new ValidationException("Логин должен быть заполнен");
        }

        if (string.IsNullOrEmpty(userToCreate.Password))
        {
            throw new ValidationException("Пароль должен быть заполнен");
        }

        if (string.IsNullOrEmpty(userToCreate.UserGroup))
        {
            throw new ValidationException("Группа должна быть заполнена");
        }

        var userGroup = await _userGroupRepository.GetByCodeAsync(userToCreate.UserGroup);
        if (userGroup == null)
        {
            throw new ValidationException($"Группы {userToCreate.UserGroup} не существует");
        }

        if (userGroup.Id == (int)PredefinedUserGroup.Admin)
        {
            if (await _userRepository.IsAdminExistAsync())
            {
                throw new ValidationException("Администратор уже существует");
            }
        }

        return userGroup;
    }
}