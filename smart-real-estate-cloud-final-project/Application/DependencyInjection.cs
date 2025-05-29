using Application.Commands.Inquiry;
using Application.Commands.Payment;
using Application.Commands.Property;
using Application.Commands.User;
using Application.Utils;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssemblyContaining<CreatePropertyCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdatePropertyCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<CreatePaymentCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdatePaymentCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateUserInformationCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateUserInformationCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateInquiryCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateInquiryCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginUserCommandValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
