using System.Collections.Generic;

namespace DoDoModels
{
    public class Product
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int Cost { get; set; }

        public string ImageName { get; set; }

        public IEnumerable<Order> Orders { get; set; }
    }
}
