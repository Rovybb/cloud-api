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
    public class CreateUserInformationCommandHandlerTests
    {
        private readonly IUserInformationRepository userRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly CreateUserInformationCommandHandler handler;

        public CreateUserInformationCommandHandlerTests()
        {
            userRepositoryMock = Substitute.For<IUserInformationRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new CreateUserInformationCommandHandler(userRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenUserInformationIsCreated()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateUserInformationCommand(mockId);
            var userInformation = EntityFactory.CreateUserInformation(mockId);

            mapperMock.Map<Domain.Entities.UserInformation>(command).Returns(userInformation);
            userRepositoryMock.CreateAsync(userInformation).Returns(Result<Guid>.Success(mockId));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(mockId, result.Data);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenUserInformationCreationFails()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateUserInformationCommand(mockId);
            var userInformation = EntityFactory.CreateUserInformation(mockId);

            mapperMock.Map<Domain.Entities.UserInformation>(command).Returns(userInformation);
            userRepositoryMock.CreateAsync(userInformation).Returns(Result<Guid>.Failure("User creation failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User creation failed.", result.ErrorMessage);
        }
    }
}


