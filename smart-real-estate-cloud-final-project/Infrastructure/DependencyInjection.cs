using Application.Interfaces;
using Domain.Filters;
using Domain.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IUserInformationRepository, UserInformationRepository>();
            services.AddScoped<IInquiryRepository, InquiryRepository>();


            services.AddScoped<TitleFilterStrategy>();
            services.AddScoped<DescriptionFilterStrategy>();
            services.AddScoped<PriceMinFilterStrategy>();
            services.AddScoped<PriceMaxFilterStrategy>();
            services.AddScoped<IPropertyFilterStrategy, TitleFilterStrategy>();
            services.AddScoped<IPropertyFilterStrategy, DescriptionFilterStrategy>();
            services.AddScoped<IPropertyFilterStrategy, PriceMinFilterStrategy>();
            services.AddScoped<IPropertyFilterStrategy, PriceMaxFilterStrategy>();
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<PropertyFilterService>();

            services.AddTransient<IEmailService, SendGridEmailService>();
            services.AddTransient<IImageStorageService, FirebaseImageStorageService>();
            services.AddTransient<ICheckoutService, StripeCheckoutService>();

            return services;
        }
    }
}
