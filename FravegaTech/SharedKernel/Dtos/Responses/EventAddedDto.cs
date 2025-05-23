namespace SharedKernel.Dtos.Responses
{
    public class EventAddedDto
    {
        public int OrderId { get; set; }
        public string PreviousStatus { get; set; }
        public string NewStatus { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
