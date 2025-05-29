using Application.Commands.Inquiry;
using Application.Commands.Payment;
using Application.Commands.Property;
using Application.Commands.User;
using Application.Contracts.Inquiry;
using Application.Contracts.Payment;
using Application.Contracts.Property;
using Application.Contracts.UserInformation;
using Application.DTOs;
using Application.Queries.Inquiry;
using Application.Queries.Payment;
using Application.Queries.Property;
using Application.Queries.UserInformation;
using Domain.Entities;
using Domain.Types.Inquiry;
using Domain.Types.Payment;
using Domain.Types.Property;
using Domain.Types.UserInformation;
using Domain.Utils;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Utils
{
    public static class EntityFactory
    {
        public static Payment CreatePayment(Guid mockId)
        {
            return new Payment
            {
                Id = mockId,
                Type = PaymentType.SALE,
                Date = DateTime.UtcNow,
                Price = 1000m,
                Status = PaymentStatus.COMPLETED,
                PaymentMethod = PaymentMethod.CREDIT_CARD,
                PropertyId = mockId,
                SellerId = mockId,
                BuyerId = mockId,
                Property = CreateProperty(mockId),
                Seller = CreateSeller(mockId),
                Buyer = CreateBuyer(mockId)
            };
        }

        public static Property CreateProperty(Guid mockId)
        {
            return new Property
            {
                Id = mockId,
                Title = "Sample Title",
                Description = "Sample Description",
                Price = 100000.00m,
                Address = "123 Sample Street",
                Area = 1500.00m,
                Rooms = 3,
                Type = PropertyType.HOUSE,
                Status = PropertyStatus.AVAILABLE,
                Bathrooms = 2,
                ConstructionYear = 2020,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = mockId,
                User = CreateUserInformation(mockId, "sampleuser", "sampleuser@example.com", "Sample", "User", "123 Sample Street", "123-456-7890", "Sample Nationality")
            };
        }

        public static UserInformation CreateSeller(Guid mockId)
        {
            return CreateUserInformation(mockId, "selleruser", "selleruser@example.com", "Seller", "User", "456 Seller Street", "987-654-3210", "Seller Nationality");
        }

        public static UserInformation CreateBuyer(Guid mockId)
        {
            return CreateUserInformation(mockId, "buyeruser", "buyeruser@example.com", "Buyer", "User", "789 Buyer Street", "321-654-9870", "Buyer Nationality");
        }

        public static UserInformation CreateUserInformation(Guid mockId, string username, string email, string firstName, string lastName, string address, string phoneNumber, string nationality)
        {
            return new UserInformation
            {
                Id = mockId,
                Username = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                PhoneNumber = phoneNumber,
                Nationality = nationality,
                CreatedAt = DateTime.UtcNow,
                Status = UserStatus.ACTIVE,
                Role = UserRole.CLIENT
            };
        }

        public static User CreateUser(Guid mockId, string email, string passwordHash)
        {
            return new User
            {
                Id = mockId,
                Email = email,
                PasswordHash = passwordHash
            };
        }

        public static CreatePaymentCommand CreatePaymentCommand(Guid mockId)
        {
            return new CreatePaymentCommand
            {
                Type = PaymentType.SALE,
                Date = DateTime.UtcNow,
                Price = 1000m,
                Status = PaymentStatus.COMPLETED,
                PaymentMethod = PaymentMethod.CREDIT_CARD,
                PropertyId = mockId,
                SellerId = mockId,
                BuyerId = mockId
            };
        }

        public static UpdatePaymentCommand CreateUpdatePaymentCommand(Guid mockId)
        {
            return new UpdatePaymentCommand
            {
                Id = mockId,
                Request = new UpdatePaymentRequest
                {
                    Type = PaymentType.SALE,
                    Date = DateTime.UtcNow,
                    Price = 1000m,
                    Status = PaymentStatus.COMPLETED,
                    PaymentMethod = PaymentMethod.CREDIT_CARD,
                    PropertyId = mockId,
                    SellerId = mockId,
                    BuyerId = mockId
                }
            };
        }

        public static CreatePropertyCommand CreatePropertyCommand(Guid mockId)
        {
            return new CreatePropertyCommand
            {
                Title = "Sample Title",
                Description = "Sample Description",
                Price = 100000m,
                Address = "Sample Address",
                Area = 120.5m,
                Rooms = 3,
                Bathrooms = 2,
                ConstructionYear = 2020,
                UserId = mockId
            };
        }

        public static DeletePropertyCommand CreateDeletePropertyCommand(Guid mockId)
        {
            return new DeletePropertyCommand
            {
                Id = mockId
            };
        }

        public static UpdatePropertyCommand CreateUpdatePropertyCommand(Guid mockId)
        {
            return new UpdatePropertyCommand
            {
                Id = mockId,
                Request = new UpdatePropertyRequest
                {
                    Title = "Updated Title",
                    Description = "Updated Description",
                    Price = 150000m,
                    Address = "Updated Address",
                    Area = 130.5m,
                    Rooms = 4,
                    Bathrooms = 3,
                    ConstructionYear = 2021,
                    UserId = mockId
                }
            };
        }

        public static GetAllPaymentsQuery CreateGetAllPaymentsQuery()
        {
            return new GetAllPaymentsQuery();
        }

        public static GetPaymentByIdQuery CreateGetPaymentByIdQuery(Guid mockId)
        {
            return new GetPaymentByIdQuery { Id = mockId };
        }

        public static GetAllPropertiesQuery CreateGetAllPropertiesQuery()
        {
            return new GetAllPropertiesQuery();
        }

        public static GetPropertyByIdQuery CreateGetPropertyByIdQuery(Guid mockId)
        {
            return new GetPropertyByIdQuery { Id = mockId };
        }

        public static PaymentDto CreatePaymentDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                Type = payment.Type,
                Date = payment.Date,
                Price = payment.Price,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                PropertyId = payment.PropertyId,
                SellerId = payment.SellerId,
                BuyerId = payment.BuyerId
            };
        }

        public static PropertyDto CreatePropertyDto(Property property)
        {
            return new PropertyDto
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                Price = property.Price,
                Address = property.Address,
                Type = PropertyType.HOUSE,
                Status = PropertyStatus.AVAILABLE,
                Area = property.Area,
                Rooms = property.Rooms,
                Bathrooms = property.Bathrooms,
                ConstructionYear = property.ConstructionYear,
                CreatedAt = property.CreatedAt,
                UpdatedAt = property.UpdatedAt,
                UserId = property.UserId
            };
        }

        public static PaginatedList<Property> CreatePaginatedProperties(List<Property> properties, int pageNumber, int pageSize)
        {
            return new PaginatedList<Property>(properties, properties.Count, pageNumber, pageSize);
        }
        public static CreateCheckoutCommand CreateCheckoutCommand(Guid mockId)
        {
            return new CreateCheckoutCommand
            {
                Type = PaymentType.SALE,
                Date = DateTime.UtcNow,
                Price = 1000m,
                Status = PaymentStatus.PENDING,
                PaymentMethod = PaymentMethod.CREDIT_CARD,
                PropertyId = mockId,
                SellerId = mockId,
                BuyerId = mockId,
                SuccessUrl = "https://example.com/payment-success",
                CancelUrl = "https://example.com/payment-cancel"
            };
        }
        public static CreateInquiryCommand CreateInquiryCommand(Guid mockId)
        {
            return new CreateInquiryCommand
            {
                Message = "Sample inquiry message",
                Status = InquiryStatus.PENDING,
                PropertyId = mockId,
                AgentId = mockId,
                ClientId = mockId
            };
        }
        public static Inquiry CreateInquiry(Guid mockId)
        {
            return new Inquiry
            {
                Id = mockId,
                Message = "Sample inquiry message",
                Status = InquiryStatus.PENDING,
                CreatedAt = DateTime.UtcNow,
                PropertyId = mockId,
                Property = CreateProperty(mockId),
                ClientId = mockId,
                Client = CreateUserInformation(mockId, "clientuser", "clientuser@example.com", "Client", "User", "789 Client Street", "321-654-9870", "Client Nationality"),
                AgentId = mockId,
                Agent = CreateUserInformation(mockId, "agentuser", "agentuser@example.com", "Agent", "User", "456 Agent Street", "987-654-3210", "Agent Nationality")
            };
        }
        public static UpdateInquiryCommand CreateUpdateInquiryCommand(Guid mockId)
        {
            return new UpdateInquiryCommand
            {
                Id = mockId,
                Request = new UpdateInquiryRequest
                {
                    Message = "Updated inquiry message",
                    CreatedAt = DateTime.UtcNow,
                    Status = InquiryStatus.ANSWERED,
                    PropertyId = mockId,
                    ClientId = mockId,
                    AgentId = mockId
                }
            };
        }
        public static CreateUserInformationCommand CreateUserInformationCommand(Guid mockId)
        {
            return new CreateUserInformationCommand
            {
                Email = "user@example.com",
                Username = "username",
                FirstName = "First",
                LastName = "Last",
                Address = "123 User Street",
                PhoneNumber = "123-456-7890",
                Nationality = "User Nationality",
                Status = UserStatus.ACTIVE,
                Role = UserRole.CLIENT,
                Company = "User Company",
                Type = "User Type"
            };
        }

        public static UpdateUserInformationCommand CreateUpdateUserInformationCommand(Guid mockId)
        {
            return new UpdateUserInformationCommand
            {
                Id = mockId,
                Request = new UpdateUserInformationRequest
                {
                    Email = "updateduser@example.com",
                    Username = "updatedusername",
                    FirstName = "UpdatedFirst",
                    LastName = "UpdatedLast",
                    Address = "456 Updated User Street",
                    PhoneNumber = "987-654-3210",
                    Nationality = "Updated User Nationality",
                    Status = UserStatus.ACTIVE,
                    Role = UserRole.CLIENT,
                    Company = "Updated User Company",
                    Type = "Updated User Type"
                }
            };
        }

        public static UserInformation CreateUserInformation(Guid mockId)
        {
            return new UserInformation
            {
                Id = mockId,
                Email = "user@example.com",
                Username = "username",
                FirstName = "First",
                LastName = "Last",
                Address = "123 User Street",
                PhoneNumber = "123-456-7890",
                Nationality = "User Nationality",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatus.ACTIVE,
                Role = UserRole.CLIENT,
                Company = "User Company",
                Type = "User Type"
            };
        }
        public static GetUserInformationByIdQuery CreateGetUserInformationByIdQuery(Guid mockId)
        {
            return new GetUserInformationByIdQuery
            {
                Id = mockId
            };
        }

        public static GetAllUserInformationsQuery CreateGetAllUserInformationsQuery()
        {
            return new GetAllUserInformationsQuery();
        }

        public static UserDto CreateUserDto(UserInformation userInformation)
        {
            return new UserDto
            {
                Id = userInformation.Id,
                Username = userInformation.Username,
                Email = userInformation.Email,
                FirstName = userInformation.FirstName,
                LastName = userInformation.LastName,
                Address = userInformation.Address,
                PhoneNumber = userInformation.PhoneNumber,
                Nationality = userInformation.Nationality,
                CreatedAt = userInformation.CreatedAt,
                LastLogin = userInformation.LastLogin,
                Status = userInformation.Status,
                Role = userInformation.Role
            };
        }
        public static GetInquiryByIdQuery CreateGetInquiryByIdQuery(Guid mockId)
        {
            return new GetInquiryByIdQuery
            {
                Id = mockId
            };
        }

        public static GetAllInquiriesQuery CreateGetAllInquiriesQuery()
        {
            return new GetAllInquiriesQuery();
        }

        public static InquiryDto CreateInquiryDto(Inquiry inquiry)
        {
            return new InquiryDto
            {
                Id = inquiry.Id,
                PropertyId = inquiry.PropertyId,
                AgentId = inquiry.AgentId,
                ClientId = inquiry.ClientId,
                Message = inquiry.Message,
                Status = inquiry.Status,
                CreatedAt = inquiry.CreatedAt
            };
        }
    }
}
