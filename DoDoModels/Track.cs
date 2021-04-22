using System;

namespace DoDoModels
{
    public class Track
    {
        public long Id { get; set; }

        public decimal Longitude { get; set; }

        public decimal Latitude { get; set; }

        public DateTime TrackTime { get; set; }

        public Courier Courier { get; set; }
        public long CourierId { get; set; }
    }
}
