namespace Acceloka.Models.Response
{
    public class BookedTicketItemDetail
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTimeOffset EventDate { get; set; }
    }
}
