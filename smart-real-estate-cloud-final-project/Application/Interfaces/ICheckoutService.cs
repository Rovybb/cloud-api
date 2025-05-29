namespace Application.Interfaces
{
    public interface ICheckoutService
    {
        Task<string> CreateCheckoutSessionAsync(
            decimal amount,
            string currency,
            string successUrl,
            string cancelUrl
        );
    }
}
