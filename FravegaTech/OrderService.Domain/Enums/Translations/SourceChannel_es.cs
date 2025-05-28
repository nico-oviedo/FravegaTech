namespace OrderService.Domain.Enums.Translations
{
    public static class SourceChannel_es
    {
        public static readonly Dictionary<SourceChannel, string> Translations = new()
        {
            { SourceChannel.Ecommerce, "Comercio electrónico" },
            { SourceChannel.CallCenter, "Centro de llamadas" },
            { SourceChannel.Store, "Local comercial" },
            { SourceChannel.Affiliate, "Filial" }
        };
    }
}
