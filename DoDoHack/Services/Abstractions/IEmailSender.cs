using System.Threading.Tasks;

namespace DoDoHack.Services.Abstractions
{
    public interface IEmailSender
    {
        public Task SendSupportEmailAsync(string subject, string bodyHtml);
    }
}
