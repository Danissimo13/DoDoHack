using DoDoHack.Data;
using DoDoHack.Services.Abstractions;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoDoHack.SignalHubs
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = nameof(Courier))]
    public class SupportHub : Hub
    {
        private readonly DodoBase _dbContext;
        private readonly ISupportService _supportService;

        public SupportHub(DodoBase db, ISupportService supportService)
        {
            _dbContext = db;
            _supportService = supportService;
        }

        public async Task SOS(string location)
        {
            long senderId = long.Parse(Context.User.FindFirst("Id").Value);
            Courier courier = await _dbContext.Set<Courier>().FirstOrDefaultAsync(c => c.Id == senderId);
            if(courier == null) return;

            await _supportService.SendSOSEmailAsync(courier, location);
            await Clients.Caller.SendAsync("SOSSent");
        }
    }
}
