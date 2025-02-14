namespace Acceloka.Models.Request
{
    public class AvailableTicketParam
    {
        public string? CategoryName { get; set; }
        public string? TicketCode { get; set; }
        public string? TicketName { get; set; }
        public decimal? Price { get; set; }
        public DateTimeOffset? DateMin { get; set; }
        public DateTimeOffset? DateMax { get; set; }

        public string? OrderBy { get; set; }    // Kolom mana yang di-order (ticketCode, ticketName, price, eventDate, categoryName)
        public string? OrderState { get; set; } // "asc" atau "desc"

        public int? Page { get; set; }          // Untuk pagination, bonus
    }
}
