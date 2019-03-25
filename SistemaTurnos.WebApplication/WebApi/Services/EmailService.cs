using SendGrid;
using SendGrid.Helpers.Mail;
using SistemaTurnos.WebApplication.WebApi.Dto.Email;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class EmailService
    {
        private const string apiKey = "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";
        private readonly SendGridClient client = new SendGridClient(apiKey);

        public async void Send(EmailDto email)
        {
            var from = new EmailAddress(email.From);
            var subject = email.Subject;
            var to = email.To.Select(t => new EmailAddress(t)).ToList();
            var plainTextContent = "";
            var htmlContent = email.Message;
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
