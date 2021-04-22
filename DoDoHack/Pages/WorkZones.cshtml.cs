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
    public class WorkZonesModel : PageModel
    {
        public const int ZonesOnPage = 10;

        [BindProperty(SupportsGet = true)]
        public int? PageNumber { get; set; }

        public string ZoneMessage { get; set; }
        public IEnumerable<WorkZone> WorkZones { get; set; }

        private readonly DodoBase _dbContext;

        public WorkZonesModel(DodoBase db)
        {
            _dbContext = db;
        }

        public IActionResult OnGet()
        {
            if (!PageNumber.HasValue) PageNumber = 1;

            WorkZones = _dbContext.Set<WorkZone>()
                                  .AsQueryable()
                                  .Include(z => z.PinnedCouriers)
                                  .Skip((PageNumber.Value - 1) * ZonesOnPage)
                                  .Take(ZonesOnPage);
            return Page();
        }

        public async Task<IActionResult> OnPostCreate(string zoneName)
        {
            if (string.IsNullOrWhiteSpace(zoneName))
            {
                ZoneMessage = "Вы не ввели название зоны";
                return OnGet();
            }

            var zones = _dbContext.Set<WorkZone>();
            if (zones.Any(z => (z.Name.ToLower() == zoneName.ToLower())))
            {
                ZoneMessage = "Зона с таким именем уже существует.";
                return OnGet();
            }

            WorkZone workZone = new WorkZone() { Name = zoneName };

            await zones.AddAsync(workZone);
            await _dbContext.SaveChangesAsync();

            ZoneMessage = "Зона успешно добавлена.";

            return OnGet();
        }

        public async Task<IActionResult> OnPostDelete(int? deleteId)
        {
            if (!deleteId.HasValue) return OnGet();

            var zones = _dbContext.Set<WorkZone>();

            var zone = await zones.FirstOrDefaultAsync(z => z.Id == deleteId);
            if (zone == null) return OnGet();

            zones.Remove(zone);
            await _dbContext.SaveChangesAsync();

            ZoneMessage = "Зона успешно удалена.";

            return OnGet();
        }
    }
}
