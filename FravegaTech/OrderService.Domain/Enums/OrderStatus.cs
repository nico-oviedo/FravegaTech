namespace OrderService.Domain.Enums
{
    public enum OrderStatus
    {
        Created,
        PaymentReceived,
        Invoiced,
        Returned,
        Cancelled
    }
}
