using System;

namespace DoDoModels
{
    public class CourierAction
    {
        public long Id { get; set; }

        public string Discription { get; set; }

        public DateTime ActionTime { get; set; }

        public Courier Courier { get; set; }
        public long CourierId { get; set; }
    }
}
