using System;
using System.Collections.Generic;

namespace DoDoModels
{
    public class Order
    {
        public long Id { get; set; }

        public string ClientPhone { get; set; }

        public string Comment { get; set; }

        public string Address { get; set; }

        public string Apartment { get; set; }

        public int TotalCost { get; set; }

        public DateTime CreatedTime { get; set; }

        public bool Closed { get; set; }

        public IEnumerable<Product> Products { get; set; }

        public long? WorkZoneId { get; set; }
        public WorkZone WorkZone { get; set; }

        public long? CourierId { get; set; }
        public Courier Courier { get; set; }
    }
}
