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
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = nameof(Courier))]
    public class ActionsModel : PageModel
    {
        public Courier Courier { get; set; }

        private readonly DodoBase _dbContext;

        public ActionsModel(DodoBase db)
        {
            _dbContext = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Courier = await _dbContext.Set<Courier>()
                                .FirstOrDefaultAsync(c => c.Id.ToString() == User.FindFirst("Id").Value);
            if(Courier == null) return LocalRedirect("~/Logout");

            return Page();
        }
    }
}
