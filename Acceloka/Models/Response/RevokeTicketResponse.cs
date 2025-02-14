namespace Acceloka.Models.Response
{
    public class RevokeTicketResponse
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
