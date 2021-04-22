using DoDoHack.Services.Abstractions;
using DoDoModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace DoDoHack.Services.Implementations
{
    public class SupportService : ISupportService
    {
        private readonly IEmailSender _emailSender;

        public SupportService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendSOSEmailAsync(Courier sender, string locationRef)
        {
            string subject = $"SOS сигнал от {sender.Name} {sender.Surname}";
            StringBuilder htmlMessage = new StringBuilder();

            #region HtmlMailCreate
            htmlMessage.Append("<div style=\"display:flex;flex-direction:column;border-radius:5px;background-color:#ff6900;color:white !important;text-align:center;font-family:Trebuchet MS;padding:20px;\">");
            htmlMessage.Append($"<h1>{sender.Name} {sender.Surname} вызывает срочную помощь</h1>");
            htmlMessage.Append($"<h2>Id Курьера: {sender.Id}</h2>");
            htmlMessage.Append($"<h1>Телефон курьера: {sender.Phone}</h1>");
            htmlMessage.Append($"<h2>Статус курьера: {(sender.OnOrder ? "На заказе" : "Свободен")}</h2>");
            htmlMessage.Append($"<h1>Примерное расположение: <a href=\"{locationRef}\" style=\"color:white !important;\">Локация курьера</a></h1>");
            htmlMessage.Append($"<img src=\"https://cdn1.flamp.ru/22732ae9adbcd1570b07be33424c814f.png\" width=\"300\" style=\"border-radius:150px\"/>");
            htmlMessage.Append("</div>");
            #endregion HtmlMailCreate

            await _emailSender.SendSupportEmailAsync(subject, htmlMessage.ToString());
        }
    }
}
