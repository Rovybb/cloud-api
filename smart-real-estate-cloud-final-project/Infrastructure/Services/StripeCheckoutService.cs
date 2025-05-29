using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class StripeCheckoutService : ICheckoutService
    {
        private readonly string _secretKey;

        public StripeCheckoutService(IConfiguration configuration)
        {
            _secretKey = configuration["Stripe:SecretKey"]
                ?? throw new Exception("Stripe SecretKey not found in configuration.");
        }

        public async Task<string> CreateCheckoutSessionAsync(
            decimal amount,
            string currency,
            string successUrl,
            string cancelUrl)
        {
            StripeConfiguration.ApiKey = _secretKey;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(amount * 100), // convert to cents
                            Currency = currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Real Estate Payment"
                            },
                        },
                        Quantity = 1,
                    },
                },
                SuccessUrl = successUrl,  
                CancelUrl = cancelUrl     
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            // session.Url is the Stripe-hosted checkout page
            return session.Url;
        }
    }
}
