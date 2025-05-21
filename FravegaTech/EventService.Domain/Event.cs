namespace EventService.Domain
{
    public class Event
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
    }
}
