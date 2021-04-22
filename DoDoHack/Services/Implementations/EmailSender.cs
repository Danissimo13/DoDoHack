using DoDoHack.Services.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace DoDoHack.Services.Implementations
{
    public class EmailSender : IEmailSender
    {
        private readonly string _supportEmail;
        private readonly string _supportPassword;

        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly bool _smtpUseSSL;

        public EmailSender(IConfiguration configuration)
        {
            var supportEmailData = configuration.GetSection("SupportEmail");
            _supportEmail = supportEmailData.GetValue<string>("Email");
            _supportPassword = supportEmailData.GetValue<string>("Password");

            var smtpData = configuration.GetSection("Smtp");
            _smtpServer = smtpData.GetValue<string>("Server");
            _smtpPort = smtpData.GetValue<int>("Port");
            _smtpUseSSL = smtpData.GetValue<bool>("UseSSL");
        }

        public async Task SendSupportEmailAsync(string subject, string bodyHtml)
        {
            MimeMessage emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("DodoSupport", _supportEmail));
            emailMessage.To.Add(new MailboxAddress("DodoSupport", _supportEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = bodyHtml
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, _smtpUseSSL);
                await client.AuthenticateAsync(_supportEmail, _supportPassword);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
