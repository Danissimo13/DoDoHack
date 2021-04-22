using System.Collections.Generic;

namespace DoDoModels
{
    public class Courier : User
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Phone { get; set; }

        public bool ShiftOpen { get; set; }

        public bool OnLine { get; set; }

        public bool Accepted { get; set; }

        public bool OnOrder { get; set; }

        public long? OnOrderId { get; set; }

        public bool AgreeAutoOrders { get; set; }

        public decimal Rating { get; set; }

        public CourierStatistic Statistic { get; set; }
        public long StatisticId { get; set; }

        public CourierOrdersVision OrdersVision { get; set; }
        public long OrdersVisionId { get; set; }

        public IEnumerable<Order> Orders { get; set; }

        public IEnumerable<WorkZone> WorkZones { get; set; }

        public IEnumerable<CourierAction> CourierActions { get; set; }

        public IEnumerable<Track> Tracks { get; set; }

        public Courier()
        {
            Role = nameof(Courier);
        }
    }
}
