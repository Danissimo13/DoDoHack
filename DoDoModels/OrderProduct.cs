namespace DoDoModels
{
    public class OrderProduct
    {
        public long Id { get; set; }

        public Order Order { get; set; }
        public long OrderId { get; set; }

        public Product Product { get; set; }
        public long ProductId { get; set; }
    }
}
