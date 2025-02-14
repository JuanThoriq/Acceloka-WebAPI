namespace Acceloka.Models.Response
{
    public class BookedTicketItemResponse
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public decimal Price { get; set; }      // Harga total (ticket.Price * quantity)
    }
}
