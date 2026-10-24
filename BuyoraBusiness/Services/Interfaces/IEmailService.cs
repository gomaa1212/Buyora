using System;
using System.Collections.Generic;
using System.Text;

namespace BuyoraBusiness.Services.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body);
    }
}
