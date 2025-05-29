using Application.CommandHandlers.UserInformation;
using Application.Commands.User;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.UserInformation
{
    public class UpdateUserInformationCommandHandlerTests
    {
        private readonly IUserInformationRepository userRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly UpdateUserInformationCommandHandler handler;

        public UpdateUserInformationCommandHandlerTests()
        {
            userRepositoryMock = Substitute.For<IUserInformationRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new UpdateUserInformationCommandHandler(userRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenUserInformationIsUpdated()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateUpdateUserInformationCommand(mockId);
            var userInformation = EntityFactory.CreateUserInformation(mockId);

            userRepositoryMock.GetByIdAsync(mockId).Returns(Task.FromResult(Result<Domain.Entities.UserInformation>.Success(userInformation)));
            userRepositoryMock.UpdateAsync(userInformation).Returns(Task.FromResult(Result<Guid>.Success(mockId)));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenUserInformationNotFound()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateUpdateUserInformationCommand(mockId);

            userRepositoryMock.GetByIdAsync(mockId).Returns(Task.FromResult(Result<Domain.Entities.UserInformation>.Failure("User not found.")));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenUserInformationUpdateFails()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateUpdateUserInformationCommand(mockId);
            var userInformation = EntityFactory.CreateUserInformation(mockId);

            userRepositoryMock.GetByIdAsync(mockId).Returns(Task.FromResult(Result<Domain.Entities.UserInformation>.Success(userInformation)));
            userRepositoryMock.UpdateAsync(userInformation).Returns(Task.FromResult(Result<Guid>.Failure("User update failed.")));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User update failed.", result.ErrorMessage);
        }
    }
}


