using System.ComponentModel.DataAnnotations;
using UserManager.Models;
using UserManager.Repositories;
using UserManager.Validators;

namespace UserManager.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ICreateUserValidator _createUserValidator;

    public UserService(IUserRepository userRepository,
        IPasswordService passwordService,
        ICreateUserValidator createUserValidator)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _createUserValidator = createUserValidator;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userRepository.GetActiveUserByIdAsync(id);
    }

    public async Task<User[]> GetAllAsync(int pageSize, int pageNumber)
    {
        if (pageSize < 1 || pageNumber < 1)
        {
            throw new ValidationException("Размер страницы и номер страницы должны быть положительными числами");
        }

        return await _userRepository.GetAllActiveUsersAsync(pageSize, pageNumber);
    }

    public async Task<User> CreateUserAsync(CreateUserDto user)
    {
        var userGroup = await _createUserValidator.Validate(user);

        var newUser = PrepareUser(user, userGroup);
        var createdUser = await _userRepository.CreateUserAsync(newUser);

        // Fake job to increase time
        await Task.Delay(5000);

        await _userRepository.UpdateStateAsync(createdUser.Id, PredefinedUserState.Active);

        return (await GetByIdAsync(createdUser.Id))!;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        await _userRepository.UpdateStateAsync(user.Id, PredefinedUserState.Blocked);
        return true;
    }

    private User PrepareUser(CreateUserDto user, UserGroup userGroup)
    {
        _passwordService.CreatePasswordHash(user.Password, out var hash, out var salt);

        return new User
        {
            Login = user.Login,
            UserGroupId = userGroup.Id,
            UserStateId = (int)PredefinedUserState.Initializing,
            Password = hash,
            Salt = salt,
            CreatedDate = DateTime.UtcNow
        };
    }
}