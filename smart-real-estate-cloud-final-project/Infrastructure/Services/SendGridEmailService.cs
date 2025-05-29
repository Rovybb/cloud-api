using SendGrid;
using SendGrid.Helpers.Mail;
using Application.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class SendGridEmailService(IConfiguration configuration) : IEmailService
{
    private readonly string _apiKey = configuration["SendGrid:ApiKey"];

    public async Task SendEmailAsync(string to, string subject, string plainTextContent, string htmlContent = null)
    {
        var client = new SendGridClient(_apiKey);

        var from = new EmailAddress("flaviburca@gmail.com", "Smart Real Estate Management System");
        var toDetails = new EmailAddress(to, "Agent");

        // If you want the HTML content as well, pass it as the fifth argument.
        // The third argument is the subject, the fourth is the plain text content,
        // and the fifth is the HTML content.
        var msg = MailHelper.CreateSingleEmail(from, toDetails, subject, plainTextContent, htmlContent);

        var response = await client.SendEmailAsync(msg);
        // Optionally, handle response status or errors here
    }
}
