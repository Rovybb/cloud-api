using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;
using Domain.Entities;
using Domain.Types.Property;
using Domain.Types.UserInformation;
using Domain.Types.Inquiry;
using Application.DTOs;
using Application.Commands.Inquiry;
using Application.Contracts.Inquiry;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class InquiriesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _dbName;

        private const string BaseUrl = "/api/v1/inquiries";

        // Predefined GUIDs for testing
        private static readonly Guid AgentId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        private static readonly Guid ClientId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        private static readonly Guid PropertyId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        private static readonly Guid InquiryId = Guid.Parse("88888888-8888-8888-8888-888888888888");

        public InquiriesControllerIntegrationTests()
        {
            // Generate a unique in-memory database name per test instance for isolation
            _dbName = Guid.NewGuid().ToString();

            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    // Remove existing DbContext registrations if any
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
        public async Task GivenValidInquiryData_WhenCreateIsCalled_ThenInquiryIsSavedToDatabase()
        {
            // Arrange: Seed users and property
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SeedUsersAndProperty(db);
            }

            var command = new CreateInquiryCommand
            {
                Message = "I am interested in this property.",
                Status = InquiryStatus.PENDING,
                PropertyId = PropertyId,
                AgentId = AgentId,
                ClientId = ClientId
            };

            // Act
            var response = await _client.PostAsJsonAsync(BaseUrl, command);
            response.EnsureSuccessStatusCode();

            // Extract the created Inquiry ID from the response
            var createdInquiryId = await response.Content.ReadFromJsonAsync<Guid>();

            // Assert: Retrieve the inquiry from the database
            using var assertScope = _factory.Services.CreateScope();
            var dbAssert = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var inquiryInDb = await dbAssert.Inquiries
                .Include(i => i.Property)
                .Include(i => i.Client)
                .Include(i => i.Agent)
                .FirstOrDefaultAsync(i => i.Id == createdInquiryId);

            inquiryInDb.Should().NotBeNull();
            inquiryInDb!.Message.Should().Be(command.Message);
            inquiryInDb.Status.Should().Be(command.Status);
            inquiryInDb.PropertyId.Should().Be(command.PropertyId);
            inquiryInDb.AgentId.Should().Be(command.AgentId);
            inquiryInDb.ClientId.Should().Be(command.ClientId);
        }

        [Fact]
        public async Task GivenExistingInquiry_WhenGetByIdIsCalled_ThenReturnsInquiry()
        {
            // Arrange: Seed all data
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedAll(db);

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{InquiryId}");
            response.EnsureSuccessStatusCode();

            // Assert
            var inquiry = await response.Content.ReadFromJsonAsync<InquiryDto>();
            inquiry.Should().NotBeNull();
            inquiry!.Id.Should().Be(InquiryId);
            inquiry.Message.Should().Be("Initial inquiry message.");
            inquiry.Status.Should().Be(InquiryStatus.PENDING);
            inquiry.PropertyId.Should().Be(PropertyId);
            inquiry.AgentId.Should().Be(AgentId);
            inquiry.ClientId.Should().Be(ClientId);
        }

        [Fact]
        public async Task GivenExistingInquiry_WhenGetAllIsCalled_ThenReturnsInquiries()
        {
            // Arrange: Seed all data
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedAll(db);

            // Act
            var response = await _client.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();

            // Assert
            var inquiries = await response.Content.ReadFromJsonAsync<List<InquiryDto>>();
            inquiries.Should().NotBeNull();
            inquiries.Should().HaveCount(1);
            inquiries.First().Id.Should().Be(InquiryId);
        }

        [Fact]
        public async Task GivenExistingInquiry_WhenDeleteIsCalled_ThenRemovesInquiryFromDatabase()
        {
            // Arrange: Seed all data
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedAll(db);

            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/{InquiryId}");
            response.EnsureSuccessStatusCode();

            // Assert: Attempt to retrieve the deleted inquiry
            using var assertScope = _factory.Services.CreateScope();
            var dbAssert = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var inquiryInDb = await dbAssert.Inquiries.FindAsync(InquiryId);
            inquiryInDb.Should().BeNull();
        }

        [Fact]
        public async Task GivenUpdatedInquiry_WhenUpdateIsCalled_ThenInquiryIsUpdatedInDatabase()
        {
            // Arrange: Seed all data
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedAll(db);

            var request = new UpdateInquiryRequest
            {
                Message = "Updated inquiry message.",
                Status = InquiryStatus.ANSWERED,
                CreatedAt = DateTime.UtcNow,
                PropertyId = PropertyId,
                AgentId = AgentId,
                ClientId = ClientId
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{InquiryId}", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update failed: {error}");
            }
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Assert: Retrieve the updated inquiry
            using var assertScope = _factory.Services.CreateScope();
            var dbAssert = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var updatedInquiry = await dbAssert.Inquiries.FindAsync(InquiryId);
            updatedInquiry.Should().NotBeNull();
            updatedInquiry!.Message.Should().Be(request.Message);
            updatedInquiry.Status.Should().Be(request.Status);
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

        private void SeedUsersAndProperty(ApplicationDbContext db)
        {
            // Seed Agent User
            if (!db.Users.Any(u => u.Id == AgentId))
            {
                var agentUser = new UserInformation
                {
                    Id = AgentId,
                    Username = "AgentUser",
                    Email = "agent@example.com",
                    FirstName = "Agent",
                    LastName = "User",
                    Address = "789 Agent St",
                    PhoneNumber = "555-555-5555",
                    Nationality = "AgentLand",
                    CreatedAt = DateTime.UtcNow,
                    Status = UserStatus.ACTIVE,
                    Role = UserRole.PROFESSIONAL
                };
                db.Users.Add(agentUser);
            }

            // Seed Client User
            if (!db.Users.Any(u => u.Id == ClientId))
            {
                var clientUser = new UserInformation
                {
                    Id = ClientId,
                    Username = "ClientUser",
                    Email = "client@example.com",
                    FirstName = "Client",
                    LastName = "User",
                    Address = "321 Client Rd",
                    PhoneNumber = "444-444-4444",
                    Nationality = "ClientLand",
                    CreatedAt = DateTime.UtcNow,
                    Status = UserStatus.ACTIVE,
                    Role = UserRole.CLIENT
                };
                db.Users.Add(clientUser);
            }

            // Seed Property
            if (!db.Properties.Any(p => p.Id == PropertyId))
            {
                var property = new Property
                {
                    Id = PropertyId,
                    Title = "Test Property",
                    Description = "Property belonging to AgentUser",
                    Type = PropertyType.HOUSE,
                    Status = PropertyStatus.AVAILABLE,
                    Price = 150000m,
                    Address = "456 Real Estate Blvd",
                    Area = 200.0m,
                    Rooms = 4,
                    Bathrooms = 3,
                    ConstructionYear = 2018,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = AgentId,
                    User = db.Users.Find(AgentId)
                };
                db.Properties.Add(property);
            }

            db.SaveChanges();
        }

        private void SeedAll(ApplicationDbContext db)
        {
            SeedUsersAndProperty(db);

            if (!db.Inquiries.Any(i => i.Id == InquiryId))
            {
                var inquiry = new Inquiry
                {
                    Id = InquiryId,
                    CreatedAt = DateTime.UtcNow,
                    Message = "Initial inquiry message.",
                    Status = InquiryStatus.PENDING,
                    PropertyId = PropertyId,
                    AgentId = AgentId,
                    ClientId = ClientId,
                    Property = db.Properties.Find(PropertyId),
                    Agent = db.Users.Find(AgentId),
                    Client = db.Users.Find(ClientId)
                };
                db.Inquiries.Add(inquiry);
            }

            db.SaveChanges();
        }
    }
}
