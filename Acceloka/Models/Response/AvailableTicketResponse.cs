namespace Acceloka.Models.Response
{
    public class AvailableTicketResponse
    {
        public string CategoryName { get; set; } = string.Empty;
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public DateTimeOffset EventDate { get; set; }
        public decimal Price { get; set; }
        public int Quota { get; set; }
    }
}
