namespace SharedKernel.Dtos.Responses
{
    public class OrderTranslatedDto : OrderDto
    {
        public string ChannelTranslate { get; set; }
        public string StatusTranslate { get; set; }
    }
}
