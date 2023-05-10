using System.ComponentModel.DataAnnotations;
using Moq;
using UserManager.Models;
using UserManager.Repositories;
using UserManager.Validators;

namespace UserManagerTest;

public class CreateUserValidatorTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserGroupRepository> _userGroupRepositoryMock = new();

    private readonly CreateUserValidator _validator;

    public CreateUserValidatorTests()
    {
        _validator = new CreateUserValidator(_userRepositoryMock.Object, _userGroupRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateUser_LoginIsMissing_ValidationException()
    {
        var createUserParams = new CreateUserDto
        {
            Password = "Password",
            UserGroup = "UserGroup"
        };

        await Assert.ThrowsAsync<ValidationException>(() => _validator.Validate(createUserParams));
    }

    [Fact]
    public async Task CreateUser_PasswordIsMissing_ValidationException()
    {
        var createUserParams = new CreateUserDto
        {
            Login = "Login",
            UserGroup = "UserGroup"
        };

        await Assert.ThrowsAsync<ValidationException>(() => _validator.Validate(createUserParams));
    }

    [Fact]
    public async Task CreateUser_UserGroupIsMissing_ValidationException()
    {
        var createUserParams = new CreateUserDto
        {
            Login = "Login",
            Password = "Password"
        };

        await Assert.ThrowsAsync<ValidationException>(() => _validator.Validate(createUserParams));
    }

    [Fact]
    public async Task CreateUser_UserGroupDoesNotExist_ValidationException()
    {
        var createUserParams = new CreateUserDto
        {
            Login = "Login",
            Password = "Password",
            UserGroup = "UserGroup"
        };

        _userGroupRepositoryMock
            .Setup(repository => repository.GetByCodeAsync(createUserParams.UserGroup))
            .ReturnsAsync(() => null);

        await Assert.ThrowsAsync<ValidationException>(() => _validator.Validate(createUserParams));
    }

    [Fact]
    public async Task CreateUser_ThereIsAdmin_ValidationException()
    {
        var createUserParams = new CreateUserDto
        {
            Login = "Login",
            Password = "Password",
            UserGroup = "UserGroup"
        };

        _userGroupRepositoryMock
            .Setup(repository => repository.GetByCodeAsync(createUserParams.UserGroup))
            .ReturnsAsync(() => null);

        _userRepositoryMock
            .Setup(repository => repository.IsAdminExistAsync())
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ValidationException>(() => _validator.Validate(createUserParams));
    }

    [Fact]
    public async Task CreateUser_RegularUser_ReturnUserGroup()
    {
        var createUserParams = new CreateUserDto
        {
            Login = "Login",
            Password = "Password",
            UserGroup = "UserGroup"
        };

        var userGroup = new UserGroup
        {
            Id = (int)PredefinedUserGroup.User
        };

        _userGroupRepositoryMock
            .Setup(repository => repository.GetByCodeAsync(createUserParams.UserGroup))
            .ReturnsAsync(userGroup);

        var actualUserGroup = await _validator.Validate(createUserParams);

        Assert.Equal(userGroup, actualUserGroup);
        _userGroupRepositoryMock.Verify(repository => repository.GetByCodeAsync(createUserParams.UserGroup), Times.Once);
    }

    [Fact]
    public async Task CreateUser_Admin_ReturnUserGroup()
    {
        var createUserParams = new CreateUserDto
        {
            Login = "Login",
            Password = "Password",
            UserGroup = "UserGroup"
        };

        var userGroup = new UserGroup
        {
            Id = (int)PredefinedUserGroup.Admin
        };

        _userGroupRepositoryMock
            .Setup(repository => repository.GetByCodeAsync(createUserParams.UserGroup))
            .ReturnsAsync(userGroup);

        _userRepositoryMock
            .Setup(repository => repository.IsAdminExistAsync())
            .ReturnsAsync(false);

        var actualUserGroup = await _validator.Validate(createUserParams);

        Assert.Equal(userGroup, actualUserGroup);
        _userRepositoryMock.Verify(repository => repository.IsAdminExistAsync(), Times.Once);
        _userGroupRepositoryMock.Verify(repository => repository.GetByCodeAsync(createUserParams.UserGroup), Times.Once);
    }
}