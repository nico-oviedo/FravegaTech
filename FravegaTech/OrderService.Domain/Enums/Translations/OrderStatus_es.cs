namespace OrderService.Domain.Enums.Translations
{
    public static class OrderStatus_es
    {
        public static readonly Dictionary<OrderStatus, string> Translations = new()
        {
            { OrderStatus.Created, "Creada" },
            { OrderStatus.PaymentReceived, "Pago recibido" },
            { OrderStatus.Invoiced, "Facturada" },
            { OrderStatus.Returned, "Devuelta" },
            { OrderStatus.Cancelled, "Cancelada" }
        };
    }
}
