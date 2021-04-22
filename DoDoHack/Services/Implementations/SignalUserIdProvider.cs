using Microsoft.AspNetCore.SignalR;

namespace DoDoHack.Services.Implementations
{
    public class SignalUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("Id")?.Value;
        }
    }
}
