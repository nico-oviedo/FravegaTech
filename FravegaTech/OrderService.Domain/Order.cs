using OrderService.Domain.Enums;

namespace OrderService.Domain
{
    public class Order
    {
        public int OrderId { get; set; }
        public string ExternalReferenceId { get; set; }
        public SourceChannel Channel { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalValue { get; set; }
        //public Buyer Customer { get; set; }
        //public IEnumerable<OrderProduct> OrderProducts { get; set; }
        public OrderStatus Status { get; set; }
        //public IEnumerable<Event> Events { get; set; }
    }
}
