using SharedKernel.Dtos.Requests;

namespace SharedKernel.Dtos
{
    public class OrderDto : OrderRequestDto
    {
        public int OrderId { get; set; }

        public string Status { get; set; }

        public List<EventDto> Events { get; set; }
    }
}
