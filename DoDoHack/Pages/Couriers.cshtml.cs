using DoDoHack.Data;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace DoDoHack.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = nameof(Admin))]
    public class CouriersModel : PageModel
    {
        public const int CouriersOnPage = 10;

        [BindProperty(SupportsGet = true)]
        public int? PageNumber { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool OnlyFree { get; set; }

        public IEnumerable<Courier> Couriers { get; set; }

        private readonly DodoBase _dbContext;

        public CouriersModel(DodoBase db)
        {
            _dbContext = db;
        }

        public IActionResult OnGet()
        {
            if(!PageNumber.HasValue) PageNumber = 1;

            Couriers = _dbContext.Set<Courier>().AsQueryable().Where(c => c.Accepted);
            if(OnlyFree) Couriers = Couriers.Where(c => !c.OnOrder);
            Couriers = Couriers.Skip((PageNumber.Value - 1) * CouriersOnPage).Take(CouriersOnPage);

            return Page();
        }
    }
}
