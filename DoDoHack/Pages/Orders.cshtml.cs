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
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class OrdersModel : PageModel
    {
        public const int OrdersOnPage = 10;

        [BindProperty(SupportsGet = true)]
        public int? PageNumber { get; set; }

        public IEnumerable<Order> Orders { get; set; }

        private readonly DodoBase _dbContext;

        public OrdersModel(DodoBase db)
        {
            _dbContext = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if(!PageNumber.HasValue) PageNumber = 1;

            Orders = _dbContext.Set<Order>()
                               .Include(o => o.WorkZone)
                               .Include(o => o.Products);


            if (User.IsInRole(nameof(Courier)))
            {
                long courierId = long.Parse(User.FindFirst("Id").Value);
                Courier courier = await _dbContext.Set<Courier>()
                                                  .Include(c => c.OrdersVision)  
                                                  .FirstOrDefaultAsync(c => c.Id == courierId);

                Orders = Orders.Where(o => !o.Closed && !o.CourierId.HasValue)
                               .Where(o => (o.TotalCost >= courier.OrdersVision.FromCost) && (o.TotalCost <= courier.OrdersVision.ToCost));      
            }
            else
            { 
                Orders = Orders.OrderByDescending(o => o.Closed).ThenBy(o => o.CourierId.HasValue);
            }
            Orders = Orders.Skip((PageNumber.Value - 1) * OrdersOnPage).Take(OrdersOnPage);

            return Page();
        }
    }
}
