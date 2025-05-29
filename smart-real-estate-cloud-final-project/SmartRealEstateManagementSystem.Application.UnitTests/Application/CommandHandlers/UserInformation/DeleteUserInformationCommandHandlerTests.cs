using Application.CommandHandlers.UserInformation;
using Application.Commands.User;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.UserInformation
{
    public class DeleteUserInformationCommandHandlerTests
    {
        private readonly IUserInformationRepository userRepositoryMock;
        private readonly DeleteUserInformationCommandHandler handler;

        public DeleteUserInformationCommandHandlerTests()
        {
            userRepositoryMock = Substitute.For<IUserInformationRepository>();
            handler = new DeleteUserInformationCommandHandler(userRepositoryMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenUserInformationIsDeleted()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = new DeleteUserInformationCommand { Id = mockId };

            userRepositoryMock.DeleteAsync(mockId).Returns(Result.Success());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenUserInformationDeletionFails()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = new DeleteUserInformationCommand { Id = mockId };

            userRepositoryMock.DeleteAsync(mockId).Returns(Result.Failure("User deletion failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User deletion failed.", result.ErrorMessage);
        }
    }
}


