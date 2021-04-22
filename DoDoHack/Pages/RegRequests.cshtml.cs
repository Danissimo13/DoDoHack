using DoDoHack.Data;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoDoHack.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = nameof(Admin))]
    public class RegRequestsModel : PageModel
    {
        [BindProperty]
        public long Id { get; set; }

        [BindProperty]
        public bool Accepted { get; set; }

        public IEnumerable<Courier> RegisrationCouriers { get; set; }

        private readonly DodoBase _dbContext;

        public RegRequestsModel(DodoBase db)
        {
            _dbContext = db;
        }

        public IActionResult OnGet()
        {
            DbSet<Courier> couriers = _dbContext.Set<Courier>();
            RegisrationCouriers = couriers.Where(c => c.Accepted == false);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            DbSet<Courier> couriers = _dbContext.Set<Courier>();

            Courier courier = await couriers.FirstOrDefaultAsync(c => c.Id == Id);
            if(courier == null) return Page();

            if (Accepted) 
            { 
                courier.Accepted = true;
                couriers.Update(courier);
            }
            else
            {
                couriers.Remove(courier);
            }

            await _dbContext.SaveChangesAsync();

            return LocalRedirect("~/RegRequests");
        }
    }
}
