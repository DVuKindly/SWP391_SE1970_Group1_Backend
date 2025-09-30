using ClinicManagement.Application.Interfaces.Email;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtp;
        private readonly string _from;
        private readonly string _fromName;

        public EmailService(string host, int port, string from, string password, string fromName)
        {
            _from = from;
            _fromName = fromName;
            _smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(from, password),
                EnableSsl = true
            };
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_from, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(to);

            await _smtp.SendMailAsync(mail);
        }
    }
}
