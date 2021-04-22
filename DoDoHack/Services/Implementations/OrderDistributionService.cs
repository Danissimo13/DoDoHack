using DoDoHack.Data;
using DoDoHack.Services.Abstractions;
using DoDoModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoDoHack.Services.Implementations
{
    public class OrderDistributionService : IOrderDistributionService
    {
        private readonly DodoBase _dbContext;

        public OrderDistributionService(DodoBase db)
        {
            _dbContext = db;
        }

        public async Task<bool> DefineOrderToCourierAsync(Order order)
        {
            if(order.Closed || order.CourierId.HasValue) return false;

            var freeCouriers = _dbContext.Set<Courier>()
                                     .Include(c => c.WorkZones)
                                     .Where(c => c.AgreeAutoOrders && c.ShiftOpen && c.OnLine && !c.OnOrder)
                                     .OrderByDescending(c => c.Rating);
            Order foundOrder = await _dbContext.Set<Order>()
                                         .Include(o => o.WorkZone)
                                         .FirstOrDefaultAsync(o => o.Id == order.Id);

            Courier foundCourier = null;
            if ((foundOrder.WorkZoneId.HasValue) && freeCouriers.Any(c => c.WorkZones.Any(w => w.Id == foundOrder.WorkZoneId.Value)))
            {
                foundCourier = await freeCouriers.FirstOrDefaultAsync(c => c.WorkZones.Any(w => w.Id == foundOrder.WorkZoneId.Value));
            }
            else if (freeCouriers.Any())
            {
                foundCourier = await freeCouriers.FirstOrDefaultAsync();
            }

            if(foundCourier != null)
            {
                var couriers = _dbContext.Set<Courier>();
                var orders = _dbContext.Set<Order>();
                var couriersActions = _dbContext.Set<CourierAction>();

                CourierAction action = new CourierAction()
                {
                    ActionTime = DateTime.Now,
                    Courier = foundCourier,
                    Discription = $"Назначена доставка заказа №{foundOrder.Id} по {foundOrder.Address}."
                };

                foundCourier.OnOrder = true;
                foundCourier.OnOrderId = foundOrder.Id;
                foundOrder.Courier = foundCourier;

                couriersActions.Add(action);
                couriers.Update(foundCourier);
                orders.Update(foundOrder);

                await _dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> FindOrderToCourierAsync(Courier courier)
        {
            if(!courier.AgreeAutoOrders || !courier.ShiftOpen || !courier.OnLine || courier.OnOrder) return false;

            var freeOrders = _dbContext.Set<Order>()
                                       .Include(o => o.WorkZone)
                                       .Where(o => !o.Closed && !o.CourierId.HasValue)
                                       .OrderBy(o => o.CreatedTime);
            var foundCourier = await _dbContext.Set<Courier>()
                                         .Include(c => c.WorkZones)
                                         .FirstOrDefaultAsync(c => c.Id == courier.Id);

            Order foundOrder = null;
            if (foundCourier.WorkZones.Any())
            {
                foundOrder = freeOrders.Where(o => o.WorkZoneId.HasValue).AsEnumerable().FirstOrDefault(o => foundCourier.WorkZones.Any(w => w.Id == o.WorkZoneId.Value));
            }

            if ((foundOrder == null) && freeOrders.Any())
            {
                foundOrder = await freeOrders.FirstOrDefaultAsync();
            }

            if(foundOrder != null)
            {
                var couriers = _dbContext.Set<Courier>();
                var orders = _dbContext.Set<Order>();
                var couriersActions = _dbContext.Set<CourierAction>();

                CourierAction action = new CourierAction()
                {
                    ActionTime = DateTime.Now,
                    Courier = foundCourier,
                    Discription = $"Назначена доставка заказа №{foundOrder.Id} по {foundOrder.Address}."
                };

                foundCourier.OnOrder = true;
                foundCourier.OnOrderId = foundOrder.Id;
                foundOrder.Courier = foundCourier;

                couriersActions.Add(action);
                couriers.Update(foundCourier);
                orders.Update(foundOrder);

                await _dbContext.SaveChangesAsync();

                return true;
            }
            
            return false;
        }
    }
}
