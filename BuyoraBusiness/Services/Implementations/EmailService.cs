using BuyoraBusiness.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BuyoraBusiness.Services.Implementations
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smptClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential()
                {
                    UserName = "abdoelrahmanmohammed060@gmail.com",
                    Password = "qvpx fpwo yyyg vmwg"
                },
                EnableSsl = true
            };
            var mailMessage = new MailMessage()
            {
                From = new MailAddress("abdoelrahmanmohammed060@gmail.com"),
                Body = body,
                Subject = subject,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            await smptClient.SendMailAsync(mailMessage);
        }
    }
}
