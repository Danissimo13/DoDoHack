using DoDoHack.Data;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoDoHack.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DayTracksModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public long Id { get; set; }

        public Courier Courier { get; set; }

        private readonly DodoBase _dbContext;

        public DayTracksModel(DodoBase db)
        {
            _dbContext = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if(!User.IsInRole(nameof(Admin)) && (Id.ToString() != User.FindFirst("Id").Value)) return LocalRedirect("~/Denied");

            Courier = await _dbContext.Set<Courier>().FirstOrDefaultAsync(c => c.Id == Id);
            if(Courier == null) return LocalRedirect("~/Error/404");

            return Page();
        }
    }
}
