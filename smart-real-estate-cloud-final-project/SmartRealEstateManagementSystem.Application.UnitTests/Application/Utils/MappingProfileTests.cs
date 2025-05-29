using Application.DTOs;
using Application.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Types.Payment;
using Domain.Types.Property;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.Utils
{
    public class MappingProfileTests
    {
        private readonly IMapper mapper;

        public MappingProfileTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            mapper = config.CreateMapper();
        }

        [Fact]
        public void Should_Map_Property_To_PropertyDto()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var property = EntityFactory.CreateProperty(mockId);

            // Act
            var propertyDto = mapper.Map<PropertyDto>(property);

            // Assert
            Assert.Equal(property.Id, propertyDto.Id);
            Assert.Equal(property.Title, propertyDto.Title);
            Assert.Equal(property.Description, propertyDto.Description);
            Assert.Equal(property.Price, propertyDto.Price);
            Assert.Equal(property.Address, propertyDto.Address);
            Assert.Equal(property.Area, propertyDto.Area);
            Assert.Equal(property.Rooms, propertyDto.Rooms);
            Assert.Equal(property.Type, propertyDto.Type);
            Assert.Equal(property.Status, propertyDto.Status);
            Assert.Equal(property.Bathrooms, propertyDto.Bathrooms);
            Assert.Equal(property.ConstructionYear, propertyDto.ConstructionYear);
            Assert.Equal(property.CreatedAt, propertyDto.CreatedAt);
            Assert.Equal(property.UpdatedAt, propertyDto.UpdatedAt);
            Assert.Equal(property.UserId, propertyDto.UserId);
            Assert.Equal(property.PropertyImages.Select(img => img.Url).ToList(), propertyDto.ImageUrls);
        }

        [Fact]
        public void Should_Map_Payment_To_PaymentDto()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var payment = EntityFactory.CreatePayment(mockId);

            // Act
            var paymentDto = mapper.Map<PaymentDto>(payment);

            // Assert
            Assert.Equal(payment.Id, paymentDto.Id);
            Assert.Equal(payment.Type, paymentDto.Type);
            Assert.Equal(payment.Date, paymentDto.Date);
            Assert.Equal(payment.Price, paymentDto.Price);
            Assert.Equal(payment.Status, paymentDto.Status);
            Assert.Equal(payment.PaymentMethod, paymentDto.PaymentMethod);
            Assert.Equal(payment.PropertyId, paymentDto.PropertyId);
            Assert.Equal(payment.SellerId, paymentDto.SellerId);
            Assert.Equal(payment.BuyerId, paymentDto.BuyerId);
        }
    }
}

