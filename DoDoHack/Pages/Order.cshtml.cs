using DoDoHack.Data;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DoDoHack.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class OrderModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public long Id { get; set; }

        public Order Order { get; set; }
        public Courier Courier { get; set; }

        private readonly DodoBase _dbContext;

        public OrderModel(DodoBase db)
        {
            _dbContext = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Order = await _dbContext.Set<Order>()
                                    .Include(o => o.Products)
                                    .Include(o => o.WorkZone)
                                    .Include(o => o.Courier)
                                    .FirstOrDefaultAsync(o => o.Id == Id);
            Courier = await _dbContext.Set<Courier>()
                                      .Include(c => c.OrdersVision)
                                      .FirstOrDefaultAsync(c => c.Id.ToString() == User.FindFirst("Id").Value);
            if(Order == null) return LocalRedirect("~/Error/404");

            if (User.IsInRole(nameof(DoDoModels.Courier)) && ((Order.TotalCost <= Courier.OrdersVision.FromCost) || (Order.TotalCost >= Courier.OrdersVision.ToCost))) return LocalRedirect("~/Denied");

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if(!User.IsInRole(nameof(Admin))) return LocalRedirect("~/Orders");

            var orders = _dbContext.Set<Order>();
            var couriers = _dbContext.Set<Courier>();

            Order order = await orders.Include(o => o.Courier).FirstOrDefaultAsync(o => o.Id == Id);

            if (order.CourierId.HasValue)
            {
                order.Courier.OnOrder = false;
                order.Courier.OnOrderId = null;
                couriers.Update(order.Courier);
            }

            orders.Remove(order);
            await _dbContext.SaveChangesAsync();

            return LocalRedirect($"~/Orders");
        }

        public async Task<IActionResult> OnPostTakeAsync()
        {
            if (!User.IsInRole(nameof(Courier))) return LocalRedirect("~/Orders");
            long courierId = long.Parse(User.FindFirst("Id").Value);

            var orders = _dbContext.Set<Order>();
            var couriers = _dbContext.Set<Courier>();
            var courierActions = _dbContext.Set<CourierAction>();

            Order order = await orders.FirstOrDefaultAsync(o => o.Id == Id);
            Courier courier = await couriers.Include(c => c.OrdersVision).FirstOrDefaultAsync(c => c.Id == courierId);
            
            if((!courier.OnLine) || (!courier.ShiftOpen) || (courier.OnOrder) || (order.Closed) || (order.CourierId.HasValue)) return LocalRedirect("~/Denied");
            if ((order.TotalCost <= courier.OrdersVision.FromCost) || (order.TotalCost >= courier.OrdersVision.ToCost)) return LocalRedirect("~/Denied");

            CourierAction takeAction = new CourierAction()
            {
                Discription = $"Взял заказ №{order.Id} по {order.Address}",
                ActionTime = DateTime.Now,
                Courier = courier
            };
            order.Courier = courier;
            courier.OnOrder = true;
            courier.OnOrderId = order.Id;
            
            courierActions.Add(takeAction);
            couriers.Update(courier);
            orders.Update(order);
            await _dbContext.SaveChangesAsync();

            return LocalRedirect($"~/Order/{Id}");
        }

        public async Task<IActionResult> OnPostCancelAsync()
        {
            if (!User.IsInRole(nameof(Courier))) return LocalRedirect("~/Orders");
            long courierId = long.Parse(User.FindFirst("Id").Value);

            var orders = _dbContext.Set<Order>();
            var couriers = _dbContext.Set<Courier>();
            var courierActions = _dbContext.Set<CourierAction>();
            var courierStats = _dbContext.Set<CourierStatistic>();

            Order order = await orders.FirstOrDefaultAsync(o => o.Id == Id);
            Courier courier = await couriers.Include(c => c.Statistic).FirstOrDefaultAsync(c => c.Id == courierId);

            if ((!courier.OnOrder) || (order.CourierId != courier.Id)) return LocalRedirect("~/Denied");

            CourierAction takeAction = new CourierAction()
            {
                Discription = $"Отказался от заказа №{order.Id} по {order.Address}",
                ActionTime = DateTime.Now,
                Courier = courier
            };

            order.CourierId = null;
            order.Courier = null;
            courier.OnOrder = false;
            courier.OnOrderId = null;
            courier.Statistic.CanceledOrdersCount++;

            orders.Update(order);
            courierActions.Add(takeAction);
            couriers.Update(courier);
            courierStats.Update(courier.Statistic);

            await _dbContext.SaveChangesAsync();

            return LocalRedirect($"~/Order/{Id}");
        }

        public async Task<IActionResult> OnPostCloseAsync()
        {
            if (!User.IsInRole(nameof(DoDoModels.Courier))) return LocalRedirect("~/Orders");
            long courierId = long.Parse(User.FindFirst("Id").Value);

            var orders = _dbContext.Set<Order>();
            var couriers = _dbContext.Set<Courier>();
            var courierActions = _dbContext.Set<CourierAction>();
            var courierStats = _dbContext.Set<CourierStatistic>();

            Order order = await orders.FirstOrDefaultAsync(o => o.Id == Id);
            Courier courier = await couriers.Include(c => c.Statistic).FirstOrDefaultAsync(c => c.Id == courierId);

            if ((!courier.OnOrder) || (order.CourierId != courier.Id)) return LocalRedirect("~/Denied");

            CourierAction takeAction = new CourierAction()
            {
                Discription = $"Закрыл заказ №{order.Id} по {order.Address}",
                ActionTime = DateTime.Now,
                Courier = courier
            };

            order.Closed = true;
            courier.OnOrder = false;
            courier.OnOrderId = null;
            courier.Statistic.ClosedOrdersCount++;

            orders.Update(order);
            courierActions.Add(takeAction);
            couriers.Update(courier);
            courierStats.Update(courier.Statistic);
            await _dbContext.SaveChangesAsync();

            return LocalRedirect($"~/Order/{Id}");
        }
    }
}
