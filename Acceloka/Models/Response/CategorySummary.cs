namespace Acceloka.Models.Response
{
    public class CategorySummary
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal SummaryPrice { get; set; }       // Total per kategori
        public List<BookedTicketItemResponse> Tickets { get; set; } = new();
    }
}
