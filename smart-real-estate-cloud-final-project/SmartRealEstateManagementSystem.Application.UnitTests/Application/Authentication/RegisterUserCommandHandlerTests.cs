using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.Authentication
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly IUserRepository userRepositoryMock;
        private readonly IUserInformationRepository userInfoRepositoryMock;
        private readonly RegisterUserCommandHandler handler;

        public RegisterUserCommandHandlerTests()
        {
            userRepositoryMock = Substitute.For<IUserRepository>();
            userInfoRepositoryMock = Substitute.For<IUserInformationRepository>();
            handler = new RegisterUserCommandHandler(userRepositoryMock, userInfoRepositoryMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenEmailAlreadyExists()
        {
            // Arrange
            var command = new RegisterUserCommand { Email = "user@example.com", Password = "ValidPassword123" };
            var existingUser = EntityFactory.CreateUser(Guid.NewGuid(), "user@example.com", "hashedpassword");
            userRepositoryMock.GetByEmail(command.Email, CancellationToken.None).Returns(Result<User>.Success(existingUser));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Email already exists!", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenUserRegistrationFails()
        {
            // Arrange
            var command = new RegisterUserCommand { Email = "user@example.com", Password = "ValidPassword123" };
            userRepositoryMock.GetByEmail(command.Email, CancellationToken.None).Returns(Result<User>.Failure("User not found."));
            userRepositoryMock.Register(Arg.Any<User>(), CancellationToken.None).Returns(Result<Guid>.Failure("Registration failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Registration failed.", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var command = new RegisterUserCommand { Email = "user@example.com", Password = "ValidPassword123" };
            var userId = Guid.NewGuid();
            userRepositoryMock.GetByEmail(command.Email, CancellationToken.None).Returns(Result<User>.Failure("User not found."));
            userRepositoryMock.Register(Arg.Any<User>(), CancellationToken.None).Returns(Result<Guid>.Success(userId));
            userInfoRepositoryMock.CreateAsync(Arg.Any<UserInformation>()).Returns(Result<Guid>.Success(userId));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Data);
        }
    }
}


