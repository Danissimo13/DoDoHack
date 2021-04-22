using DoDoHack.Data;
using DoDoHack.Services.Abstractions;
using DoDoHack.ViewModels;
using DoDoModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoDoHack.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CourierModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public long Id { get; set; }

        [BindProperty]
        public CourierChangeInput Input { get; set; }

        public Courier Courier { get; set; }
        public IEnumerable<WorkZone> WorkZones { get; set; }

        private readonly DodoBase _dbContext;
        private readonly IOrderDistributionService _orderDistributionService;

        public CourierModel(DodoBase db, IOrderDistributionService orderDistributionService)
        {
            _dbContext = db;
            _orderDistributionService = orderDistributionService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.IsInRole(nameof(Admin)) && (User.FindFirst("Id").Value != Id.ToString())) return LocalRedirect("~/Denied");

            Courier = await _dbContext.Set<Courier>()
                                      .Include(c => c.Statistic)
                                      .Include(c => c.Orders)
                                      .Include(c => c.WorkZones)
                                      .Include(c => c.CourierActions)
                                      .Include(c => c.OrdersVision)
                                      .FirstOrDefaultAsync(z => z.Id == Id);
            WorkZones = _dbContext.Set<WorkZone>();

            if (Courier == null)
            {
                if (User.IsInRole(nameof(DoDoModels.Courier))) return LocalRedirect("~/Logout");
                else return LocalRedirect("~/Couriers");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostChangeAsync()
        {
            if (!ModelState.IsValid) return LocalRedirect($"~/Courier/{Id}");
            if (!User.IsInRole(nameof(Admin)) && (User.FindFirst("Id").Value != Id.ToString())) return LocalRedirect("~/Denied");

            var couriers = _dbContext.Set<Courier>();

            Courier courier = await couriers.FirstOrDefaultAsync(c => c.Id == Id);
            courier.Name = Input.Name;
            courier.Surname = Input.Surname;
            courier.Phone = Input.Phone;
            if (User.IsInRole(nameof(Admin))) courier.Rating = Input.Rating;

            couriers.Update(courier);
            await _dbContext.SaveChangesAsync();

            return LocalRedirect($"~/Courier/{Id}");
        }

        public async Task<IActionResult> OnPostShift()
        {
            if (!User.IsInRole(nameof(Admin))) return LocalRedirect("~/Denied");

            var couriers = _dbContext.Set<Courier>();
            var courierActions = _dbContext.Set<CourierAction>();
            var courierStatistics = _dbContext.Set<CourierStatistic>();

            Courier courier = await couriers.Include(c => c.Statistic).FirstOrDefaultAsync(c => c.Id == Id);
            if (courier == null) return LocalRedirect("~/Index");

            CourierAction action = new CourierAction() { ActionTime = DateTime.Now, Courier = courier };
            if (courier.ShiftOpen)
            {
                if(!courier.OnOrder) courier.OnLine = false;
                courier.AgreeAutoOrders = false;
                courier.Statistic.ClosedShiftsCount++;

                action.Discription = "Закрыли смену";
            }
            else action.Discription = "Открыли смену";

            courier.ShiftOpen = !courier.ShiftOpen;
            couriers.Update(courier);
            courierStatistics.Update(courier.Statistic);
            courierActions.Add(action);

            await _dbContext.SaveChangesAsync();

            return LocalRedirect($"~/Courier/{Id}");
        }

        public async Task<IActionResult> OnPostLineAsync()
        {
            if (User.FindFirst("Id").Value != Id.ToString()) return LocalRedirect("~/Denied");

            var couriers = _dbContext.Set<Courier>();
            var courierActions = _dbContext.Set<CourierAction>();
            var courierStats = _dbContext.Set<CourierStatistic>();

            Courier courier = await couriers.Include(c => c.Statistic).FirstOrDefaultAsync(c => c.Id == Id);
            if (courier == null) return LocalRedirect("~/Index");

            if (courier.OnLine || courier.ShiftOpen)
            {
                CourierAction action = new CourierAction() { ActionTime = DateTime.Now, Courier = courier };
                if (courier.OnLine)
                {
                    courier.AgreeAutoOrders = false;
                    action.Discription = "Закрыл линию";
                }
                else
                {
                    courier.Statistic.OpenedLinesCount++;
                    action.Discription = "Открыл линию";
                }

                courier.OnLine = !courier.OnLine;
                couriers.Update(courier);
                courierStats.Update(courier.Statistic);
                courierActions.Add(action);

                await _dbContext.SaveChangesAsync();
            }

            return LocalRedirect($"~/Courier/{Id}");
        }

        public async Task<IActionResult> OnPostAutoOrdersAsync()
        {
            if (User.FindFirst("Id").Value != Id.ToString()) return LocalRedirect("~/Denied");

            var couriers = _dbContext.Set<Courier>();
            var courierActions = _dbContext.Set<CourierAction>();

            Courier courier = await couriers.FirstOrDefaultAsync(c => c.Id == Id);
            if (courier == null) return LocalRedirect("~/Logout");

            if (courier.OnLine && courier.ShiftOpen)
            {
                CourierAction action = new CourierAction() { ActionTime = DateTime.Now, Courier = courier };
                if (courier.AgreeAutoOrders) action.Discription = "Отказался от автоматического принятии заказов.";
                else action.Discription = "Включил автоматическое принятие заказов.";

                courier.AgreeAutoOrders = !courier.AgreeAutoOrders;
                couriers.Update(courier);
                courierActions.Add(action);

                await _dbContext.SaveChangesAsync();

                if (courier.AgreeAutoOrders && (await _orderDistributionService.FindOrderToCourierAsync(courier)))
                    return LocalRedirect($"~/Order/{courier.OnOrderId}");
            }

            return LocalRedirect($"~/Courier/{Id}");
        }

        public async Task<IActionResult> OnPostDelete()
        {
            if (!User.IsInRole(nameof(Admin)) && (User.FindFirst("Id").Value != Id.ToString())) return LocalRedirect("~/Denied");

            var couriers = _dbContext.Set<Courier>();
            var courierActions = _dbContext.Set<CourierAction>();

            var courier = await couriers.FirstOrDefaultAsync(c => c.Id == Id);
            if (courier == null) return LocalRedirect("~/Index");

            couriers.Remove(courier);

            await _dbContext.SaveChangesAsync();

            if (User.IsInRole(nameof(DoDoModels.Courier)))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return LocalRedirect("~/Index");
        }

        public async Task<IActionResult> OnPostPinZoneAsync(long zoneId)
        {
            if (!User.IsInRole(nameof(Admin))) return LocalRedirect("~/Denied");

            var couriers = _dbContext.Set<Courier>();
            var workZones = _dbContext.Set<WorkZone>();
            var courierWorkZones = _dbContext.Set<CourierWorkZone>();

            Courier courier = await couriers.Include(c => c.WorkZones).FirstOrDefaultAsync(c => c.Id == Id);
            if(courier == null) return LocalRedirect("~/Couriers");

            WorkZone workZone = await workZones.FirstOrDefaultAsync(w => w.Id == zoneId);
            if (workZone == null) return LocalRedirect($"~/Courier/{Id}");

            if (courierWorkZones.Any(cw => (cw.WorkZoneId == workZone.Id) && (cw.PinnedCourierId == courier.Id)))
                return LocalRedirect($"~/Courier/{Id}");

            CourierWorkZone courierWorkZone = new CourierWorkZone()
            {
                PinnedCourier = courier,
                WorkZone = workZone
            };
            await courierWorkZones.AddAsync(courierWorkZone);

            await _dbContext.SaveChangesAsync();

            return LocalRedirect($"~/Courier/{Id}");
        }

        public async Task<IActionResult> OnPostUnpinZoneAsync(long zoneId)
        {
            if (!User.IsInRole(nameof(Admin))) return LocalRedirect("~/Denied");

            var couriers = _dbContext.Set<Courier>();
            var workZones = _dbContext.Set<WorkZone>();
            var courierWorkZones = _dbContext.Set<CourierWorkZone>();

            Courier courier = await couriers.Include(c => c.WorkZones).FirstOrDefaultAsync(c => c.Id == Id);
            if (courier == null) return LocalRedirect("~/Couriers");
            
            WorkZone workZone = await workZones.FirstOrDefaultAsync(w => w.Id == zoneId);
            if(workZone == null) return await OnGetAsync();

            CourierWorkZone courierWorkZone = await courierWorkZones.FirstOrDefaultAsync(cw => (cw.WorkZoneId == workZone.Id) && (cw.PinnedCourierId == courier.Id));
            if(courierWorkZone == null) return await OnGetAsync();

            courierWorkZones.Remove(courierWorkZone);

            await _dbContext.SaveChangesAsync();

            return LocalRedirect($"~/Courier/{Id}");
        }

        public async Task<IActionResult> OnPostOrdersVisionAsync(uint from, uint to)
        {
            if (!User.IsInRole(nameof(Admin))) return LocalRedirect("~/Denied");

            var couriers = _dbContext.Set<Courier>();
            var couriersOrdersVisions = _dbContext.Set<CourierOrdersVision>();

            Courier courier = await couriers.Include(c => c.OrdersVision).FirstOrDefaultAsync(c => c.Id == Id);
            if (courier == null) return LocalRedirect("~/Couriers");

            courier.OrdersVision.FromCost = from;
            courier.OrdersVision.ToCost = to;

            couriersOrdersVisions.Update(courier.OrdersVision);
            await _dbContext.SaveChangesAsync();

            return LocalRedirect($"~/Courier/{Id}");
        }
    }
}
