using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Domain.Entities;
using Domain.Types.Property;
using Domain.Types.UserInformation;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Application.Commands.Property;
using Application.Contracts.Property;
using Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Domain.Utils;
using Application.QueryReponses.Property;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class PropertiesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _dbName;

        private const string BaseUrl = "/api/v1/properties";

        // Predefined GUIDs for testing
        private static readonly Guid UserId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        private static readonly Guid ExistingPropertyId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        private static readonly Guid AnotherPropertyId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        private static readonly Guid ExistingImageId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        public PropertiesControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Generate a unique in-memory database name per test instance for isolation
            _dbName = Guid.NewGuid().ToString();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    // Remove existing DbContext registrations
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

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

        #region CRUD Tests

        [Fact]
        public async Task GivenValidPropertyData_WhenCreateIsCalled_ThenPropertyIsSavedToDatabase()
        {
            // Arrange: Seed user
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedUser(db);

            var command = new CreatePropertyCommand
            {
                Title = "Beautiful House",
                Description = "A beautiful house with a spacious garden.",
                Address = "123 Dream Lane",
                Price = 250000m,
                Area = 200.5m,
                Rooms = 4,
                Bathrooms = 3,
                ConstructionYear = 2015,
                UserId = UserId
            };

            // Act
            var response = await _client.PostAsJsonAsync(BaseUrl, command);
            response.EnsureSuccessStatusCode();

            // Extract the created Property ID from the response
            var createdPropertyId = await response.Content.ReadFromJsonAsync<Guid>();

            // Assert: Retrieve the property from the database
            using var assertScope = _factory.Services.CreateScope();
            var dbAssert = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var propertyInDb = await dbAssert.Properties
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == createdPropertyId);
            propertyInDb.Should().NotBeNull();
            propertyInDb!.Title.Should().Be(command.Title);
            propertyInDb.Description.Should().Be(command.Description);
            propertyInDb.Address.Should().Be(command.Address);
            propertyInDb.Price.Should().Be(command.Price);
            propertyInDb.Area.Should().Be(command.Area);
            propertyInDb.Rooms.Should().Be(command.Rooms);
            propertyInDb.Bathrooms.Should().Be(command.Bathrooms);
            propertyInDb.ConstructionYear.Should().Be(command.ConstructionYear);
            propertyInDb.UserId.Should().Be(command.UserId);
            propertyInDb.User.Should().NotBeNull();
            propertyInDb.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
            propertyInDb.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task GivenExistingProperty_WhenGetByIdIsCalled_ThenReturnsProperty()
        {
            // Arrange: Seed user and existing property
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedUser(db);
            SeedExistingProperty(db);

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{ExistingPropertyId}");
            response.EnsureSuccessStatusCode();

            // Assert
            var property = await response.Content.ReadFromJsonAsync<PropertyDto>();
            property.Should().NotBeNull();
            property!.Id.Should().Be(ExistingPropertyId);
            property.Title.Should().Be("Existing Property");
            property.Description.Should().Be("An existing property in the database.");
            property.Address.Should().Be("456 Existing Ave");
            property.Price.Should().Be(150000m);
            property.Area.Should().Be(150.0m);
            property.Rooms.Should().Be(3);
            property.Bathrooms.Should().Be(2);
            property.ConstructionYear.Should().Be(2010);
            property.UserId.Should().Be(UserId);
            property.CreatedAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(-10), precision: TimeSpan.FromSeconds(5));
            property.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(-10), precision: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task GivenExistingProperties_WhenGetAllIsCalled_ThenReturnsProperties()
        {
            // Arrange: Seed user and multiple properties
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedUser(db);
            SeedExistingProperty(db);
            SeedAnotherExistingProperty(db);

            // Act
            var response = await _client.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();

            // Assert
        }

        [Fact]
        public async Task GivenExistingProperty_WhenUpdateIsCalled_ThenPropertyIsUpdatedInDatabase()
        {
            // Arrange: Seed user and existing property
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedUser(db);
            SeedExistingProperty(db);

            var request = new UpdatePropertyRequest
            {
                Title = "Updated Property Title",
                Description = "Updated description of the property.",
                Address = "789 Updated Blvd",
                Price = 175000m,
                Area = 180.0m,
                Rooms = 4,
                Bathrooms = 3,
                ConstructionYear = 2012,
                UserId = UserId
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{ExistingPropertyId}", request);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Assert: Retrieve the updated property from the database
            using var assertScope = _factory.Services.CreateScope();
            var dbAssert = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var updatedProperty = await dbAssert.Properties.FindAsync(ExistingPropertyId);
            updatedProperty.Should().NotBeNull();
            updatedProperty!.Title.Should().Be(request.Title);
            updatedProperty.Description.Should().Be(request.Description);
            updatedProperty.Address.Should().Be(request.Address);
            updatedProperty.Price.Should().Be(request.Price);
            updatedProperty.Area.Should().Be(request.Area);
            updatedProperty.Rooms.Should().Be(request.Rooms);
            updatedProperty.Bathrooms.Should().Be(request.Bathrooms);
            updatedProperty.ConstructionYear.Should().Be(request.ConstructionYear);
            updatedProperty.UserId.Should().Be(request.UserId);
            updatedProperty.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task GivenExistingProperty_WhenDeleteIsCalled_ThenRemovesPropertyFromDatabase()
        {
            // Arrange: Seed user and existing property
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedUser(db);
            SeedExistingProperty(db);

            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/{ExistingPropertyId}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Assert: Attempt to retrieve the deleted property
            using var assertScope = _factory.Services.CreateScope();
            var dbAssert = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var deletedProperty = await dbAssert.Properties.FindAsync(ExistingPropertyId);
            deletedProperty.Should().BeNull();
        }

        [Fact]
        public async Task GivenNonExistingProperty_WhenGetByIdIsCalled_ThenReturnsNotFound()
        {
            // Arrange: No seeding needed for non-existing property

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Be("Property not found.");
        }

        [Fact]
        public async Task GivenInvalidPropertyData_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            // Arrange: Missing required fields or invalid data
            var invalidCommand = new CreatePropertyCommand
            {
                // Intentionally leaving out required fields or providing invalid data
                Title = "", // Missing title
                Description = "", // Missing description
                Address = "", // Missing address
                Price = -100m, // Invalid price
                Area = -50m, // Invalid area
                Rooms = -1, // Invalid rooms
                Bathrooms = -1, // Invalid bathrooms
                ConstructionYear = 1800, // Possibly invalid year
                UserId = Guid.Empty // Invalid UserId
            };

            // Act
            var response = await _client.PostAsJsonAsync(BaseUrl, invalidCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Image Handling Tests

        [Fact]
        public async Task GivenValidImages_WhenUploadPropertyImagesIsCalled_ThenImagesAreSavedToDatabase()
        {
            // Arrange: Seed user and existing property
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedUser(db);
            SeedExistingProperty(db);

            // Prepare mock image files
            var imageFiles = new List<IFormFile>
            {
                CreateMockFormFile("image1.jpg", "image/jpeg", "Dummy Image Content 1"),
                CreateMockFormFile("image2.png", "image/png", "Dummy Image Content 2")
            };

            var content = new MultipartFormDataContent();
            foreach (var file in imageFiles)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "files", file.FileName);
            }

            // Act
            var response = await _client.PostAsync($"{BaseUrl}/{ExistingPropertyId}/images", content);
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Helper Methods

        private void SeedUser(ApplicationDbContext db)
        {
            if (!db.Users.Any(u => u.Id == UserId))
            {
                var user = new UserInformation
                {
                    Id = UserId,
                    Username = "TestUser",
                    Email = "testuser@example.com",
                    FirstName = "Test",
                    LastName = "User",
                    Address = "123 Test St",
                    PhoneNumber = "123-456-7890",
                    Nationality = "TestLand",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    LastLogin = DateTime.UtcNow.AddDays(-1),
                    Status = UserStatus.ACTIVE,
                    Role = UserRole.PROFESSIONAL,
                    Company = "TestCo",
                    Type = "Professional"
                };
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        private void SeedExistingProperty(ApplicationDbContext db)
        {
            if (!db.Properties.Any(p => p.Id == ExistingPropertyId))
            {
                var property = new Property
                {
                    Id = ExistingPropertyId,
                    Title = "Existing Property",
                    Description = "An existing property in the database.",
                    Address = "456 Existing Ave",
                    Price = 150000m,
                    Area = 150.0m,
                    Rooms = 3,
                    Bathrooms = 2,
                    ConstructionYear = 2010,
                    UserId = UserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10),
                    Type = PropertyType.HOUSE,
                    Status = PropertyStatus.AVAILABLE,
                    User = db.Users.Find(UserId)
                };
                db.Properties.Add(property);
                db.SaveChanges();
            }
        }

        private void SeedAnotherExistingProperty(ApplicationDbContext db)
        {
            if (!db.Properties.Any(p => p.Id == AnotherPropertyId))
            {
                var property = new Property
                {
                    Id = AnotherPropertyId,
                    Title = "Another Property",
                    Description = "Another existing property in the database.",
                    Address = "789 Another Rd",
                    Price = 200000m,
                    Area = 180.0m,
                    Rooms = 4,
                    Bathrooms = 3,
                    ConstructionYear = 2012,
                    UserId = UserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    Status = PropertyStatus.AVAILABLE,
                    Type = PropertyType.APARTMENT,
                    User = db.Users.Find(UserId)
                };
                db.Properties.Add(property);
                db.SaveChanges();
            }
        }

        private void SeedExistingPropertyWithImage(ApplicationDbContext db)
        {
            if (!db.PropertyImages.Any(pi => pi.Id == ExistingImageId))
            {
                var image = new PropertyImage
                {
                    Id = ExistingImageId,
                    PropertyId = ExistingPropertyId,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    Url = "https://example.com"
                };
                db.PropertyImages.Add(image);
                db.SaveChanges();
            }

            if (!db.Properties.Any(p => p.Id == ExistingPropertyId))
            {
                // Seed the property if not already present
                SeedExistingProperty(db);
            }
        }

        private IFormFile CreateMockFormFile(string fileName, string contentType, string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            return new FormFile(stream, 0, stream.Length, "files", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        #endregion

        public void Dispose()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var dbIdentity = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            db.Database.EnsureDeleted();
            dbIdentity.Database.EnsureDeleted();
            _client.Dispose();
        }
    }
}
