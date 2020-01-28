using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServices.Services
{
    public static class SendMailsApi
    {
        public static async Task<bool> Execute(string userEmail, string userName, string subject, string plainTextContent, string htmlContent)
        {
            //var apiKey = Environment.GetEnvironmentVariable("apiKey");
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "Abdelali EL BAHLOUL");
            var to = new EmailAddress(userEmail, userName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return await Task.FromResult(true);
        }
    }
}
