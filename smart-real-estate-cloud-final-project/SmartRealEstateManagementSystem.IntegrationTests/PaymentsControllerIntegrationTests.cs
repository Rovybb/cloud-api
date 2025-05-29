using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;
using Domain.Entities;
using Domain.Types.Property;
using Domain.Types.Payment;
using Domain.Types.UserInformation;
using Application.Commands.Payment;
using Application.Contracts.Payment;
using Application.DTOs;
using Microsoft.AspNetCore.Hosting;
using System.Net;


namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class PaymentsControllerIntegrationTests
        : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ApplicationDbContext _dbContext;
        private readonly UsersDbContext _dbIdentityContext;
        private readonly HttpClient client;

        private const string BaseUrl = "/api/v1/payments";

        private static readonly Guid SellerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid BuyerId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid PropertyId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        private static readonly Guid PaymentId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        public PaymentsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var dbIdentity = scopedServices.GetRequiredService<UsersDbContext>();
                    db.Database.EnsureCreated();
                    dbIdentity.Database.EnsureCreated();
                });
            });

            _dbContext = _factory.Services.GetRequiredService<ApplicationDbContext>();
            _dbIdentityContext = _factory.Services.GetRequiredService<UsersDbContext>();
            client = _factory.CreateClient();
        }

        [Fact]
        public async Task GivenValidPaymentData_WhenCreateIsCalled_ThenPaymentIsSavedToDatabase()
        {
            SeedUsersAndProperty();

            var command = new CreatePaymentCommand
            {
                Type = PaymentType.SALE,
                Date = DateTime.UtcNow,
                Price = 999.99m,
                Status = PaymentStatus.PENDING,
                PaymentMethod = PaymentMethod.CREDIT_CARD,
                PropertyId = PropertyId,
                SellerId = SellerId,
                BuyerId = BuyerId
            };

            var response = await client.PostAsJsonAsync(BaseUrl, command);
            response.EnsureSuccessStatusCode();

            var paymentInDb = await _dbContext.Payments.FirstOrDefaultAsync();
            paymentInDb.Should().NotBeNull();
            paymentInDb!.Price.Should().Be(999.99m);
            paymentInDb!.SellerId.Should().Be(SellerId);
            paymentInDb!.BuyerId.Should().Be(BuyerId);
            paymentInDb!.PropertyId.Should().Be(PropertyId);
            paymentInDb!.Type.Should().Be(PaymentType.SALE);
            paymentInDb!.Status.Should().Be(PaymentStatus.PENDING);
        }

        [Fact]
        public async Task GivenExistingPayment_WhenGetByIdIsCalled_ThenReturnsPayment()
        {
            SeedAll();
            var response = await client.GetAsync($"{BaseUrl}/{PaymentId}");
            response.EnsureSuccessStatusCode();

            var payment = await response.Content.ReadFromJsonAsync<PaymentDto>();
            payment.Should().NotBeNull();
            payment!.Id.Should().Be(PaymentId);
            payment!.Price.Should().Be(99.99m);
            payment!.SellerId.Should().Be(SellerId);
            payment!.BuyerId.Should().Be(BuyerId);
            payment!.PropertyId.Should().Be(PropertyId);
            payment!.Type.Should().Be(PaymentType.SALE);
            payment!.Status.Should().Be(PaymentStatus.PENDING);
            payment!.PaymentMethod.Should().Be(PaymentMethod.CREDIT_CARD);
        }

        [Fact]
        public async Task GivenExistingPayments_WhenGetAllIsCalled_ThenReturnsPayments()
        {
            SeedAll();
            var response = await client.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();

            var payments = await response.Content.ReadFromJsonAsync<List<PaymentDto>>();
            payments.Should().NotBeNull();
            payments.Should().HaveCount(1);
            payments.First().Id.Should().Be(PaymentId);
        }

        [Fact]
        public async Task GivenExistingPayment_WhenDeleteIsCalled_ThenRemovesPaymentFromDatabase()
        {
            SeedAll();
            var response = await client.DeleteAsync($"{BaseUrl}/{PaymentId}");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GivenUpdatedPayment_WhenUpdateIsCalled_ThenPaymentIsUpdatedInDatabase()
        {
            SeedAll();

            var request = new UpdatePaymentRequest
            {
                Type = PaymentType.RENT,
                Date = DateTime.UtcNow,
                Price = 500.00m,
                Status = PaymentStatus.COMPLETED,
                PaymentMethod = PaymentMethod.BANK_TRANSFER,
                PropertyId = PropertyId,
                SellerId = SellerId,
                BuyerId = BuyerId
            };

            var response = await client.PutAsJsonAsync($"{BaseUrl}/{PaymentId}", request);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GivenValidCheckoutData_WhenCreateCheckoutIsCalled_ThenPaymentIsCreated()
        {
            SeedUsersAndProperty();
            var command = new CreateCheckoutCommand
            {
                Type = PaymentType.SALE,
                Date = DateTime.UtcNow,
                Price = 999.99m,
                Status = PaymentStatus.PENDING,
                PaymentMethod = PaymentMethod.CREDIT_CARD,
                PropertyId = PropertyId,
                SellerId = SellerId,
                BuyerId = BuyerId
            };
            var response = await client.PostAsJsonAsync($"{BaseUrl}/create-checkout-session", command);
            response.EnsureSuccessStatusCode();
            var paymentInDb = await _dbContext.Payments.FirstOrDefaultAsync();
            paymentInDb.Should().NotBeNull();
            paymentInDb!.Price.Should().Be(999.99m);
            paymentInDb!.SellerId.Should().Be(SellerId);
            paymentInDb!.BuyerId.Should().Be(BuyerId);
            paymentInDb!.PropertyId.Should().Be(PropertyId);
            paymentInDb!.Type.Should().Be(PaymentType.SALE);
            paymentInDb!.Status.Should().Be(PaymentStatus.PENDING);
        }

        public void Dispose()
        {
            var scope = _factory.Services.CreateScope();
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                var dbIdentityContext = services.GetRequiredService<UsersDbContext>();
                dbContext.Database.EnsureDeleted();
                dbIdentityContext.Database.EnsureDeleted();
                client.Dispose();
        }

        private void SeedUsersAndProperty()
        {
            if (!_dbContext.Users.Any(u => u.Id == SellerId))
            {
                var sellerUser = new UserInformation
                {
                    Id = SellerId,
                    Username = "SellerUser",
                    Email = "seller@example.com",
                    FirstName = "Seller",
                    LastName = "User",
                    Address = "123 Seller St",
                    PhoneNumber = "123-456-7890",
                    Nationality = "SellerLand",
                    CreatedAt = DateTime.UtcNow,
                    Status = UserStatus.ACTIVE,
                    Role = UserRole.PROFESSIONAL
                };
                _dbContext.Users.Add(sellerUser);
            }

            if (!_dbContext.Users.Any(u => u.Id == BuyerId))
            {
                var buyerUser = new UserInformation
                {
                    Id = BuyerId,
                    Username = "BuyerUser",
                    Email = "buyer@example.com",
                    FirstName = "Buyer",
                    LastName = "User",
                    Address = "456 Buyer Rd",
                    PhoneNumber = "987-654-3210",
                    Nationality = "BuyerLand",
                    CreatedAt = DateTime.UtcNow,
                    Status = UserStatus.ACTIVE,
                    Role = UserRole.CLIENT
                };
                _dbContext.Users.Add(buyerUser);
            }

            if (!_dbContext.Properties.Any(p => p.Id == PropertyId))
            {
                var property = new Property
                {
                    Id = PropertyId,
                    Title = "Test Property",
                    Description = "Property belonging to SellerUser",
                    Type = PropertyType.HOUSE,
                    Status = PropertyStatus.AVAILABLE,
                    Price = 10000m,
                    Address = "100 Real Estate Way",
                    Area = 120.5m,
                    Rooms = 3,
                    Bathrooms = 2,
                    ConstructionYear = 2020,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = SellerId,
                    User = _dbContext.Users.Find(SellerId)
                };
                _dbContext.Properties.Add(property);
            }

            _dbContext.SaveChanges();
        }

        private void SeedAll()
        {
            SeedUsersAndProperty();

            if (!_dbContext.Payments.Any(p => p.Id == PaymentId))
            {
                var payment = new Payment
                {
                    Id = PaymentId,
                    Type = PaymentType.SALE,
                    Date = DateTime.UtcNow,
                    Price = 99.99m,
                    Status = PaymentStatus.PENDING,
                    PaymentMethod = PaymentMethod.CREDIT_CARD,
                    PropertyId = PropertyId,
                    SellerId = SellerId,
                    BuyerId = BuyerId,
                    Property = _dbContext.Properties.Find(PropertyId),
                    Seller = _dbContext.Users.Find(SellerId),
                    Buyer = _dbContext.Users.Find(BuyerId)
                };
                _dbContext.Payments.Add(payment);
            }

            _dbContext.SaveChanges();
        }
    }
}
