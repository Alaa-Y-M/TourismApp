using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace GradProj.Services
{
    public static class SendGridApi
    {
        public static async Task<bool> Execute(string hostName, string userEmail, string userName, string plainTextContent, string htmlContent, string subject)

        {

            var apiKey = "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";

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("test@example.com", hostName);

            var to = new EmailAddress(userEmail, userName);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

            return await Task.FromResult(true);
        }
    }
}
