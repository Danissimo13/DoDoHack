namespace DoDoModels
{
    public class CourierWorkZone
    {
        public long Id { get; set; }

        public Courier PinnedCourier { get; set; }
        public long PinnedCourierId { get; set; }

        public WorkZone WorkZone { get; set; }
        public long WorkZoneId { get; set; }
    }
}
