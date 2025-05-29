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
    public class GetInquiryByIdQueryHandlerTests
    {
        private readonly IInquiryRepository inquiryRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly GetInquiryByIdQueryHandler handler;

        public GetInquiryByIdQueryHandlerTests()
        {
            inquiryRepositoryMock = Substitute.For<IInquiryRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new GetInquiryByIdQueryHandler(inquiryRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenInquiryExists()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var inquiry = EntityFactory.CreateInquiry(mockId);
            var inquiryDto = EntityFactory.CreateInquiryDto(inquiry);

            inquiryRepositoryMock.GetByIdAsync(inquiry.Id).Returns(Result<InquiryEntities.Inquiry>.Success(inquiry));
            mapperMock.Map<InquiryDto>(inquiry).Returns(inquiryDto);

            var query = EntityFactory.CreateGetInquiryByIdQuery(inquiry.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(inquiryDto, result.Data);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenInquiryDoesNotExist()
        {
            // Arrange
            var inquiryId = Guid.NewGuid();
            inquiryRepositoryMock.GetByIdAsync(inquiryId).Returns(Result<InquiryEntities.Inquiry>.Failure("Inquiry not found."));

            var query = EntityFactory.CreateGetInquiryByIdQuery(inquiryId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Inquiry not found.", result.ErrorMessage);
        }
    }
}



