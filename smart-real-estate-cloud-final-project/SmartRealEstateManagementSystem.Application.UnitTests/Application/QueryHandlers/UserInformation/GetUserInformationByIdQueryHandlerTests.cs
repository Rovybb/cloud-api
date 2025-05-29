using Application.DTOs;
using Application.QueryHandlers.User;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using UserEntities = Domain.Entities;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.QueryHandlers.User
{
    public class GetUserInformationByIdQueryHandlerTests
    {
        private readonly IUserInformationRepository userRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly GetUserInformationByIdQueryHandler handler;

        public GetUserInformationByIdQueryHandlerTests()
        {
            userRepositoryMock = Substitute.For<IUserInformationRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new GetUserInformationByIdQueryHandler(userRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenUserInformationExists()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var userInformation = EntityFactory.CreateUserInformation(mockId);
            var userDto = EntityFactory.CreateUserDto(userInformation);

            userRepositoryMock.GetByIdAsync(userInformation.Id).Returns(Result<UserEntities.UserInformation>.Success(userInformation));
            mapperMock.Map<UserDto>(userInformation).Returns(userDto);

            var query = EntityFactory.CreateGetUserInformationByIdQuery(userInformation.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userDto, result.Data);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenUserInformationDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            userRepositoryMock.GetByIdAsync(userId).Returns(Result<UserEntities.UserInformation>.Failure("User not found."));

            var query = EntityFactory.CreateGetUserInformationByIdQuery(userId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.ErrorMessage);
        }
    }
}



