using DoDoHack.Data;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace DoDoHack.SignalHubs
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private const int MessageOnRequest = 10;

        private readonly DodoBase _dbContext;

        public ChatHub(DodoBase db)
        {
            _dbContext = db;
        }

        public async Task SendMessage(long receiverId, string message)
        {
            var senderId = long.Parse(Context.User.FindFirst("Id").Value);
            var messages = _dbContext.Set<ChatMessage>();

            ChatMessage newMessage = new ChatMessage()
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
            };
            await messages.AddAsync(newMessage);
            await _dbContext.SaveChangesAsync();

            await Clients.User(receiverId.ToString()).SendAsync("NewMessage", newMessage, senderId, receiverId);
            await Clients.Caller.SendAsync("NewMessage", newMessage, senderId, receiverId);
        }

        public async Task LoadMessages(long partnerId, int skipMessages)
        {
            var senderId = long.Parse(Context.User.FindFirst("Id").Value);
            var messages = _dbContext.Set<ChatMessage>().AsQueryable();
            messages = messages.Where(m => ((m.ReceiverId == partnerId) && (m.SenderId == senderId)) ||
                                           ((m.SenderId == partnerId) && (m.ReceiverId == senderId)))
                               .OrderByDescending(m => m.Id)
                               .Skip(skipMessages)
                               .Take(MessageOnRequest);

            await Clients.Caller.SendAsync("Enter", senderId);
            await Clients.Caller.SendAsync("LoadMessages", messages, senderId, partnerId);
        }
    }
}
