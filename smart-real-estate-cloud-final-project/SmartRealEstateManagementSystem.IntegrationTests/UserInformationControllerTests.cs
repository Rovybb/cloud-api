using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;
using Domain.Entities;
using Domain.Types.UserInformation;
using Application.Commands.User;
using Application.Contracts.UserInformation;
using Application.DTOs;
using Microsoft.AspNetCore.Hosting;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class UserInformationControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _dbName;

        private const string BaseUrl = "/api/v1/userinformation";

        // Predefined GUIDs for testing
        private static readonly Guid ExistingUserId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        private static readonly Guid NewUserId = Guid.NewGuid(); // For Create tests

        public UserInformationControllerIntegrationTests()
        {
            // Generate a unique in-memory database name per test instance for isolation
            _dbName = Guid.NewGuid().ToString();

            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    // Remove existing DbContext registrations
                    var appDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (appDescriptor != null)
                        services.Remove(appDescriptor);

                    var identityDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<UsersDbContext>));
                    if (identityDescriptor != null)
                        services.Remove(identityDescriptor);

                    // Add in-memory databases with unique names
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(_dbName));

                    services.AddDbContext<UsersDbContext>(options =>
                        options.UseInMemoryDatabase($"{_dbName}_Identity"));
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GivenValidUserData_WhenCreateIsCalled_ThenUserIsSavedToDatabase()
        {
            // Arrange: No pre-seeded data needed for create

            var command = new CreateUserInformationCommand
            {
                Email = "newuser@example.com",
                Username = "NewUser",
                FirstName = "New",
                LastName = "User",
                Address = "123 New St",
                PhoneNumber = "111-222-3333",
                Nationality = "Newland",
                Status = UserStatus.ACTIVE,
                Role = UserRole.CLIENT,
                Company = "NewCo",
                Type = "Regular"
            };

            // Act
            var response = await _client.PostAsJsonAsync(BaseUrl, command);
            response.EnsureSuccessStatusCode();

            // Extract the created User ID from the response
            var createdUserId = await response.Content.ReadFromJsonAsync<Guid>();

            // Assert: Retrieve the user from the database
            using var scope = _factory.Services.CreateScope();
            var dbAssert = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userInDb = await dbAssert.Users.FirstOrDefaultAsync(u => u.Id == createdUserId);
            userInDb.Should().NotBeNull();
            userInDb!.Email.Should().Be(command.Email);
            userInDb.Username.Should().Be(command.Username);
            userInDb.FirstName.Should().Be(command.FirstName);
            userInDb.LastName.Should().Be(command.LastName);
            userInDb.Address.Should().Be(command.Address);
            userInDb.PhoneNumber.Should().Be(command.PhoneNumber);
            userInDb.Nationality.Should().Be(command.Nationality);
            userInDb.Status.Should().Be(command.Status);
            userInDb.Role.Should().Be(command.Role);
            userInDb.Company.Should().Be(command.Company);
            userInDb.Type.Should().Be(command.Type);
            userInDb.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task GivenExistingUser_WhenGetByIdIsCalled_ThenReturnsUser()
        {
            // Arrange: Seed an existing user
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SeedExistingUser(db);
            }

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{ExistingUserId}");
            response.EnsureSuccessStatusCode();

            // Assert
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            user.Should().NotBeNull();
            user!.Id.Should().Be(ExistingUserId);
            user.Email.Should().Be("existinguser@example.com");
            user.Username.Should().Be("ExistingUser");
            user.FirstName.Should().Be("Existing");
            user.LastName.Should().Be("User");
            user.Address.Should().Be("456 Existing Ave");
            user.PhoneNumber.Should().Be("444-555-6666");
            user.Nationality.Should().Be("ExistingLand");
            user.Status.Should().Be(UserStatus.ACTIVE);
            user.Role.Should().Be(UserRole.PROFESSIONAL);
            user.LastLogin.Should().BeNull();
        }

        [Fact]
        public async Task GivenExistingUser_WhenGetAllIsCalled_ThenReturnsUsers()
        {
            // Arrange: Seed existing users
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SeedExistingUser(db);
                SeedAnotherExistingUser(db);
            }

            // Act
            var response = await _client.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();

            // Assert
            var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
            users.Should().NotBeNull();
            users.Should().HaveCount(2);
            users.Should().ContainSingle(u => u.Id == ExistingUserId);
            users.Should().ContainSingle(u => u.Email == "anotheruser@example.com");
        }

        [Fact]
        public async Task GivenExistingUser_WhenUpdateIsCalled_ThenUserIsUpdatedInDatabase()
        {
            // Arrange: Seed an existing user
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SeedExistingUser(db);
            }

            var request = new UpdateUserInformationRequest
            {
                Username = "UpdatedUser",
                Email = "updateduser@example.com",
                FirstName = "Updated",
                LastName = "User",
                Address = "789 Updated Blvd",
                PhoneNumber = "777-888-9999",
                Nationality = "UpdatedLand",
                LastLogin = DateTime.UtcNow,
                Status = UserStatus.INACTIVE,
                Role = UserRole.PROFESSIONAL,
                Company = "UpdatedCo",
                Type = "Premium"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{ExistingUserId}", request);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Assert: Retrieve the updated user from the database
            using var assertScope = _factory.Services.CreateScope();
            var dbAssert = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var updatedUser = await dbAssert.Users.FindAsync(ExistingUserId);
            updatedUser.Should().NotBeNull();
            updatedUser!.Username.Should().Be(request.Username);
            updatedUser.Email.Should().Be(request.Email);
            updatedUser.FirstName.Should().Be(request.FirstName);
            updatedUser.LastName.Should().Be(request.LastName);
            updatedUser.Address.Should().Be(request.Address);
            updatedUser.PhoneNumber.Should().Be(request.PhoneNumber);
            updatedUser.Nationality.Should().Be(request.Nationality);
            updatedUser.LastLogin.Should().BeCloseTo(request.LastLogin.Value, precision: TimeSpan.FromSeconds(5));
            updatedUser.Status.Should().Be(request.Status);
            updatedUser.Role.Should().Be(request.Role);
            updatedUser.Company.Should().Be(request.Company);
            updatedUser.Type.Should().Be(request.Type);
        }

        [Fact]
        public async Task GivenExistingUser_WhenDeleteIsCalled_ThenRemovesUserFromDatabase()
        {
            // Arrange: Seed an existing user
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SeedExistingUser(db);
            }

            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/{ExistingUserId}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Assert: Attempt to retrieve the deleted user
            using var assertScope = _factory.Services.CreateScope();
            var dbAssert = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var deletedUser = await dbAssert.Users.FindAsync(ExistingUserId);
            deletedUser.Should().BeNull();
        }

        [Fact]
        public async Task GivenNonExistingUser_WhenGetByIdIsCalled_ThenReturnsNotFound()
        {
            // Arrange: No seeding needed for non-existing user

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenInvalidUserData_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            // Arrange: Missing required fields
            var invalidCommand = new CreateUserInformationCommand
            {
                // Intentionally leaving out required fields like Username, Email, etc.
                // This depends on your validator implementation
                Email = "",
                Username = "",
                FirstName = "",
                LastName = "",
                Address = "",
                PhoneNumber = "",
                Nationality = "",
                Status = UserStatus.ACTIVE,
                Role = UserRole.CLIENT
                // Company and Type are optional
            };

            // Act
            var response = await _client.PostAsJsonAsync(BaseUrl, invalidCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errors = await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
            errors.Should().NotBeNull();
            errors.Should().Contain(e => e.Contains("Email"));
            errors.Should().Contain(e => e.Contains("Username"));
            // Add more assertions based on your validators
        }

        public void Dispose()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var dbIdentity = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            db.Database.EnsureDeleted();
            dbIdentity.Database.EnsureDeleted();
            _client.Dispose();
        }

        // Helper Methods for Seeding Data

        private void SeedExistingUser(ApplicationDbContext db)
        {
            if (!db.Users.Any(u => u.Id == ExistingUserId))
            {
                var existingUser = new UserInformation
                {
                    Id = ExistingUserId,
                    Username = "ExistingUser",
                    Email = "existinguser@example.com",
                    FirstName = "Existing",
                    LastName = "User",
                    Address = "456 Existing Ave",
                    PhoneNumber = "444-555-6666",
                    Nationality = "ExistingLand",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    LastLogin = null,
                    Status = UserStatus.ACTIVE,
                    Role = UserRole.PROFESSIONAL,
                    Company = "ExistingCo",
                    Type = "Premium"
                };
                db.Users.Add(existingUser);
                db.SaveChanges();
            }
        }

        private void SeedAnotherExistingUser(ApplicationDbContext db)
        {
            var anotherUserId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
            if (!db.Users.Any(u => u.Id == anotherUserId))
            {
                var anotherUser = new UserInformation
                {
                    Id = anotherUserId,
                    Username = "AnotherUser",
                    Email = "anotheruser@example.com",
                    FirstName = "Another",
                    LastName = "User",
                    Address = "789 Another St",
                    PhoneNumber = "777-888-9999",
                    Nationality = "AnotherLand",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    LastLogin = DateTime.UtcNow.AddDays(-1),
                    Status = UserStatus.ACTIVE,
                    Role = UserRole.CLIENT,
                    Company = "AnotherCo",
                    Type = "Standard"
                };
                db.Users.Add(anotherUser);
                db.SaveChanges();
            }
        }
    }
}
