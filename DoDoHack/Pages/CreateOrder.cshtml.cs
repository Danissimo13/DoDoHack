using DoDoHack.Data;
using DoDoHack.Services.Abstractions;
using DoDoHack.ViewModels;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoDoHack.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = nameof(Admin))]
    public class CreateOrderModel : PageModel
    {
        [BindProperty]
        public CreateOrderInput Input { get; set; }

        public IEnumerable<WorkZone> WorkZones { get; set; }

        private readonly DodoBase _dbContext;
        private readonly IOrderDistributionService _orderDistributionService;

        public CreateOrderModel(DodoBase db, IOrderDistributionService orderDistributionService)
        {
            _dbContext = db;
            _orderDistributionService = orderDistributionService;
        }

        public IActionResult OnGet()
        {
            WorkZones = _dbContext.Set<WorkZone>();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return OnGet();

            var orders = _dbContext.Set<Order>();
            var products = _dbContext.Set<Product>();
            var ordersProducts = _dbContext.Set<OrderProduct>();

            var productIds = Input.ProductsIds.Split(',').Select(i => long.Parse(i));

            Order order = new Order()
            {
                Address = Input.Address,
                Apartment = Input.Apartment,
                ClientPhone = Input.ClientPhone,
                Comment = Input.Comment,
                TotalCost = Input.TotalCost,
                CreatedTime = DateTime.Now,
                WorkZoneId = Input.WorkZoneId
            };

            foreach (int productId in productIds)
            {
                if (products.Any(p => p.Id == productId))
                {
                    ordersProducts.Add(new OrderProduct()
                    {
                        Order = order,
                        ProductId = productId
                    });
                }
            }

            await orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            await _orderDistributionService.DefineOrderToCourierAsync(order);

            return LocalRedirect($"~/Order/{order.Id}");
        }
    }
}
