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
    public class GetAllUserInformationsQueryHandlerTests
    {
        private readonly IUserInformationRepository userRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly GetAllUserInformationsQueryHandler handler;

        public GetAllUserInformationsQueryHandlerTests()
        {
            userRepositoryMock = Substitute.For<IUserInformationRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new GetAllUserInformationsQueryHandler(userRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenUserInformationsExist()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var userInformation = EntityFactory.CreateUserInformation(mockId);
            var userInformations = new List<UserEntities.UserInformation> { userInformation };
            var userDtos = userInformations.Select(EntityFactory.CreateUserDto).ToList();

            userRepositoryMock.GetAllAsync().Returns(userInformations);
            mapperMock.Map<UserDto>(Arg.Any<UserEntities.UserInformation>()).Returns(callInfo =>
            {
                var user = callInfo.Arg<UserEntities.UserInformation>();
                return EntityFactory.CreateUserDto(user);
            });

            var query = EntityFactory.CreateGetAllUserInformationsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userDtos, result.Data.ToList(), new UserDtoComparer());
        }

        [Fact]
        public async Task Handle_ShouldReturnTrueResult_WhenNoUserInformationsExist()
        {
            // Arrange
            userRepositoryMock.GetAllAsync().Returns(Enumerable.Empty<UserEntities.UserInformation>());

            var query = EntityFactory.CreateGetAllUserInformationsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }

    public class UserDtoComparer : IEqualityComparer<UserDto>
    {
        public bool Equals(UserDto x, UserDto y)
        {
            if (x == null || y == null)
                return false;

            return x.Id == y.Id &&
                   x.Username == y.Username &&
                   x.Email == y.Email &&
                   x.FirstName == y.FirstName &&
                   x.LastName == y.LastName &&
                   x.Address == y.Address &&
                   x.PhoneNumber == y.PhoneNumber &&
                   x.Nationality == y.Nationality &&
                   x.CreatedAt == y.CreatedAt &&
                   x.LastLogin == y.LastLogin &&
                   x.Status == y.Status &&
                   x.Role == y.Role;
        }

        public int GetHashCode(UserDto obj)
        {
            var hash1 = HashCode.Combine(obj.Id, obj.Username, obj.Email, obj.FirstName, obj.LastName, obj.Address, obj.PhoneNumber);
            var hash2 = HashCode.Combine(obj.Nationality, obj.CreatedAt, obj.LastLogin, obj.Status, obj.Role);
            return HashCode.Combine(hash1, hash2);
        }
    }
}



