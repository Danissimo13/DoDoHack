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
    public class NewsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public long Id { get; set; }

        public News News { get; set; }

        private readonly DodoBase _dbContext;

        public NewsModel(DodoBase db)
        {
            _dbContext = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            News = await _dbContext.Set<News>().FirstOrDefaultAsync(n => n.Id == Id);
            if(News == null) return LocalRedirect("~/Error/404");


            return Page();
        }
    }
}
