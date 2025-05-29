using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.Authentication
{
    public class LoginUserCommandHandlerTests
    {
        private readonly IUserRepository userRepositoryMock;
        private readonly LoginUserCommandHandler handler;

        public LoginUserCommandHandlerTests()
        {
            userRepositoryMock = Substitute.For<IUserRepository>();
            handler = new LoginUserCommandHandler(userRepositoryMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenLoginFails()
        {
            // Arrange
            var command = new LoginUserCommand { Email = "user@example.com", Password = "InvalidPassword" };
            userRepositoryMock.Login(Arg.Any<User>(), CancellationToken.None).Returns(Result<string>.Failure("Login failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Login failed.", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenLoginSucceeds()
        {
            // Arrange
            var command = new LoginUserCommand { Email = "user@example.com", Password = "ValidPassword123" };
            var token = "sample-token";
            userRepositoryMock.Login(Arg.Any<User>(), CancellationToken.None).Returns(Result<string>.Success(token));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(token, result.Data);
        }
    }
}


