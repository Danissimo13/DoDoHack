namespace DoDoModels
{
    public class CourierStatistic
    {
        public long Id { get; set; }

        public long ClosedShiftsCount { get; set; }

        public long OpenedLinesCount { get; set; }

        public long ClosedOrdersCount { get; set; }

        public long CanceledOrdersCount { get; set; }
    }
}
