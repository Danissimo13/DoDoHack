using DoDoHack.Data;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoDoHack.SignalHubs
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LineChatHub : Hub
    {
        private const int MessageOnRequest = 10;

        private readonly DodoBase _dbContext;

        public LineChatHub(DodoBase db)
        {
            _dbContext = db;
        }

        public async Task SendMessage(string message)
        {
            var senderId = long.Parse(Context.User.FindFirst("Id").Value);
            var messages = _dbContext.Set<LineChatMessage>();

            LineChatMessage newMessage = new LineChatMessage()
            {
                SenderId = senderId,
                Message = message,
                IsAdmin = Context.User.IsInRole(nameof(Admin))
            };

            await messages.AddAsync(newMessage);
            await _dbContext.SaveChangesAsync();

            await Clients.All.SendAsync("NewMessage", newMessage);
        }

        public async Task LoadMessages(int skipMessages)
        {
            var messages = _dbContext.Set<LineChatMessage>().AsQueryable();
            messages = messages.OrderByDescending(m => m.Id).Skip(skipMessages).Take(MessageOnRequest);

            await Clients.Caller.SendAsync("Enter", Context.User.FindFirst("Id").Value);
            await Clients.Caller.SendAsync("LoadMessages", messages);
        }
    }
}
