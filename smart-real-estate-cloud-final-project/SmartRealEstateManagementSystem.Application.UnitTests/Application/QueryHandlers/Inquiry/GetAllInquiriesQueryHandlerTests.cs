using Application.DTOs;
using Application.QueryHandlers.Inquiry;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using InquiryEntities = Domain.Entities;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.QueryHandlers.Inquiry
{
    public class GetAllInquiriesQueryHandlerTests
    {
        private readonly IInquiryRepository inquiryRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly GetAllInquiriesQueryHandler handler;

        public GetAllInquiriesQueryHandlerTests()
        {
            inquiryRepositoryMock = Substitute.For<IInquiryRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new GetAllInquiriesQueryHandler(inquiryRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenInquiriesExist()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var inquiry = EntityFactory.CreateInquiry(mockId);
            var inquiries = new List<InquiryEntities.Inquiry> { inquiry };
            var inquiryDtos = inquiries.Select(EntityFactory.CreateInquiryDto).ToList();

            inquiryRepositoryMock.GetAllAsync().Returns(inquiries);
            mapperMock.Map<InquiryDto>(Arg.Any<InquiryEntities.Inquiry>()).Returns(callInfo =>
            {
                var inquiry = callInfo.Arg<InquiryEntities.Inquiry>();
                return EntityFactory.CreateInquiryDto(inquiry);
            });

            var query = EntityFactory.CreateGetAllInquiriesQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(inquiryDtos, result.Data.ToList(), new InquiryDtoComparer());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenNoInquiriesExist()
        {
            // Arrange
            inquiryRepositoryMock.GetAllAsync().Returns((IEnumerable<InquiryEntities.Inquiry>)null!);

            var query = EntityFactory.CreateGetAllInquiriesQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No inquiries found", result.ErrorMessage);
        }
    }

    public class InquiryDtoComparer : IEqualityComparer<InquiryDto>
    {
        public bool Equals(InquiryDto x, InquiryDto y)
        {
            if (x == null || y == null)
                return false;

            return x.Id == y.Id &&
                   x.PropertyId == y.PropertyId &&
                   x.AgentId == y.AgentId &&
                   x.ClientId == y.ClientId &&
                   x.Message == y.Message &&
                   x.Status == y.Status &&
                   x.CreatedAt == y.CreatedAt;
        }

        public int GetHashCode(InquiryDto obj)
        {
            return HashCode.Combine(obj.Id, obj.PropertyId, obj.AgentId, obj.ClientId, obj.Message, obj.Status, obj.CreatedAt);
        }
    }
}



