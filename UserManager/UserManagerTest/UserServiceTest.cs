using System.ComponentModel.DataAnnotations;
using Moq;
using UserManager.Models;
using UserManager.Repositories;
using UserManager.Services;
using UserManager.Validators;

namespace UserManagerTest
{
    public class UserServiceTests
    {
        private readonly Mock<IPasswordService> _passwordServiceMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<ICreateUserValidator> _createUserValidatorMock = new();

        private readonly UserService _service;

        public UserServiceTests()
        {
            _service = new UserService(_userRepositoryMock.Object,
                _passwordServiceMock.Object,
                _createUserValidatorMock.Object);
        }

        [Fact]
        public async Task CreateUser_ValidationError_ValidationException()
        {
            _createUserValidatorMock
                .Setup(validator => validator.Validate(It.IsAny<CreateUserDto>()))
                .Throws<ValidationException>();

            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateUserAsync(new CreateUserDto()));
        }


        [Fact]
        public async Task GetByIdAsync_CorrectValues_GetFromUserRepository()
        {
            const int userId = 12;

            await _service.GetByIdAsync(userId);

            _userRepositoryMock.Verify(repository => repository.GetActiveUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_CorrectValues_GetFromUserRepository()
        {
            const int pageSize = 12;
            const int pageNumber = 8;

            await _service.GetAllAsync(pageSize, pageNumber);

            _userRepositoryMock.Verify(repository => repository.GetAllActiveUsersAsync(pageSize, pageNumber), Times.Once);
        }

        [Theory]
        [InlineData(0, 8)]
        [InlineData(-8, 8)]
        [InlineData(12, 0)]
        [InlineData(12, -12)]
        public async Task GetAllAsync_NonPositivePageSizeOrPageNumber_ValidationException(int pageSize, int pageNumber)
        {
            await Assert.ThrowsAsync<ValidationException>(() => _service.GetAllAsync(pageSize, pageNumber));
        }

        [Fact]
        public async Task DeleteUserAsync_CorrectValues_DeleteFromUserRepository()
        {
            const int userId = 12;

            _userRepositoryMock
                .Setup(repository => repository.GetActiveUserByIdAsync(userId))
                .ReturnsAsync(new User() { Id = userId });

            var success = await _service.DeleteUserAsync(userId);

            Assert.True(success);
            _userRepositoryMock
                .Verify(repository => repository.UpdateStateAsync(userId, PredefinedUserState.Blocked), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_InvalidUserId_DeleteFromUserRepository()
        {
            const int userId = 12;

            _userRepositoryMock
                .Setup(repository => repository.GetActiveUserByIdAsync(userId))
                .ReturnsAsync(() => null);

            var success = await _service.DeleteUserAsync(userId);

            Assert.False(success);
        }

        [Fact]
        public async Task CreateUserAsync_CorrectValues_UserCreated()
        {
            var parameters = new CreateUserDto
            {
                Login = "Login",
                Password = "Password",
                UserGroup = "UserGroup"
            };

            var hash = new byte[16];
            var salt = new byte[8];

            _passwordServiceMock
                .Setup(service => service.CreatePasswordHash(parameters.Password, out hash, out salt));

            var userGroup = new UserGroup { Id = 42 };

            _createUserValidatorMock
                .Setup(validator => validator.Validate(parameters))
                .ReturnsAsync(userGroup);

            _userRepositoryMock
                .Setup(repository => repository.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(new User());

            await _service.CreateUserAsync(parameters);

            _userRepositoryMock.Verify(repository =>
                repository.CreateUserAsync(It.Is<User>(user =>
                    user.Login == parameters.Login &&
                    user.Password == hash &&
                    user.Salt == salt &&
                    user.UserGroupId == userGroup.Id &&
                    user.UserStateId == (int)PredefinedUserState.Initializing)), Times.Once);

            _userRepositoryMock.Verify(repository =>
                repository.UpdateStateAsync(It.IsAny<int>(), PredefinedUserState.Active), Times.Once);
        }
    }
}