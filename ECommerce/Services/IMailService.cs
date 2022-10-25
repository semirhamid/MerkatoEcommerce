using SendGrid.Helpers.Mail;
using SendGrid;

namespace ECommerce.Services
{
    public interface IMailService
    {
        Task sendEmailAsync(string Emailto, string subject, string content);
    }

    public class SendGridMailService : IMailService
    {
        private readonly IConfiguration configuration;

        public SendGridMailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task sendEmailAsync(string Emailto, string subject, string content)
        {
            var apiKey = configuration["SendGridMailService"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("semhamirid@gmail.com", "ECommerce");
            var to = new EmailAddress(Emailto);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
