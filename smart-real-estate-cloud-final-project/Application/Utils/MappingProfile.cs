using Application.Commands.Inquiry;
using Application.Commands.Payment;
using Application.Commands.Property;
using Application.Commands.User;
using Application.Contracts.Inquiry;
using Application.Contracts.Payment;
using Application.Contracts.Property;
using Application.Contracts.UserInformation;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Utils;

namespace Application.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Property, PropertyDto>()
                .ForMember(
                    dest => dest.ImageUrls,
                    opt => opt.MapFrom(src => src.PropertyImages.Select(img => img.Url).ToList())
                )
                .ReverseMap();

            CreateMap<Property, CreatePropertyCommand>().ReverseMap();
            CreateMap<Property, UpdatePropertyRequest>().ReverseMap();

            CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>))
                .ConvertUsing(typeof(PaginatedListConverter<,>));

            CreateMap<Payment, PaymentDto>().ReverseMap();
            CreateMap<Payment, CreatePaymentCommand>().ReverseMap();
            CreateMap<Payment, UpdatePaymentRequest>().ReverseMap();
            CreateMap<Payment, CreateCheckoutCommand>().ReverseMap();

            CreateMap<UserInformation, UserDto>().ReverseMap();
            CreateMap<UserInformation, CreateUserInformationCommand>().ReverseMap();
            CreateMap<UserInformation, UpdateUserInformationRequest>().ReverseMap();

            CreateMap<Inquiry, InquiryDto>().ReverseMap();
            CreateMap<Inquiry, CreateInquiryCommand>().ReverseMap();
            CreateMap<Inquiry, UpdateInquiryRequest>().ReverseMap();
        }
    }


    public class PaginatedListConverter<TSource, TDestination> : ITypeConverter<PaginatedList<TSource>, PaginatedList<TDestination>>
    {
        public PaginatedList<TDestination> Convert(PaginatedList<TSource> source, PaginatedList<TDestination> destination, ResolutionContext context)
        {
            var items = context.Mapper.Map<List<TDestination>>(source.Items);
            return new PaginatedList<TDestination>(items, source.TotalCount, source.PageNumber, source.PageSize);
        }
    }
}
