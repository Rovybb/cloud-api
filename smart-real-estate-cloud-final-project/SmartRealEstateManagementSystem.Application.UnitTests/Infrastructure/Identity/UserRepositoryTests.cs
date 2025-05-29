using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using Identity.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Infrastructure.Identity
{
    public class UserRepositoryTests
    {
        private readonly UsersDbContext dbContextMock;
        private readonly IConfiguration configurationMock;
        private readonly UserRepository userRepository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            dbContextMock = new UsersDbContext(options);
            configurationMock = Substitute.For<IConfiguration>();
            userRepository = new UserRepository(dbContextMock, configurationMock);
        }

        [Fact]
        public async Task Login_ShouldReturnFailureResult_WhenUserNotFound()
        {
            // Arrange
            var user = new User { Email = "user@example.com", PasswordHash = "password" };

            // Act
            var result = await userRepository.Login(user, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.ErrorMessage);
        }

        [Fact]
        public async Task Login_ShouldReturnFailureResult_WhenPasswordIsInvalid()
        {
            // Arrange
            var user = new User { Email = "user@example.com", PasswordHash = "invalidpassword" };
            var existingUser = new User { Id = Guid.NewGuid(), Email = "user@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("validpassword") };

            // Clear the database to ensure no duplicate users
            dbContextMock.Users.RemoveRange(dbContextMock.Users);
            await dbContextMock.SaveChangesAsync();

            await dbContextMock.Users.AddAsync(existingUser);
            await dbContextMock.SaveChangesAsync();

            // Act
            var result = await userRepository.Login(user, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid password.", result.ErrorMessage);
        }

        [Fact]
        public async Task Login_ShouldReturnSuccessResult_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new User { Email = "user@example.com", PasswordHash = "validpassword" };
            var existingUser = new User { Id = Guid.NewGuid(), Email = "user@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("validpassword") };

            // Clear the database to ensure no duplicate users
            dbContextMock.Users.RemoveRange(dbContextMock.Users);
            await dbContextMock.SaveChangesAsync();

            await dbContextMock.Users.AddAsync(existingUser);
            await dbContextMock.SaveChangesAsync();

            configurationMock["Jwt:Key"].Returns("supersecretkey12345678901234567890"); // Ensure the key is at least 32 characters (256 bits)

            // Act
            var result = await userRepository.Login(user, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Register_ShouldReturnSuccessResult_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "user@example.com", PasswordHash = "hashedpassword" };

            // Act
            var result = await userRepository.Register(user, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(user.Id, result.Data);
        }

        [Fact]
        public async Task Register_ShouldReturnFailureResult_WhenExceptionIsThrown()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "user@example.com", PasswordHash = "hashedpassword" };
            dbContextMock.Users.Add(user);
            await dbContextMock.SaveChangesAsync();

            // Act
            var result = await userRepository.Register(user, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrorMessage);
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnFailureResult_WhenUserNotFound()
        {
            // Arrange
            var email = "user@example.com";

            // Act
            var result = await userRepository.GetByEmail(email, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnSuccessResult_WhenUserIsFound()
        {
            // Arrange
            var email = "user@example.com";
            var user = new User { Id = Guid.NewGuid(), Email = email, PasswordHash = "hashedpassword" };

            // Clear the database to ensure no duplicate users
            dbContextMock.Users.RemoveRange(dbContextMock.Users);
            await dbContextMock.SaveChangesAsync();

            await dbContextMock.Users.AddAsync(user);
            await dbContextMock.SaveChangesAsync();

            // Act
            var result = await userRepository.GetByEmail(email, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(user, result.Data);
        }
    }
}


